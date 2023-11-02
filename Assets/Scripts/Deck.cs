using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private GameMode gameMode;

    private readonly List<Card> cards = new();

    public IEnumerable<Card> Cards => cards;

    private void Start()
    {
        for (var i = 1; i < 11; i++)
        {
            var card = Instantiate(cardPrefab, transform);
            card.Setup(i, this);
            card.transform.position += Vector3.right * (i - 5);
            cards.Add(card);
        }
        
        gameMode.Setup();
    }

    public void Kill(List<Card> targets)
    {
        targets.ForEach(c => c.Kill());
        cards.RemoveAll(targets.Contains);
    }

    public void Select()
    {
        gameMode.Select();
    }
}