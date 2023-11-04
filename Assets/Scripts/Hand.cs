using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private bool locked;
    [SerializeField] private int size;

    private readonly List<Card> cards = new();

    public IEnumerable<Card> Cards => cards.Where(c => c && !c.IsRemoved);

    public bool IsEmpty => !Cards.Any();
    public bool HasRoom => Cards.ToList().Count < size;

    public void Fill()
    {
        StartCoroutine(DrawUntilFull());
    }

    private IEnumerator DrawUntilFull()
    {
        while (HasRoom)
        {
            Draw();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Draw()
    {
        var card = deck.Draw();
        if (card)
        {
            card.Flip();
            cards.Add(card);
            card.Lock(locked);
            RepositionAll();
        }
    }

    public void Add(List<Card> additions)
    {
        additions.ForEach(c =>
        {
            c.Lock(locked);
            if (deck)
            {
                c.SetDeck(deck);
            }
        });
        cards.AddRange(additions);
        RepositionAll();
    }

    public void Remove(Card card)
    {
        cards.Remove(card);
        RepositionAll();
    }

    private void Reposition(Card card, float offset)
    {
        Tweener.MoveToBounceOut(card.transform, transform.position + Vector3.right * (1.2f * offset), 0.1f);
    }

    private void RepositionAll()
    {
        var handCards = Cards.OrderBy(c => c.transform.position.x).ToList();
        var p = -(handCards.Count - 1) * 0.5f;
        if (deck)
        {
            Tweener.MoveToBounceOut(deck.transform, transform.position + Vector3.right * Mathf.Min(-2f, 1.2f * (p - 1.2f)), 0.1f);   
        }
        handCards.ForEach(c => Reposition(c, p++));
    }
}