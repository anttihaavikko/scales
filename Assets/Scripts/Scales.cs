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
        var pos = slot.GetPosition();
        Tweener.MoveToBounceOut(card.transform, pos, 0.1f);

        var leftSum = slots[0].Sum * leftMass;
        var rightSum = slots[1].Sum * rightMass;
        var difference = leftSum - rightSum;
        var abs = Mathf.Abs(difference);
        
        Score(card, slot, abs, pos);

        card.transform.SetParent(slot.transform);

        this.StartCoroutine(() =>
        {
            var angle = Mathf.Clamp(difference, -10f, 10f);
            Tweener.RotateToQuad(beam, Quaternion.Euler(new Vector3(0, 0, angle)), 0.5f);
            Tweener.RotateToQuad(leftBasket, Quaternion.Euler(new Vector3(0, 0, -angle)), 0.5f);
            Tweener.RotateToQuad(rightBasket, Quaternion.Euler(new Vector3(0, 0, -angle)), 0.5f);
        }, 0.2f);

        hand.Draw();

        left.text = leftSum.ToString();
        right.text = rightSum.ToString();

        diff.text = abs.ToString();

        if (hand.IsEmpty)
        {
            Invoke(nameof(RoundEnded), 1f);
        }
    }

    private void Score(Card card, Slot slot, int difference, Vector3 pos)
    {
        var total = card.ScoreValue * State.Instance.LevelMulti;
        var multi = slot.Cards.ToList().Count;
        ShowScore(total, multi, pos);
        scoreDisplay.Add(total * multi);

        if (difference == 0) scoreDisplay.AddMulti();
        if(difference > 10) scoreDisplay.ResetMulti();
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
    
    public override int GetJokerValue()
    {
        return slots.Sum(s => s.TopCard && !s.TopCard.IsJoker ? s.TopCard.Number : 0) + hand.Cards.Where(c => !c.IsJoker).Sum(c => c.Number);
    }
}