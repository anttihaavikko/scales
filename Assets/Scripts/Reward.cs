using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Reward : GameMode
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private TMP_Text pageText, countText;
    [SerializeField] private Transform contentStart;
    [SerializeField] private Appearer phoneButton;
    [SerializeField] private TMP_Text pickLabel;
    [SerializeField] private RectTransform pickBg;
    [SerializeField] private SpriteRenderer deckBorders;

    private Card modifier;
    private int picks = 3;
    private bool picked;
    private int page;
    private List<Card> cardOptions = new();
    
    private const int PerRow = 12;
    private const int PageSize = PerRow * 2;

    private int MaxPage => Mathf.CeilToInt(deck.Cards.Count(c => !c.IsUsed) * 1f / PageSize);

    public override void Setup()
    {
        MoveDeck();
        picks = 3 + State.Instance.GetCount(Effect.Greed);
        ShowCards(5 + State.Instance.GetCount(Effect.MoreOptions));

        if (State.Instance.Has(Effect.Phone))
        {
            this.StartCoroutine(() =>
            {
                phoneButton.Show();
                AudioManager.Instance.PlayEffectAt(0, Vector3.right * 3f, 0.3f);
            }, 0.5f);
        }
        
        SetPickText($"Pick <color=#CDE7B0>{picks}</color> new cards...");
    }

    private void LateUpdate()
    {
        if (DevKey.Down(KeyCode.S))
        {
            ShowSkills();
        }
    }

    private List<CardData> GetFixedOptions()
    {
        var list = new List<CardData>();
        
        if (State.Instance.Has(Effect.Cheater))
        {
            list.Add(CardData.GetCheat());
        }
        
        if (State.Instance.Has(Effect.Modifiers))
        {
            list.Add(CardData.GetRandomModifier());
        }
        
        if (State.Instance.Has(Effect.Basics))
        {
            list.Add(CardData.GetBasic());
        }

        return list;
    }

    private void ShowCards(int amount)
    {
        if (picks <= 0) return;
        
        var options = new List<Card>();
        var fixedOptions = GetFixedOptions();
        for (var i = 0; i < amount; i++)
        {
            var option = Instantiate(cardPrefab, transform);
            option.transform.position = new Vector3(0, -5, 0);
            option.Detach();
            options.Add(option);
            cardOptions.Add(option);
            var data = fixedOptions.Any() ? fixedOptions.FirstOrDefault() : CardData.GetRandom();
            fixedOptions.Remove(data);
            option.Setup(data, deck);
            option.Nudge();
            option.Flip();

            option.picked += () => deckBorders.enabled = !option.IsModifier;
            option.dropped += () => deckBorders.enabled = false;

            option.click += () =>
            {
                if (hasEnded) return;
                
                deckBorders.enabled = false;
                    
                dragon.Nudge();
                
                if (option.IsModifier)
                {
                    dragon.Tutorial.Show(TutorialMessage.ModInfo);
                    var wasSelected = option.IsSelected;
                    DeselectAll();
                    if (wasSelected) return;
                    option.ChangeSelection(true);
                    modifier = option;
                    return;
                }

                if (modifier != default)
                {
                    modifier = default;
                    DeselectAll();
                    return;
                }

                cardOptions.Remove(option);
                option.Pop();
                hand.Remove(option);
                deck.AddCard(data);
                option.Kill();
                State.Instance.Add(data);
                MoveDeck();
                CheckEnd();
            };
        }

        hand.Add(options);
    }

    private void ShowSkills()
    {
        AudioManager.Instance.NudgePitch(0.5f, 0.4f);
        AudioManager.Instance.PlayEffectFromCollection(3, Vector3.zero, 0.7f);
        
        hand.Clear();
        var options = new List<Card>();
        
        dragon.Tutorial.Show(TutorialMessage.SkillPick);
        
        var skills = skillPool.Get(2 + State.Instance.GetCount(Effect.UkkoMine), State.Instance.Skills).ToList();
        skills.ForEach(skill =>
        {
            var option = Instantiate(cardPrefab, transform);
            option.transform.position = new Vector3(0, -5, 0);
            option.Detach();
            options.Add(option);
            option.Setup(skill, deck);
            option.Nudge();
            option.Flip();

            option.click += () =>
            {
                if (hasEnded) return;
                if (picked) return;
                
                AudioManager.Instance.PlayEffectFromCollection(3, Vector3.zero, 0.7f);
                
                dragon.Nudge();
                
                picked = true;
                State.Instance.Add(skill);

                if (skill.effect == Effect.UkkoTower)
                {
                    State.Instance.MaxStrikes++;
                    strikeDisplay.AddMax(1);
                }

                if (skill.effect == Effect.Phone)
                {
                    phoneButton.Show();
                }

                if (skill.effect == Effect.ScoreDoubler)
                {
                    var total = scoreDisplay.Total;
                    scoreDisplay.Add(total, false);
                    ShowScore(total, 1, Vector3.zero);
                }
                
                for (var i = 0; i < State.Instance.GetCount(Effect.Pestilence); i++)
                {
                    var data = new CardData(0) { type = CardType.Kill, icon = 6, playable = true };
                    State.Instance.Add(data);
                    deck.AddCard(data);
                }
                
                option.Pop();
                hand.Remove(option);
                option.Kill();
                MoveDeck();

                hand.Clear();
                pickBg.gameObject.SetActive(false);

                if (skill.effect == Effect.CardInstead)
                {
                    picks = 3 + State.Instance.GetCount(Effect.Greed);
                    ShowCards(5);
                    return;
                }
                
                NextLevel();

                if (skill.effect == Effect.Heal)
                {
                    strikeDisplay.AddStrikes(-1);
                }
                
                skillIcons.Add(skill);
            };
        });
        
        hand.Add(options);
    }

    public void ChangePage(int dir)
    {
        page = Mathf.Clamp(page + dir, 0, MaxPage - 1);
        MoveDeck();
    }

    private void MoveDeck()
    {
        var index = 0;
        var cards = deck.Cards.Where(c => !c.IsUsed).OrderByDescending(c => c.SortValue).ToList();
        cards.ForEach(c =>
        {
            c.ToggleTrail(false);
            var shown = index >= page * PageSize && index < (page + 1) * PageSize;
            c.Flip();
            c.Detach();
            c.Nudge();
            var x = index % PerRow;
            var y = Mathf.FloorToInt((index - 2 * page * PerRow) * 1f / PerRow);
            c.transform.position = shown ? 
                new Vector3((-(PerRow - 1) * 0.5f + x) * 1.2f, contentStart.transform.position.y - y * 1.5f * 1.2f, 0) : 
                Vector3.up * 999;
            index++;
        });
        
        pageText.text = $"{page + 1}/{MaxPage}";
        countText.text = $"{deck.Cards.ToList().Count} cards";
    }

    private void CheckEnd()
    {
        picks--;
        var term = picks == 1 ? "card" : "cards";
        SetPickText($"Pick <color=#CDE7B0>{picks}</color> more {term}...");
        
        if (picks == 0 || hand.IsEmpty)
        {
            cardOptions.ForEach(c => State.Instance.AddForOpponent(c.GetData()));
            SetPickText("Pick a <color=#CDE7B0>skill</color> addition...");

            if (picked)
            {
                hand.Clear();
                NextLevel();
                return;
            }
            ShowSkills();
        }
    }

    private void SetPickText(string text)
    {
        pickLabel.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(pickBg);
    }

    private void NextLevel()
    {
        State.Instance.Score = scoreDisplay.Total;
        this.StartCoroutine(() => State.Instance.NextLevel(), 0.5f);
    }

    public override void Select(Card card)
    {
        if (modifier != default)
        {
            modifier.ChangeSelection(false);
            Combine(modifier, card);
        }
        
        DeselectAll();
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        card.click?.Invoke();
    }

    protected override void Combine(Card first, Card second)
    {
        var data = State.Instance.GetCard(second.Id);
        if (data == default) return;
        if (second.NeedsFlattening && first.IsValueModifier)
        {
            data.type = CardType.Normal;
            data.number = second.Number;
            data.icon = -1;
        }

        if (first.IsDuplicator)
        {
            var dupe = new CardData(data) { id = Guid.NewGuid() };
            State.Instance.Add(dupe);
            deck.AddCard(dupe);
            MoveDeck();
        }
        second.Pop();
        data.Modify(first.GetData());
        second.Setup(data, deck);
        second.Flip();
        hand.Remove(first);
        first.Kill();
        CheckEnd();
    }

    public override bool CanCombine(Card first, Card second)
    {
        return first.IsModifier && State.Instance.Has(second.Id);
    }

    public override void RightClick(Card card)
    {
    }

    public override bool CanPlay(Card card)
    {
        return !State.Instance.Has(card.Id) && !card.IsModifier;
    }

    public override int AddStrikes()
    {
        return 0;
    }

    public override void PlayInstant(Card card)
    {
        if (!State.Instance.Has(card.Id)) return;

        var p = card.transform.position;
        
        card.IsUsed = true;
        card.Pop();

        if (card.Is(CardType.Recall) && picks > 0)
        {
            hand.Clear();
            ShowCards(3);
            ResetMulti();
        }
        
        PlayGenericInstant(card);

        if (card.Is(CardType.Averager) && picks > 0)
        {
            var sum = Math.Floor(hand.Cards.Average(c => c.Number));
            hand.Cards.ToList().ForEach(c => c.ChangeNumber((int)sum));
        }
        
        card.gameObject.SetActive(false);
        MoveDeck();
    }

    public override IReadOnlyCollection<Card> GetVisibleCards()
    {
        var list = deck.Cards.ToList();
        list.AddRange(hand.Cards);
        return list;
    }

    public override int GetHandSize()
    {
        return hand.Cards.ToList().Count;
    }

    public override int GetTrueJokerValue()
    {
        return 0;
    }

    public override void AddExtras(List<CardData> cards)
    {
        throw new NotImplementedException();
    }

    protected override void ReSelect()
    {
    }

    public void PhoneScore()
    {
        scoreDisplay.Add(1000 * State.Instance.LevelMulti);
        ShowScore(1000, State.Instance.LevelMulti, new Vector3(4, -2));
    }
}