using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Utils.DevMenu;
using AnttiStarterKit.Visuals;
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

    private IEnumerable<Card> AllCards => deck.Cards.Concat(hand ? hand.Cards : new List<Card>());
    private bool continued;
    private EffectCamera effectCamera;

    public Dragon Dragon => dragon;
    public Tooltip Tooltip => tooltip;
    public Camera Camera => cam;

    private void Awake()
    {
        effectCamera = cam.GetComponent<EffectCamera>();
    }

    private void Start()
    {
        slots.ForEach(slot => slot.click += SlotClicked);

        if (scoreDisplay)
        {
            scoreDisplay.Set(State.Instance.Score);
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
        if(DevKey.Down(KeyCode.C)) continueButton.Show();
    }

    public void ToRewards()
    {
        if (continued) return;
        continued = true;
        continueButton.Hide();
        var delay = Mathf.Clamp(AddStrikes(), 0, State.Instance.MaxStrikes) * 0.3f;
        this.StartCoroutine(() => State.Instance.RoundEnded(scoreDisplay.Total), delay);
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

    protected void PlayGenericInstant(Card card)
    {
        var p = card.transform.position;
        
        if (card.Is(CardType.Kill))
        {
            strikeDisplay.AddStrikes(1, true);
            var amt = State.Instance.GetCount(Effect.Pestilence);
            if (amt > 0)
            {
                scoreDisplay.Add(1000 * amt * card.Multiplier);
                ShowScore(1000, amt * card.Multiplier, p);
            }
        }

        if (card.Is(CardType.Lotus))
        {
            var size = GetHandSize();
            scoreDisplay.Add(30 * card.Multiplier * size);
            ShowScore(30 * size, card.Multiplier, p);
            scoreDisplay.ResetMulti();
        }
    }

    protected void ShowScore(int total, int multi, Vector3 p)
    {
        Shake(0.2f);
        var multiText = $"<color=#CDE7B0><size=5>x{multi}</size></color>";
        if (multi > 1)
        {
            EffectManager.AddTextPopup(multiText, p + Vector3.down * 0.3f + Vector3.right * 0.3f, 1);
        }
        EffectManager.AddTextPopup($"{total}", p);
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
        return GetVisibleCards().Where(c => !c.IsJoker).Sum(c => c.Number);
    }

    protected void AfterPlay(Card card)
    {
        var heals = State.Instance.GetCount(Effect.JokerHeal);
        if (card.IsJoker && heals > 0)
        {
            strikeDisplay.AddStrikes(-heals);
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

    public void Shake(float amount)
    {
        effectCamera.BaseEffect(amount);
    }
}