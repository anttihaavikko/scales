using System.Linq;
using UnityEngine;

public class Reward : GameMode
{
    [SerializeField] private Card cardPrefab;

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

        for (var i = 0; i < 3; i++)
        {
            var option = Instantiate(cardPrefab, transform);
            var data = new CardData(Random.Range(1, 11));
            option.Setup(data, null);
            option.transform.position += Vector3.right * 1.2f * i;
            option.Flip();

            option.click += () =>
            {
                State.Instance.Add(data);
                State.Instance.NextLevel();
            };
        }
    }

    public override void Select(Card card)
    {
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