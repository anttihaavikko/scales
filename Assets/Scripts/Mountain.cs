using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mountain : GameMode
{
    [SerializeField] private TMP_Text helpText;
    [SerializeField] private Transform bg;

    private MountainOperator operation;
    private int target;

    private const float Overlap = 0.75f;
    private const float SlotOffset = 0.75f;

    public override void Setup()
    {
        SetupLevel();
        SetupSlots();

        var cards = deck.Cards.Reverse().ToList();
        
        cards.ForEach(c => c.Nudge());

        var slotCount = slots.Count(s => s.gameObject.activeSelf);
        var rows = GetRowCount(cards.Count - slotCount);
        var top = (rows - 1) * 0.5f;

        var scale = Mathf.Max(1f, rows / 4f * Overlap);
        bg.localScale = new Vector3(operation == MountainOperator.Plus ? 1 : -1, 1, 1) * scale;
        cam.orthographicSize *= scale;

        var index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < row + 1; col++)
            {
                var y = (top - row) * Overlap;
                cards[index].transform.position = new Vector3(-row * 0.6f + col * 1.2f, y);
                cards[index].SetDepth();

                ApplyCover(cards, cards[index], index - row - 1, row);
                ApplyCover(cards, cards[index], index - row, row);

                index++;

                if (index >= cards.Count) break;
            }
        }

        slots[0].transform.position = new Vector3(-(rows * 0.5f + 1) * 1.2f, -top * Overlap - SlotOffset, 0.1f);
        slots[1].transform.position = new Vector3((rows * 0.5f + 1) * 1.2f, -top * Overlap - SlotOffset, 0.1f);
        slots[2].transform.position = new Vector3(-(rows * 0.5f + 2.5f) * 1.2f, -top * Overlap - SlotOffset, 0.1f);
        slots[3].transform.position = new Vector3((rows * 0.5f + 2.5f) * 1.2f, -top * Overlap - SlotOffset, 0.1f);
        slots[4].transform.position = new Vector3(0, -top - 1f * Overlap - SlotOffset - 0.15f, 0.1f);

        helpText.transform.position = slots[4].gameObject.activeSelf ? 
            new Vector3(0, -top - 1f * Overlap - SlotOffset - 1.75f, 0) : 
            new Vector3(0, -5f * scale, 0);

        cards.Skip(index).ToList().ForEach(c =>
        {
            var slot = slots.FirstOrDefault(s => s.IsEmpty && s.gameObject.activeSelf);

            if (slot)
            {
                slot.Add(c, false);
                c.transform.position = slot.transform.position.WhereZ(0);
                c.SetDepth();
            }
        });

        FlipCards();
    }

    private static int GetRowCount(int count)
    {
        var rows = Mathf.CeilToInt(0.5f * (-1 + Mathf.Sqrt(8 * count + 1)));
        return rows;
    }

    private void SetupSlots()
    {
        var level = State.Instance.Level;
        if(level == 0)
        {
            slots[4].gameObject.SetActive(false);
            return;
        }

        if (level < 5)
        {
            slots[2].gameObject.SetActive(false);
            slots[3].gameObject.SetActive(false);
            return;
        }

        if (level >= 9)
        {
            slots[0].gameObject.SetActive(false);
            slots[1].gameObject.SetActive(false);
            slots[2].gameObject.SetActive(false);
            slots[3].gameObject.SetActive(false);
            return;
        }
        
        slots[2].gameObject.SetActive(false);
        slots[3].gameObject.SetActive(false);
        slots[4].gameObject.SetActive(false);
    }

    private void SetupLevel()
    {
        var level = State.Instance.Level;

        if (level == 0)
        {
            SetLevel(new MountainType(MountainOperator.Plus, 10));
            return;
        }

        if (level < 5)
        {
            SetLevel(new[]
            {
                new MountainType(MountainOperator.Plus, Random.Range(9, 13)),
                new MountainType(MountainOperator.Minus, Random.Range(0, 3))
            }.Random());
            return;
        }
        
        SetLevel(new[]
        {
            new MountainType(MountainOperator.Plus, Random.Range(9, 26)),
            new MountainType(MountainOperator.Minus, Random.Range(0, 6))
        }.Random());
    }

    private void SetLevel(MountainType type)
    {
        operation = type.operation;
        target = type.target;
        helpText.text = $"Select cards that <color=#CDE7B0>{GetOperation()}</color> to <color=#CDE7B0>{target}</color>...";
    }

    private string GetOperation()
    {
        return operation switch
        {
            MountainOperator.Plus => "add up",
            MountainOperator.Minus => "subtract",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        scoreDisplay.ResetMulti();
        DeselectAll();
        slots.ForEach(s => s.Remove(card));
        card.ChangeSelection(false);
        slot.Add(card);
        card.MoveTo(slot.GetPosition());
        card.Lift();
        deck.Cards.ToList().ForEach(c => c.RemoveCover(card));
        FlipCards();
    }

    private bool Operate(int a, int b)
    {
        return operation switch
        {
            MountainOperator.Plus => a + b == target,
            MountainOperator.Minus => a - b == target || b - a == target,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public override bool CanCombine(Card first, Card second)
    {
        return Operate(first.Number, second.Number);
    }

    public override int GetJokerValue()
    {
        return deck.Cards.Where(c => !c.IsJoker && c.IsOpen).Sum(c => c.Number);
    }

    protected override void Combine(Card first, Card second)
    {
        var both = new List<Card> { first, second };
        Score(both);
        deck.Kill(both);
        first.Pop();
        second.Pop();
        slots.ForEach(s =>
        {
            s.Remove(first);
            s.Remove(second);
        });
        FlipCards();
    }

    public override void RightClick(Card card)
    {
        var cp = card.transform.position;
        DeselectAll();
        var slot = slots
            .Where(s => s.IsEmpty && s.gameObject.activeSelf)
            .OrderBy(s => Vector3.Distance(s.transform.position, cp))
            .FirstOrDefault();
        if (slot)
        {
            EffectManager.AddEffect(2, cp);
            DropToSlot(card, slot);
        }
    }

    public override bool CanPlay(Card card)
    {
        return true;
    }

    public override int AddStrikes()
    {
        var total = GetRowCount(deck.Cards.Count(c => c.IsCovered));
        strikeDisplay.AddStrikes(total);
        return total;
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
        var selected = deck.Cards.Where(c => c.IsSelected && !c.IsRemoved).ToList();
        var numbers = selected.Select(c => c.Number).ToList();
        var sum = numbers.Sum();
        
        if (CanCalcTo(numbers, target, true))
        {
            selected.ForEach(c => c.Pop());
            Score(selected);
            deck.Kill(selected);
        }

        if (sum > target && selected.Count > 1 && operation == MountainOperator.Plus)
        {
            selected.ForEach(c => c.ChangeSelection(false));
            card.ChangeSelection(true);
            Select(card);
            return;
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
        var noOpenSlots = slots.All(s => !s.IsEmpty || !s.gameObject.activeSelf);

        var canCalc = CanCalcTo(numbers, target);
        // Debug.Log($"Calc: {string.Join(",", numbers)} => {canCalc}, all: {allOpen}, slots used: {noOpenSlots}");
        
        if ((allOpen || noOpenSlots) && !canCalc)
        {
            if (numbers.Count == 0)
            {
                dragon.Hop();
            }
            else
            {
                dragon.Sit();
            }
            
            Invoke(nameof(RoundEnded), 0.7f);
        }
    }

    private void RoundEnded()
    {
        continueButton.Show();
    }

    private bool CanCalcTo(IList<int> set, int sum, bool exact = false)
    {
        return operation == MountainOperator.Plus ? 
            CanAddTo(set, sum, exact) : 
            CanSubTo(set, sum, exact);
    }

    private void Score(ICollection<Card> cards)
    {
        dragon.Acknowledge(cards.Count > 2);
        var total = cards.Sum(c => c.ScoreValue) * State.Instance.LevelMulti;
        var x = cards.Average(c => c.transform.position.x);
        var y = cards.Average(c => c.transform.position.y);
        var p = new Vector3(x, y);
        ShowScore(total, cards.Count, p);
        scoreDisplay.Add(cards.Count * total);
        scoreDisplay.AddMulti();

        if (cards.Count > 2)
        {
            this.StartCoroutine(() => dragon.Tutorial.Show(TutorialMessage.BigScore), 1f);
        }
    }
}

public struct MountainType
{
    public MountainOperator operation;
    public int target;

    public MountainType(MountainOperator operation, int target)
    {
        this.target = target;
        this.operation = operation;
    }
}

public enum MountainOperator
{
    Plus,
    Minus
}