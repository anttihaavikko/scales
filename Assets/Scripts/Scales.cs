using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

public class Scales : GameMode
{
    [SerializeField] private TMP_Text left, right, diff;
    [SerializeField] private Transform beam, leftBasket, rightBasket;
    [SerializeField] private GameObject leftMassVisuals, rightMassVisuals;
    [SerializeField] private TMP_Text leftMassAmount, rightMassAmount;

    private int leftMass = 1;
    private int rightMass = 1;
    private int trueJokerValue;

    public override void Setup()
    {
        this.StartCoroutine(() => hand.Fill(), 0.5f);
        SetupWeights();
        
        if (State.Instance.Has(Effect.ExtraSlot))
        {
            slots[2].gameObject.SetActive(true);
        }
    }

    private void SetupWeights()
    {
        var level = State.Instance.Level;
        if (level > 1) rightMass = Random.Range(1, 4);
        if (level > 5) leftMass = Random.Range(1, 4);

        if (rightMass > 1 || leftMass > 1)
        {
            dragon.Tutorial.Show(TutorialMessage.ExtraWeights);
        }

        leftMassAmount.text = $"x{leftMass}";
        rightMassAmount.text = $"x{rightMass}";
        
        leftMassVisuals.SetActive(leftMass > 1);
        rightMassVisuals.SetActive(rightMass > 1);
    }

    public override void Select(Card card)
    {
        if (hasEnded) return;
        var state = card.IsSelected;
        DeselectAll();
        card.ChangeSelection(state);
    }

    private void UpdateTrueJokerFor(Slot slot)
    {
        var isLeft = slot == slots[0];
        var leftSum = slots[0].Sum * leftMass;
        var rightSum = slots[1].Sum * rightMass;
        trueJokerValue = isLeft ? (rightSum - leftSum) / leftMass : (leftSum - rightSum) / rightMass;
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        if (hasEnded) return;
        
        UpdateTrueJokerFor(slot);
        card.transform.SetParent(null);

        card.SetDepth(slot.TopCard, 1);
        card.Lock();
        hand.Remove(card);
        card.ChangeSelection(false);
        slot.Add(card);
        AfterPlay(card);
        card.Flatten();
        var pos = slot.GetPosition();
        card.MoveTo(pos);
        
        card.transform.SetParent(slot.transform);

        var abs = UpdateScales();
        Score(card, slot, abs, pos);
        hand.Draw();

        EndCheck();
    }

    private int UpdateScales()
    {
        var difference = GetDifference();
        var abs = Mathf.Abs(difference);

        this.StartCoroutine(() =>
        {
            var angle = Mathf.Clamp(difference, -10f, 10f);
            Tweener.RotateToQuad(beam, Quaternion.Euler(new Vector3(0, 0, angle)), 0.5f);
            Tweener.RotateToQuad(leftBasket, Quaternion.Euler(new Vector3(0, 0, -angle)), 0.5f);
            Tweener.RotateToQuad(rightBasket, Quaternion.Euler(new Vector3(0, 0, -angle)), 0.5f);
        }, 0.2f);

        diff.text = abs.ToString();
        return abs;
    }

    private int GetDifference()
    {
        var leftSum = slots[0].Sum * leftMass;
        var rightSum = slots[1].Sum * rightMass;
        var difference = leftSum - rightSum;
        left.text = leftSum.ToString();
        right.text = rightSum.ToString();
        return difference;
    }

    private void EndCheck()
    {
        if (hasEnded) return;
        
        if (hand.IsEmpty)
        {
            if (GetDifference() != 0 && hasExtras)
            {
                Invoke(nameof(BecameStuck), 1f);
                Invoke(nameof(EndCheck), 2f);
                return;
            }
            
            continueButton.Show();
        }
    }

    private void Score(Card card, Slot slot, int difference, Vector3 pos)
    {
        var total = card.ScoreValue * State.Instance.LevelMulti;
        var multi = slot.Cards.ToList().Count;
        ShowScore(total, multi, pos);
        scoreDisplay.Add(total * multi);

        if (difference == 0)
        {
            scoreDisplay.AddMulti();
            dragon.Hop();
            return;
        }
        
        if(difference > 10)
        {
            dragon.Tutorial.Show(TutorialMessage.Overloaded);
            scoreDisplay.ResetMulti();
        }
        
        dragon.Nudge();
    }

    private void RoundEnded()
    {
        State.Instance.RoundEnded(scoreDisplay.Total);
    }

    protected override void Combine(Card first, Card second)
    {
    }

    public override bool CanCombine(Card first, Card second)
    {
        return false;
    }

    public override void RightClick(Card card)
    {
    }

    public override bool CanPlay(Card card)
    {
        return !hasEnded;
    }

    public override int AddStrikes()
    {
        var leftSum = slots[0].Sum * leftMass;
        var rightSum = slots[1].Sum * rightMass;
        var difference = leftSum - rightSum;
        if(difference == 0)
        {
            Perfect();
        }
        var total = Mathf.Min(State.Instance.Level, Mathf.Abs(difference));
        strikeDisplay.AddStrikes(total);
        return total;
    }

    public override void PlayInstant(Card card)
    {
        if (hasEnded) return;
        
        var p = card.transform.position;
        
        card.Pop();
        hand.Remove(card);
        
        if (card.Is(CardType.Recall))
        {
            scoreDisplay.ResetMulti();
            hand.Draw();
            hand.Draw();
        }
        
        PlayGenericInstant(card);
        
        if (card.Is(CardType.Averager))
        {
            var list = GetVisibleCards().ToList();
            var sum = list.Average(c => c.Number);
            list.ForEach(c => c.ChangeNumber((int)sum));
        }
        
        hand.Draw();
        card.gameObject.SetActive(false);
        UpdateScales();
        EndCheck();
    }

    public override IReadOnlyCollection<Card> GetVisibleCards()
    {
        var list = hand.Cards.ToList();
        list.AddRange(slots.Select(s => s.TopCard));
        return list.Where(c => c).ToList();
    }

    public override int GetHandSize()
    {
        return hand.Cards.ToList().Count;
    }

    public override int GetTrueJokerValue()
    {
        return trueJokerValue;
    }

    public override void AddExtras(List<CardData> cards)
    {
        hand.Add(cards.Select(c =>
        {
            var card = deck.Create(c);
            card.Flip();
            return card;
        }).ToList());
    }

    protected override void ReSelect()
    {
    }
}