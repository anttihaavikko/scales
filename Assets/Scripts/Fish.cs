using System.Collections.Generic;
using System.Data;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Fish : GameMode
{
    [SerializeField] private List<FishLane> lanes;
    
    private readonly List<Card> selected = new();
    private Card root;
    
    public override void Setup()
    {
        Fill();
        slots.ForEach(slot => slot.click += ClickSlot);

        if (State.Instance.Has(Effect.ExtraSlot))
        {
            slots[0].gameObject.SetActive(true);
        }
        
        TriggerSelect();
    }

    private void ClickSlot(Slot slot)
    {
        if (hasEnded) return;
        
        if (selected.Any())
        {
            DropToSlot(selected.First(), slot);
        }
    }

    private void Fill()
    {
        lanes.ForEach(l => l.Fill(deck));
        if(lanes.Any(l => !l.IsFull) && !deck.IsEmpty)
        {
            Invoke(nameof(Fill), 0.6f);
            return;
        }
        
        Invoke(nameof(Evaluate), 1f);
    }

    private void Evaluate()
    {
        if (hasEnded) return;
        
        var all = lanes.SelectMany(l => l.Cards).Where(c => c.IsOpen && !c.IsUsed).ToList();
        var available = all.Any(c => Check(c, all));

        if (!available)
        {
            if (lanes.Any(lane => lane.Cards.Any()) && hasExtras)
            {
                Invoke(nameof(BecameStuck), 1f);
            }
            
            continueButton.Show();
        }
    }

    private bool Check(Card card, List<Card> all)
    {
        var numbers = all.Where(c => c != card && AreNeighbours(card, c)).Select(c => c.Number).ToList();
        return CanAddTo(numbers, card.Number) || CanSubTo(numbers, card.Number);
    }

    protected override void Combine(Card first, Card second)
    {
    }

    private bool AreNeighbours(Card first, Card second)
    {
        return Vector3.Distance(first.transform.position, second.transform.position) < 2f;
    } 

    public override void Select(Card card)
    {
        if (hasEnded) return;
        
        if (!card.IsSelected && !selected.Contains(card)) return;

        if (root && !AreNeighbours(root, card))
        {
            ClearSelection();
        }
        
        if (selected.Contains(card))
        {
            if (card == root) ClearSelection();
            selected.Remove(card);
            ReSelect();
            return;
        }
        
        card.ChangeSelection(true, selected.Any());
        
        if (!selected.Any() && card.IsSelected)
        {
            root = card;
        }
        selected.Add(card);

        ReSelect();
    }

    protected override void ReSelect()
    {
        if (!root) return;

        var others = selected.Where(c => c != root).ToList();
        var target = root.Number;
        var numbers = others.Select(c => c.Number).ToList();

        if (selected.Any(c => c.IsTrueJoker) || CanAddTo(numbers, target, true) || CanSubTo(numbers, target, true))
        {
            Score(selected);
            lanes.ForEach(l => l.Remove(selected));
            selected.ForEach(c =>
            {
                c.Pop();
                c.Kill();
            });
            selected.Clear();
            root = null;

            DropAndFill();
        }
    }

    private void DropAndFill()
    {
        this.StartCoroutine(() => lanes.ForEach(l => l.Drop()), 0.3f);
        Invoke(nameof(Fill), 1f);
    }

    private void Score(ICollection<Card> cards)
    {
        cards.ToList().ForEach(AfterPlay);
        
        dragon.Acknowledge(cards.Count > 3);
        var total = cards.Sum(c => c.ScoreValue) * State.Instance.LevelMulti;
        var x = cards.Average(c => c.transform.position.x);
        var y = cards.Average(c => c.transform.position.y);
        var p = new Vector3(x, y);
        ShowScore(total, cards.Count, p);
        scoreDisplay.Add(cards.Count * total);
        scoreDisplay.AddMulti();
        
        Shake(0.15f * cards.Count);
    }

    private void ClearSelection()
    {
        ResetMulti();
        selected.Clear();
        lanes.ForEach(l => l.Deselect());
        root = null;
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        if (hasEnded) return;
        
        card.ChangeSelection(false);
        lanes.ForEach(l => l.Remove(new List<Card>{ card }));
        ClearSelection();
        DropAndFill();
        slot.Add(card);
        card.MoveTo(slot.GetPosition());
        card.Lift();
        card.IncreaseNumber();
    }

    public override bool CanCombine(Card first, Card second)
    {
        return false;
    }

    public override void RightClick(Card card)
    {
        // lanes.ForEach(l => l.Deselect());
    }

    public override bool CanPlay(Card card)
    {
        return slots[0].IsEmpty && !hasEnded;
    }

    public override int AddStrikes()
    {
        var deckCards = deck.Cards.Count(c => !c.IsRemoved);
        var laneCards = lanes.SelectMany(l => l.Cards).ToList();
        var laneTopCards = laneCards.Count(c => !c.IsOpen);
        var total = deckCards + laneTopCards;
        if(!laneCards.Any()) Perfect();
        strikeDisplay.AddStrikes(total);
        return total;
    }

    public override void PlayInstant(Card card)
    {
        if (hasEnded) return;
        
        var p = card.transform.position;
        card.IsUsed = true;
        card.Pop();
        card.Kill();
        lanes.ForEach(l => l.Remove(new List<Card>{ card }));
        
        if (card.Is(CardType.Recall))
        {
            ResetMulti();
        }

        PlayGenericInstant(card);
        
        if (card.Is(CardType.Averager))
        {
            var list = GetVisibleCards().ToList();
            var sum = list.Average(c => c.Number);
            list.ForEach(c => c.ChangeNumber((int)sum));
        }
        
        DropAndFill();
    }

    public override IReadOnlyCollection<Card> GetVisibleCards()
    {
        return lanes.SelectMany(l => l.Cards).Where(c => !c.IsRemoved && c.IsOpen).ToList();
    }

    public override int GetHandSize()
    {
        return 0;
    }

    public override int GetTrueJokerValue()
    {
        return selected.Where(c => !c.IsTrueJoker && !c.IsJoker).Sum(c => c.Number);
    }

    public override void AddExtras(List<CardData> cards)
    {
        cards.ForEach(c => deck.AddCard(c));
        Fill();
    }
}