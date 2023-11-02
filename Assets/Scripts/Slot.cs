using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private readonly List<Card> cards = new();

    public Action<Slot> click;

    public Card TopCard => cards.LastOrDefault(c => c != default && !c.IsRemoved);
    
    public bool IsEmpty => !cards.Any(c => c != default && !c.IsRemoved);

    public void Add(Card card)
    {
        cards.Add(card);
    }

    public void Remove(Card card)
    {
        cards.Remove(card);
    }
    
    private void OnMouseDown()
    {
        click?.Invoke(this);
    }
}