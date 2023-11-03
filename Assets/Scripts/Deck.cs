using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private GameMode gameMode;

    private readonly List<Card> cards = new();

    public IEnumerable<Card> Cards => cards;

    private void Start()
    {
        new List<int>{ 1, 2, 3, 4, 5, 5, 6, 7, 8, 9, 10, 10 }.RandomOrder().ToList().ForEach(AddCard);
        gameMode.Setup();
    }

    public Card Draw()
    {
        var card = cards.LastOrDefault();
        cards.Remove(card);
        return card;
    }

    private void AddCard(int i)
    {
        var card = Instantiate(cardPrefab, transform);
        card.transform.position += Vector3.up * 0.2f * cards.Count;
        card.Setup(i, this);
        cards.Add(card);
    }

    public void Kill(List<Card> targets)
    {
        targets.ForEach(c => c.Kill());
        cards.RemoveAll(targets.Contains);
    }

    public void Select(Card card)
    {
        gameMode.Select(card);
    }

    public void DropToSlot(Card card, Slot slot)
    {
        gameMode.DropToSlot(card, slot);
    }

    public bool CanCombine(Card first, Card second)
    {
        return first != second && gameMode.CanCombine(first, second);
    }

    public bool TryCombine(Card first, Card second)
    {
        return first != second && gameMode.TryCombine(first, second);
    }

    public void RightClick(Card card)
    {
        gameMode.RightClick(card);
    }
}