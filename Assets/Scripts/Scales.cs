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

    public override void Setup()
    {
        this.StartCoroutine(() => hand.Fill(), 0.5f);
        SetupWeights();
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
        var state = card.IsSelected;
        DeselectAll();
        card.ChangeSelection(state);
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        card.transform.SetParent(null);
        card.Flatten();
        
        card.SetDepth(slot.TopCard, 1);
        card.Lock();
        hand.Remove(card);
        card.ChangeSelection(false);
        slot.Add(card);
        card.Flatten();
        var pos = slot.GetPosition();
        Tweener.MoveToBounceOut(card.transform, pos, 0.1f);
        
        card.transform.SetParent(slot.transform);

        var abs = UpdateScales();
        Score(card, slot, abs, pos);
        hand.Draw();
        
        AfterPlay(card);

        EndCheck();
    }

    private int UpdateScales()
    {
        var leftSum = slots[0].Sum * leftMass;
        var rightSum = slots[1].Sum * rightMass;
        var difference = leftSum - rightSum;
        var abs = Mathf.Abs(difference);

        this.StartCoroutine(() =>
        {
            var angle = Mathf.Clamp(difference, -10f, 10f);
            Tweener.RotateToQuad(beam, Quaternion.Euler(new Vector3(0, 0, angle)), 0.5f);
            Tweener.RotateToQuad(leftBasket, Quaternion.Euler(new Vector3(0, 0, -angle)), 0.5f);
            Tweener.RotateToQuad(rightBasket, Quaternion.Euler(new Vector3(0, 0, -angle)), 0.5f);
        }, 0.2f);

        left.text = leftSum.ToString();
        right.text = rightSum.ToString();

        diff.text = abs.ToString();
        return abs;
    }

    private void EndCheck()
    {
        if (hand.IsEmpty)
        {
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
        return true;
    }

    public override int AddStrikes()
    {
        var leftSum = slots[0].Sum * leftMass;
        var rightSum = slots[1].Sum * rightMass;
        var difference = leftSum - rightSum;
        var total = Mathf.Abs(difference);
        strikeDisplay.AddStrikes(total);
        return total;
    }

    public override void PlayInstant(Card card)
    {
        card.Pop();
        hand.Remove(card);
        
        if (card.Is(CardType.Recall))
        {
            scoreDisplay.ResetMulti();
            hand.Draw();
            hand.Draw();
        }
        
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
}