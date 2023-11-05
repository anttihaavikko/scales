using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reward : GameMode
{
    [SerializeField] private Card cardPrefab;

    private Card modifier;
    private int picks = 3;

    public override void Setup()
    {
        MoveDeck();

        var optionCount = 5;
        var options = new List<Card>();
        for (var i = 0; i < optionCount; i++)
        {
            var option = Instantiate(cardPrefab, transform);
            option.Detach();
            options.Add(option);
            var data = CardData.GetRandom();
            option.Setup(data, deck);
            option.transform.position += Vector3.right * 1.2f * (i - (optionCount - 1) * 0.5f);
            option.Nudge();
            option.Flip();

            option.click += () =>
            {
                if (option.IsModifier)
                {
                    var wasSelected = option.IsSelected;
                    DeselectAll();
                    if (wasSelected) return;
                    option.ChangeSelection(true);
                    modifier = option;
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

    private void MoveDeck()
    {
        var index = 0;
        var cards = deck.Cards.OrderByDescending(c => c.SortValue).ToList();
        cards.ForEach(c =>
        {
            c.Flip();
            c.Detach();
            c.Nudge();
            var perRow = 12;
            var x = index % perRow;
            var y = Mathf.FloorToInt(index * 1f / perRow);
            c.transform.position = new Vector3((-(perRow - 1) * 0.5f + x) * 1.2f, (1.25f - y) * 1.5f * 1.2f, 0);
            index++;
        });
    }

    private void CheckEnd()
    {
        picks--;
        
        if (picks == 0)
        {
            State.Instance.NextLevel();
        }
    }

    public override void Select(Card card)
    {
        if (modifier != default)
        {
            modifier.ChangeSelection(false);
            Combine(modifier, card);
        }
        
        modifier = default;
        DeselectAll();
    }

    public override void DropToSlot(Card card, Slot slot)
    {
    }

    protected override void Combine(Card first, Card second)
    {
        var data = State.Instance.GetCard(second.Id);
        if (data == default) return;
        if (data.type == CardType.Joker && first.IsValueModifier)
        {
            data.type = CardType.Normal;
            data.number = second.Number;
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
    
    public override int GetJokerValue()
    {
        return deck.Cards.Where(c => !c.IsJoker && c.IsOpen).Sum(c => c.Number);
    }
}