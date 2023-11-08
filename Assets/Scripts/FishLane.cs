using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class FishLane : MonoBehaviour
{
    [SerializeField] private float delay;
    
    private readonly List<Card> cards = new();

    private const float RowHeight = 1.5f;

    public bool IsFull => cards.Count == 4;
    public List<Card> Cards => cards;

    public void Fill(Deck deck)
    {
        this.StartCoroutine(() =>
        {
            if (cards.Count >= 4) return;

            var card = deck.Draw();
            if (!card) return;
            var pos = transform.position;
            var reserve = cards.Count == 3;
            card.MoveTo(pos + Vector3.up * (5f * RowHeight));
            cards.Add(card);

            if (!reserve)
            {
                this.StartCoroutine(() => DropCard(card), 0.3f - delay);
            }
            else
            {
                card.Lock();
            }
        }, delay);
    }

    public void Deselect()
    {
        foreach (var card in cards)
        {
            card.ChangeSelection(false);
        }
    }

    public void Drop()
    {
        cards.Take(3).ToList().ForEach(DropCard);
    }

    private void DropCard(Card card)
    {
        var index = cards.IndexOf(card);
        card.MoveTo(transform.position + Vector3.up * (index * RowHeight));
        card.Flip();
    }

    public void Remove(List<Card> targets)
    {
        cards.RemoveAll(targets.Contains);
    }
}