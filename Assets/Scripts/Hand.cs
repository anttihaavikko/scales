using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Deck deck;

    private readonly List<Card> cards = new();
    
    public IEnumerable<Card> Cards => cards;

    public bool IsEmpty => !cards.Any();

    public void Draw()
    {
        var card = deck.Draw();
        if (card)
        {
            card.Flip();
            Reposition(card, cards.Count);
            cards.Add(card);
        }
    }

    public void Remove(Card card)
    {
        cards.Remove(card);
        RepositionAll();
    }

    private void Reposition(Card card, int offset)
    {
        Tweener.MoveToBounceOut(card.transform, transform.position + Vector3.right * (1.2f * offset), 0.1f);
    }

    private void RepositionAll()
    {
        var p = 0;
        cards.ForEach(c => Reposition(c, p++));
    }
}