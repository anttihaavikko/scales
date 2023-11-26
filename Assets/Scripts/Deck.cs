using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private bool isEnemy;

    private readonly List<Card> cards = new();

    public IEnumerable<Card> Cards => cards;
    public Dragon Dragon => gameMode.Dragon;

    public bool IsEmpty => cards.All(c => c.IsRemoved);
    public Tooltip Tooltip => gameMode.Tooltip;
    public Camera Camera => gameMode.Camera;

    private void Start()
    {
        var deck = isEnemy ? State.Instance.OpponentCards : State.Instance.Cards;
        deck.OrderBy(c => c.favourite ? 0 : 1).ThenBy(_ => Random.value).Reverse().ToList().ForEach(c => AddCard(c));
        gameMode.Setup();
    }

    public Card Draw()
    {
        var card = cards.LastOrDefault();
        cards.Remove(card);
        return card;
    }

    public Card Create(CardData data)
    {
        data.Init();
        var card = Instantiate(cardPrefab, transform);
        card.transform.position += Vector3.up * 0.1f * cards.Count;
        card.Setup(data, this);
        return card;
    }

    public Card AddCard(CardData data)
    {
        var card = Create(data);
        card.transform.position += Vector3.up * 0.1f * cards.Count;
        cards.Add(card);
        return card;
    }

    public void Kill(List<Card> targets)
    {
        targets.ForEach(c => c.Kill());
        cards.RemoveAll(targets.Contains);
    }

    public void PlayInstant(Card card)
    {
        if (card.IsPlayable)
        {
            gameMode.PlayInstant(card);
        }
    }

    public void Select(Card card)
    {
        if (card.IsPlayable) return;
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

    public int GetTotal()
    {
        return gameMode.GetJokerValue();
    }

    public bool CanPlay(Card card)
    {
        return gameMode.CanPlay(card);
    }

    public void Shake(float amount)
    {
        gameMode.Shake(amount);
    }

    public int GetTrueJokerValue()
    {
        return gameMode.GetTrueJokerValue();
    }

    public int GetMultiplier()
    {
        return gameMode.GetMultiplier();
    }
}