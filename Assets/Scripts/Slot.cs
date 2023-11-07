using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slot : Markable
{
    [SerializeField] private bool unlimited;
    [SerializeField] private bool pile;
    [SerializeField] private Collider2D coll;
    [SerializeField] private Color normalColor, markColor;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Pulsater pulsater;

    private readonly List<Card> cards = new();

    public Action<Slot> click;

    public Card TopCard => cards.LastOrDefault(c => c != default && !c.IsRemoved);
    public bool IsEmpty => !cards.Any(c => c != default && !c.IsRemoved);
    public bool Accepts => unlimited || IsEmpty;
    public int Count => cards.Count(c => c != default && !c.IsRemoved);
    public int Sum => cards.Where(c => c != default && !c.IsRemoved).Sum(c => c.Number);
    public List<Card> Cards => cards;

    public void Add(Card card, bool pulsate = true)
    {
        if (pulsate)
        {
            pulsater.Pulsate();
        }
        
        if (TopCard && pile)
        {
            var t = TopCard.transform;
            TopCard.Nudge();
            // t.localScale *= 1.2f;
            // t.position = t.position.RandomOffset(0.1f);
        }
        
        cards.Add(card);
        // coll.enabled = false;
    }

    public override void Mark(bool state, bool dark)
    {
        sprite.color = state ? markColor : normalColor;
        if(state) pulsater.Pulsate();
    }

    public override bool AcceptsCard(Card card)
    {
        return Accepts;
    }

    public void Clear()
    {
        cards.Clear();
    }

    public void Remove(Card card)
    {
        cards.Remove(card);
        // coll.enabled = true;
    }
    
    private void OnMouseDown()
    {
        click?.Invoke(this);
    }

    public Vector3 GetPosition()
    {
        var offset = pile ? Vector3.zero : 0.2f * (Count - 1) * Vector3.up;
        return transform.position.WhereZ(0) + offset;
    }

    public bool Has(Card card)
    {
        return cards.Contains(card);
    }
}