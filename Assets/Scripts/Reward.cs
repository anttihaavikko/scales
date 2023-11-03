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
        var data = State.Instance.GetCard(card.Id);
        
        if (modifier != default && data != default)
        {
            data.Modify(modifier.GetData());
            card.Setup(data, deck);
            card.Flip();
            modifier.Kill();
            CheckEnd();
        }
        
        modifier = default;
        DeselectAll();
    }

    public override void DropToSlot(Card card, Slot slot)
    {
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