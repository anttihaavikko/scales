using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Mountain : GameMode
{
    public override void Setup()
    {
        var cards = deck.Cards.Reverse().ToList();
        
        var rows = Mathf.CeilToInt(0.5f * (-1 + Mathf.Sqrt(8 * (cards.Count - slots.Count) + 1)));
        var top = (rows - 1) * 0.5f;

        var index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < row + 1; col++)
            {
                var y = (top - row) * 0.9f;
                cards[index].transform.position = new Vector3(-row * 0.6f + col * 1.2f, y);
                cards[index].SetDepth();

                ApplyCover(cards, cards[index], index - row - 1, row);
                ApplyCover(cards, cards[index], index - row, row);
                
                index++;

                if (index >= cards.Count) break;
            }
        }

        slots[0].transform.position = new Vector3(-(rows * 0.5f + 1) * 1.2f, -top * 0.9f, 0.1f);
        slots[1].transform.position = new Vector3((rows * 0.5f + 1) * 1.2f, -top * 0.9f, 0.1f);
        slots[2].transform.position = new Vector3(-(rows * 0.5f + 2.5f) * 1.2f, -top * 0.9f, 0.1f);
        slots[3].transform.position = new Vector3((rows * 0.5f + 2.5f) * 1.2f, -top * 0.9f, 0.1f);

        cards.Skip(index).ToList().ForEach(c =>
        {
            var slot = slots.FirstOrDefault(s => s.IsEmpty);

            if (slot)
            {
                slot.Add(c);
                c.transform.position = slot.transform.position.WhereZ(0);
                c.SetDepth();
            }
        });
        
        FlipCards();
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        DeselectAll();
        slots.ForEach(s => s.Remove(card));
        card.ChangeSelection(false);
        slot.Add(card);
        card.MoveTo(slot.GetPosition());
        card.Lift();
        deck.Cards.ToList().ForEach(c => c.RemoveCover(card));
        FlipCards();
    }
    
    public override bool CanCombine(Card first, Card second)
    {
        return first.Number + second.Number == 10;
    }

    public override int GetJokerValue()
    {
        return deck.Cards.Where(c => !c.IsJoker && c.IsOpen).Sum(c => c.Number);
    }

    protected override void Combine(Card first, Card second)
    {
        deck.Kill(new List<Card>{ first, second });
        slots.ForEach(s =>
        {
            s.Remove(first);
            s.Remove(second);
        });
        FlipCards();
    }

    public override void RightClick(Card card)
    {
        DeselectAll();
        var slot = slots.FirstOrDefault(s => s.IsEmpty);
        if (slot)
        {
            DropToSlot(card, slot);
        }
    }

    private void ApplyCover(IReadOnlyList<Card> list, Card cur, int index, int row)
    {
        if (index < 0) return;
        
        var r = GetRow(index);
        if (r == row - 1)
        {
            list[index].AddCover(cur);
        }
    }

    private static int GetRow(int index)
    {
        return Mathf.FloorToInt((-1 + Mathf.Sqrt(1 + 8 * index)) / 2);
    }
    
    public override void Select(Card card)
    {
        var selected = deck.Cards.Where(c => c.IsSelected).ToList();
        var sum = selected.Sum(c => c.Number);
        if (sum == 10)
        {
            deck.Kill(selected);
        }

        if (sum > 10)
        {
            selected.ForEach(c => c.ChangeSelection(false));
            card.ChangeSelection(true);
        }
        
        FlipCards();
    }

    private void FlipCards()
    {
        deck.Cards.ToList().ForEach(c =>
        {
            if (!c.IsCovered)
            {
                c.Flip();
            }
        });

        var numbers = deck.Cards.Where(c => !c.IsCovered).Select(c => c.Number).ToList();
        var allOpen = deck.Cards.All(c => !c.IsCovered);
        var noOpenSlots = slots.All(s => !s.IsEmpty);

        if ((allOpen || noOpenSlots) && !CanSumTo(numbers, 10))
        {
            Invoke(nameof(RoundEnded), 0.5f);
        }
    }

    private void RoundEnded()
    {
        SceneChanger.Instance.ChangeScene("Reward");
    }
    
    private static bool CanSumTo(List<int> set, int sum)
    {
        return set.Any(num =>
        {
            var left = sum - num;
            if (left == 0)
            {
                return true;
            }

            var index = set.IndexOf(num);
            var possible = set.Where((n, i) => n <= sum && i != index).ToList();
            return possible.Any() && CanSumTo(possible, left);
        });
    }
}