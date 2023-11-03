using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private bool unlimited;
    
    private readonly List<Card> cards = new();

    public Action<Slot> click;

    public Card TopCard => cards.LastOrDefault(c => c != default && !c.IsRemoved);
    
    public bool IsEmpty => !cards.Any(c => c != default && !c.IsRemoved);

    public bool Accepts => unlimited || IsEmpty;
    
    public int Count => cards.Count(c => c != default && !c.IsRemoved);
    
    public int Sum => cards.Where(c => c != default && !c.IsRemoved).Sum(c => c.Number);

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