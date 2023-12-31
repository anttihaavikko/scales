using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Utils.DevMenu;
using AnttiStarterKit.Visuals;
using Leaderboards;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    [SerializeField] protected SkillPool skillPool;
    [SerializeField] protected Deck deck;
    [SerializeField] protected Hand hand;
    [SerializeField] protected List<Slot> slots;
    [SerializeField] protected Camera cam;
    [SerializeField] protected ScoreDisplay scoreDisplay;
    [SerializeField] protected Dragon dragon;
    [SerializeField] protected Appearer continueButton;
    [SerializeField] protected StrikeDisplay strikeDisplay;
    [SerializeField] private DevMenu devMenu;
    [SerializeField] protected SkillIcons skillIcons;
    [SerializeField] private Tooltip tooltip;
    [SerializeField] protected Appearer splash;
    [SerializeField] protected GameObject gameOver;
    [SerializeField] protected int music;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] protected float showContinueButtonAfter = -1f;

    private IEnumerable<Card> AllCards => deck.Cards.Concat(hand ? hand.Cards : new List<Card>());
    private bool continued;
    private EffectCamera effectCamera;

    protected bool hasExtras;
    protected bool hasEnded;

    public Dragon Dragon => dragon;
    public Tooltip Tooltip => tooltip;
    public Camera Camera => cam;
    public bool HasEnded => hasEnded;

    private void Awake()
    {
        effectCamera = cam.GetComponent<EffectCamera>();

        if (strikeDisplay)
        {
            strikeDisplay.onEnd += Ended;
        }
    }

    private void Start()
    {
        if (music >= 0)
        {
            AudioManager.Instance.ChangeMusic(music, 0.75f, 0f, 0.25f);
        }
        
        hasExtras = State.Instance.Has(Effect.Backbag);
        
        slots.ForEach(slot => slot.click += SlotClicked);

        if (scoreDisplay)
        {
            var multi = State.Instance.Has(Effect.RetainMulti) ? State.Instance.HeldMulti : 1;
            scoreDisplay.Set(State.Instance.Score, multi);
        }

        if (skillPool && devMenu && skillIcons)
        {
            devMenu.Setup(skillPool.All.Select(s => new DevMenuOption(s.Title, () =>
            {
                var skill = s.Spawn();
                State.Instance.Add(skill);
                skillIcons.Add(skill);
            })));
        }
    }

    private void Update()
    {
        if(Input.anyKeyDown && splash) splash.Hide();
            
        if(DevKey.Down(KeyCode.C)) continueButton.Show();
        
        if (DevKey.Down(KeyCode.Z))
        {
            Tweener.ZoomTo(cam, 12, 1f);
        }
    }

    public void Ended()
    {
        hasEnded = true;
        gameOver.SetActive(true);
        AudioManager.Instance.CancelPitching();
        AudioManager.Instance.TargetPitch = 0;

        if (scoreManager)
        {
            var plr = PlayerPrefs.GetString("PlayerName", "Anon");
            var id = PlayerPrefs.GetString("PlayerId", "123");
            scoreManager.SubmitScore(plr, scoreDisplay.Total, State.Instance.LevelMulti, id);   
        }
    }

    public void ToRewards()
    {
        if (continued) return;
        continued = true;
        continueButton.Hide();
        var delay = Mathf.Clamp(AddStrikes(), 0, State.Instance.MaxStrikes) * 0.3f;
        this.StartCoroutine(() =>
        {
            if (hasEnded) return;
            State.Instance.HeldMulti = scoreDisplay.Multi;
            State.Instance.RoundEnded(scoreDisplay.Total);
        }, delay);
    }
    
    protected void TriggerSelect()
    {
        var selected = GetVisibleCards().Where(c => c.IsSelected).ToList();
        if (selected.Any(c => c.IsTimer))
        {
            ReSelect();
        }
        Invoke(nameof(TriggerSelect), 1f);
    }

    protected void DeselectAll()
    {
        AllCards.Where(c => c.IsSelected).ToList().ForEach(c => c.ChangeSelection(false));
    }
    
    private void SlotClicked(Slot slot)
    {
        var card = AllCards.FirstOrDefault(c => c.IsSelected);

        if (slot.Accepts && card)
        {
            DropToSlot(card, slot);
        }
    }

    public bool TryCombine(Card first, Card second)
    {
        if (!CanCombine(first, second)) return false;
        Combine(first, second);
        return true;
    }

    protected void PlayGenericInstant(Card card, bool isPlayer = true)
    {
        var p = card.transform.position;
        
        if (card.Is(CardType.Kill))
        {
            strikeDisplay.AddStrikes(1, true);
            var amt = State.Instance.GetCount(Effect.Pestilence);
            if (amt > 0)
            {
                scoreDisplay.Add(1000 * amt * card.Multiplier * State.Instance.LevelMulti);
                ShowScore(1000, amt * card.Multiplier, p);
            }
        }

        if (card.Is(CardType.Lotus))
        {
            var multi = isPlayer ? 1 : -1;
            var size = GetHandSize() * multi;
            scoreDisplay.Add(30 * card.Multiplier * size * State.Instance.LevelMulti);
            ShowScore(30 * size, card.Multiplier, p);
            ResetMulti();
        }
    }

    protected void ResetMulti()
    {
        if (State.Instance.HasTrifecta())
        {
            scoreDisplay.DecreaseMulti();
            scoreDisplay.Set(scoreDisplay.Total, Mathf.CeilToInt(scoreDisplay.Multi * 0.5f));
            return;
        }
        
        scoreDisplay.ResetMulti();
    }

    protected void ShowScore(long total, int multi, Vector3 p)
    {
        ResetInactivityTimer();
        
        Shake(0.2f);
        var multiText = $"<color=#CDE7B0><size=5>x{multi}</size></color>";
        if (multi > 1)
        {
            EffectManager.AddTextPopup(multiText, p + Vector3.down * 0.3f + Vector3.right * 0.3f, 1);
        }
        EffectManager.AddTextPopup(total.AsScore(), p);
    }
    
    protected static bool CanSubTo(IList<int> set, int sum, bool exact = false)
    {
        return set.Any(num =>
        {
            var left = num - sum;
            if (left == 0 && (!exact || set.Count == 1))
            {
                return true;
            }

            var index = set.IndexOf(num);
            var possible = set.Where((n, i) => i != index).ToList();
            return possible.Any() && CanAddTo(possible, left, exact);
        });
    }

    protected static bool CanAddTo(IList<int> set, int sum, bool exact = false)
    {
        return set.Any(num =>
        {
            var left = sum - num;
            if (left == 0 && (!exact || set.Count == 1))
            {
                return true;
            }

            var index = set.IndexOf(num);
            var possible = set.Where((n, i) => n <= sum && i != index).ToList();
            return possible.Any() && CanAddTo(possible, left, exact);
        });
    }

    public int GetJokerValue()
    {
        return GetVisibleCards().Where(c => !c.IsJoker && !c.IsTrueJoker).Sum(c => c.Number);
    }

    protected void Perfect()
    {
        var levels = State.Instance.GetCount(Effect.PerfectGame);
        if (levels > 0)
        {
            scoreDisplay.Add(1000 * levels * State.Instance.LevelMulti);
            ShowScore(1000 * levels, State.Instance.LevelMulti, Vector3.zero);
        }

        if (State.Instance.Has(Effect.PerfectHeal))
        {
            strikeDisplay.AddStrikes(-1);
        }
    }

    protected void AfterPlay(Card card)
    {
        card.IncreaseNumber();

        if (card.IsAce)
        {
            scoreDisplay.AddMulti();
        }

        if (card.IsJoker)
        {
            var heals = State.Instance.GetCount(Effect.JokerHeal);
            if (heals > 0)
            {
                strikeDisplay.AddStrikes(-heals);
            }   
        }

        var increases = State.Instance.GetCount(Effect.IncreaseMultiOn, card.Number);
        if (increases > 0)
        {
            scoreDisplay.AddMulti(increases);
        }
    }

    protected void BecameStuck()
    {
        hasExtras = false;
        
        var lvl = State.Instance.GetCount(Effect.Backbag);
        if (lvl > 0)
        {
            var cards = new List<CardData>();
            for (var i = 0; i < lvl; i++)
            {
                cards.Add(CardData.GetRandomNonModifier());
            }
            AddExtras(cards);
        }
    }
    
    protected abstract void Combine(Card first, Card second);
    public abstract void Setup();
    public abstract void Select(Card card);
    public abstract void DropToSlot(Card card, Slot slot);
    public abstract bool CanCombine(Card first, Card second);
    public abstract void RightClick(Card card);
    public abstract bool CanPlay(Card card);
    public abstract int AddStrikes();
    public abstract void PlayInstant(Card card);
    public abstract IReadOnlyCollection<Card> GetVisibleCards();
    public abstract int GetHandSize();
    public abstract int GetTrueJokerValue();
    public abstract void AddExtras(List<CardData> cards);

    public void Shake(float amount)
    {
        effectCamera.BaseEffect(amount);
    }

    protected abstract void ReSelect();

    public int GetMultiplier()
    {
        return scoreDisplay.Multi;
    }

    public void ResetInactivityTimer()
    {
        if (!(showContinueButtonAfter >= 0)) return;
        CancelInvoke(nameof(ShowContinue));
        Invoke(nameof(ShowContinue), showContinueButtonAfter);
    }

    private void ShowContinue()
    {
        continueButton.Show();
    }
}