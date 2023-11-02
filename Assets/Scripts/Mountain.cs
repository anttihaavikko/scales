using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Mountain : GameMode
{
    [SerializeField] private Deck deck;
    [SerializeField] private List<Slot> slots;

    private void Start()
    {
        slots.ForEach(slot => slot.click += SlotClicked);
    }
    public override void Setup()
    {
        var shuffled = deck.Cards.RandomOrder().ToList();

        var rows = 4;
        var index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < row + 1; col++)
            {
                if(row == 3) shuffled[index].Flip();
                shuffled[index].transform.position = new Vector3(-row * 0.6f + col * 1.2f, rows - row * 0.9f);
                shuffled[index].SetDepth();

                ApplyCover(shuffled, shuffled[index], index - row - 1, row);
                ApplyCover(shuffled, shuffled[index], index - row, row);
                
                index++;
            }
        }
        
        shuffled.Skip(index).ToList().ForEach(c =>
        {
            var slot = slots.FirstOrDefault(s => s.IsEmpty);

            if (slot)
            {
                slot.Add(c);
                c.transform.position = slot.transform.position.WhereZ(0);
                c.Flip();
            }
        });
    }
    
    private void SlotClicked(Slot slot)
    {
        var card = deck.Cards.FirstOrDefault(c => c.IsSelected);
        slots.ForEach(s => s.Remove(card));
        
        if (slot.IsEmpty && card)
        {
            DropToSlot(card, slot);
        }
    }
    
    public override void DropToSlot(Card card, Slot slot)
    {
        card.ChangeSelection(false);
        slot.Add(card);
        Tweener.MoveToBounceOut(card.transform, slot.transform.position.WhereZ(0), 0.1f);
        deck.Cards.ToList().ForEach(c => c.RemoveCover(card));
        FlipCards();
    }

    public override bool TryCombine(Card first, Card second)
    {
        var ok = first.Number + second.Number == 10;
        if (ok)
        {
            deck.Kill(new List<Card>{ first, second });
            FlipCards();
        }
        return ok;
    }

    public override void RightClick(Card card)
    {
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
    }
}