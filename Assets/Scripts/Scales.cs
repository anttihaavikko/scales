using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

public class Scales : GameMode
{
    [SerializeField] private TMP_Text left, right, diff;
    
    public override void Setup()
    {
        this.StartCoroutine(() => hand.Fill(), 0.5f);
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

        var leftSum = slots[0].Sum;
        var rightSum = slots[1].Sum;

        hand.Draw();

        left.text = leftSum.ToString();
        right.text = rightSum.ToString();

        diff.text = Mathf.Abs(leftSum - rightSum).ToString();

        if (hand.IsEmpty)
        {
            SceneChanger.Instance.ChangeScene("Reward");
        }
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