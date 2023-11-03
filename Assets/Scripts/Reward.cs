using System.Linq;
using UnityEngine;

public class Reward : GameMode
{
    [SerializeField] private Card cardPrefab;

    private Card modifier;
    private int picks = 3;

    public override void Setup()
    {
        var index = 0;
        var cards = deck.Cards.OrderByDescending(c => c.Number).ToList();
        cards.ForEach(c =>
        {
            c.Flip();
            var perRow = 8;
            var x = index % perRow;
            var y = Mathf.FloorToInt(index * 1f / perRow);
            c.transform.position = new Vector3((-(perRow - 1) * 0.5f + x) * 1.2f, (2 - y) * 1.5f * 1.2f, 0);
            index++;
        });

        var optionCount = 5;
        for (var i = 0; i < optionCount; i++)
        {
            var option = Instantiate(cardPrefab, transform);
            var data = CardData.GetRandom();
            option.Setup(data, deck);
            option.transform.position += Vector3.right * 1.2f * (i - (optionCount - 1) * 0.5f);
            option.Flip();

            option.click += () =>
            {
                if (option.IsModifier)
                {
                    DeselectAll();
                    modifier = option;
                    return;
                }
                
                option.Kill();
                State.Instance.Add(data);
                CheckEnd();
            };
        }
    }

    private void CheckEnd()
    {
        picks--;
        
        if (picks == 0)
        {
            State.Instance.NextLevel();
        }
    }

    public override void Select(Card card)
    {
        if (modifier != default)
        {
            modifier.ChangeSelection(false);
            Combine(modifier, card);
        }
        
        modifier = default;
        DeselectAll();
    }

    public override void DropToSlot(Card card, Slot slot)
    {
    }

    protected override void Combine(Card first, Card second)
    {
        var data = State.Instance.GetCard(second.Id);
        if (data == default) return;
        if (data.type == CardType.Joker)
        {
            data.type = CardType.Normal;
            data.number = second.Number;
        }
        data.Modify(first.GetData());
        second.Setup(data, deck);
        second.Flip();
        first.Kill();
        CheckEnd();
    }

    public override bool CanCombine(Card first, Card second)
    {
        return first.IsModifier && State.Instance.Has(second.Id);
    }

    public override void RightClick(Card card)
    {
    }
    
    public override int GetJokerValue()
    {
        return deck.Cards.Where(c => !c.IsJoker && c.IsOpen).Sum(c => c.Number);
    }
}