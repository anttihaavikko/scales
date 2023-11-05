using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
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
        var rows = Mathf.CeilToInt(0.5f * (-1 + Mathf.Sqrt(8 * (cards.Count - slotCount) + 1)));
        var top = (rows - 1) * 0.5f;

        var scale = Mathf.Max(1f, rows / 4f * Overlap);
        bg.localScale *= scale;
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
        
        if (slots[4].gameObject.activeSelf)
        {
            helpText.transform.position = new Vector3(0, -top - 1f * Overlap - SlotOffset - 2f, 0);
        }

        cards.Skip(index).ToList().ForEach(c =>
        {
            var slot = slots.FirstOrDefault(s => s.IsEmpty && s.gameObject.activeSelf);

            if (slot)
            {
                slot.Add(c);
                c.transform.position = slot.transform.position.WhereZ(0);
                c.SetDepth();
            }
        });

        FlipCards();
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
        helpText.text = $"Select cards that {GetOperation()} to {target}...";
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
        var slot = slots.FirstOrDefault(s => s.IsEmpty && s.gameObject.activeSelf);
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
        var numbers = selected.Select(c => c.Number).ToList();
        var sum = numbers.Sum();
        
        if (CanCalcTo(numbers, target, true))
        {
            deck.Kill(selected);
        }

        if (sum > target && operation == MountainOperator.Plus)
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

        if ((allOpen || noOpenSlots) && !CanCalcTo(numbers, target))
        {
            Invoke(nameof(RoundEnded), 0.5f);
        }
    }

    private void RoundEnded()
    {
        SceneChanger.Instance.ChangeScene("Reward");
    }

    private bool CanCalcTo(IList<int> set, int sum, bool exact = false)
    {
        return operation == MountainOperator.Plus ? 
            CanAddTo(set, sum, exact) : 
            CanSubTo(set, sum, exact);
    }
    
    private bool CanSubTo(IList<int> set, int sum, bool exact = false)
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

    private static bool CanAddTo(IList<int> set, int sum, bool exact = false)
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