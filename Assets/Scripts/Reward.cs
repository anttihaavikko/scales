using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class Reward : GameMode
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private TMP_Text pageText, countText;
    [SerializeField] private Transform contentStart;

    private Card modifier;
    private int picks = 3;
    private bool picked;
    private int page;
    
    private const int PerRow = 12;
    private const int PageSize = PerRow * 2;

    private int MaxPage => Mathf.CeilToInt(deck.Cards.Count(c => !c.IsUsed) * 1f / PageSize);

    public override void Setup()
    {
        MoveDeck();
        picks = 3 + State.Instance.GetCount(Effect.Greed);
        ShowCards(5 + State.Instance.GetCount(Effect.MoreOptions));
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
            list.Add(new CardData(Random.Range(1, 11)));
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
            option.Detach();
            options.Add(option);
            var data = fixedOptions.Any() ? fixedOptions.FirstOrDefault() : CardData.GetRandom();
            fixedOptions.Remove(data);
            option.Setup(data, deck);
            option.Nudge();
            option.Flip();

            option.click += () =>
            {
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
        hand.Clear();
        var options = new List<Card>();
        
        dragon.Tutorial.Show(TutorialMessage.SkillPick);
        
        var skills = skillPool.Get(2, State.Instance.Skills).ToList();
        skills.ForEach(skill =>
        {
            var option = Instantiate(cardPrefab, transform);
            option.Detach();
            options.Add(option);
            option.Setup(skill, deck);
            option.Nudge();
            option.Flip();

            option.click += () =>
            {
                if (picked) return;
                picked = true;
                State.Instance.Add(skill);
                option.Pop();
                hand.Remove(option);
                option.Kill();
                MoveDeck();

                hand.Clear();

                if (skill.effect == Effect.CardInstead)
                {
                    picks = 3 + State.Instance.GetCount(Effect.Greed);
                    ShowCards(5);
                    return;
                }
                
                this.StartCoroutine(() => State.Instance.NextLevel(), 0.5f);

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
        
        if (picks == 0)
        {
            if (picked)
            {
                hand.Clear();
                this.StartCoroutine(() => State.Instance.NextLevel(), 0.5f);
                return;
            }
            ShowSkills();
        }
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
        return true;
    }

    public override int AddStrikes()
    {
        return 0;
    }

    public override void PlayInstant(Card card)
    {
        if (!State.Instance.Has(card.Id)) return;
        
        card.IsUsed = true;
        card.Pop();

        if (card.Is(CardType.Recall) && picks > 0)
        {
            hand.Clear();
            ShowCards(3);
            scoreDisplay.ResetMulti();
        }

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
}