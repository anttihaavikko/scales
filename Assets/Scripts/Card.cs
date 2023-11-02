using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Visuals;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Draggable draggable;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private TMP_Text numberLabel;
    [SerializeField] private Collider2D coll;
    [SerializeField] private AutoSpriteOrderOnStart sorter;

    private bool wasSelected;
    private bool selected;
    private Deck deck;
    private int number;
    private bool removed;
    
    private readonly List<Card> covers = new();

    public bool IsSelected => selected;
    public int Number => number;
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);

    public void Setup(int n, Deck d)
    {
        number = n;
        numberLabel.text = n.ToString();
        deck = d;
        numberLabel.gameObject.SetActive(false);
        draggable.CanDrag = false;
        backSprite.color = Color.red;
    }

    public void SetDepth()
    {
        sorter.Apply();
    }

    public void AddCover(Card other)
    {
        covers.Add(other);
    }
    
    public void RemoveCover(Card other)
    {
        covers.Remove(other);
    }

    public void Flip()
    {
        if (draggable.CanDrag) return;
        draggable.CanDrag = true;
        backSprite.color = Color.white;
        numberLabel.gameObject.SetActive(true);
        coll.enabled = true;
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
        draggable.droppedOn += OnDrop;
    }

    private void OnDrop(Collider2D obj)
    {
        var slot = obj.GetComponent<Slot>();
        if (slot && slot.IsEmpty)
        {
            deck.DropToSlot(this, slot);
            return;
        }

        if (slot && deck.TryCombine(this, slot.TopCard))
        {
            return;
        } 
        
        var card = obj.GetComponent<Card>();
        if (card && deck.TryCombine(this, card))
        {
            return;
        }
        
        draggable.CancelDrop();
    }

    private void OnPick()
    {
        wasSelected = selected;
        UpdateSelection(false);
    }

    private void OnClick()
    {
        UpdateSelection(!wasSelected);
    }

    private void UpdateSelection(bool state)
    {
        ChangeSelection(state);
        deck.Select(this);
    }
    
    public void ChangeSelection(bool state)
    {
        selected = state;
        backSprite.color = selected ? Color.yellow : Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            deck.RightClick(this);
        }
    }
}