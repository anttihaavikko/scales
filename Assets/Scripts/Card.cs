using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private Draggable draggable;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private TMP_Text numberLabel;

    private bool wasSelected;
    private bool selected;
    private Deck deck;
    private int number;
    private bool removed;
    
    private readonly List<Card> covers = new();

    public bool IsSelected => selected;
    public int Number => number;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);

    public void Setup(int n, Deck d)
    {
        number = n;
        numberLabel.text = n.ToString();
        deck = d;
        numberLabel.gameObject.SetActive(false);
        draggable.CanDrag = false;
    }

    public void AddCover(Card other)
    {
        covers.Add(other);
    }

    public void Flip()
    {
        draggable.CanDrag = true;
        numberLabel.gameObject.SetActive(true);
    }

    public void Kill()
    {
        removed = true;
        Destroy(gameObject);
    }

    private void Start()
    {
        draggable.click += OnClick;
        draggable.pick += OnPick;
    }

    private void OnPick()
    {
        wasSelected = selected;
        ChangeSelection(false);
    }

    private void OnClick()
    {
        ChangeSelection(!wasSelected);
    }

    private void ChangeSelection(bool state)
    {
        selected = state;
        backSprite.color = selected ? Color.yellow : Color.white;
        deck.Select();
    }
}