using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

public class Scales : GameMode
{
    [SerializeField] private TMP_Text left, right, diff;
    
    public override void Setup()
    {
        hand.Draw();
        hand.Draw();
        hand.Draw();
    }

    public override void Select(Card card)
    {
        var state = card.IsSelected;
        DeselectAll();
        card.ChangeSelection(state);
    }

    public override void DropToSlot(Card card, Slot slot)
    {
        card.SetDepth(slot.TopCard, 1);
        card.Lock();
        card.DisableCollider();
        hand.Remove(card);
        card.ChangeSelection(false);
        slot.Add(card);
        var pos = slot.transform.position.WhereZ(0) + Vector3.up * (0.2f * (slot.Count - 1));
        Tweener.MoveToBounceOut(card.transform, pos, 0.1f);
        hand.Draw();

        var leftSum = slots[0].Sum;
        var rightSum = slots[1].Sum;

        left.text = leftSum.ToString();
        right.text = rightSum.ToString();

        diff.text = Mathf.Abs(leftSum - rightSum).ToString();
    }

    public override bool TryCombine(Card first, Card second)
    {
        return false;
    }

    public override bool CanCombine(Card first, Card second)
    {
        return false;
    }

    public override void RightClick(Card card)
    {
    }
}