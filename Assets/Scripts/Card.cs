using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Visuals;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Draggable draggable;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private TMP_Text numberLabel;
    [SerializeField] private Collider2D coll;
    [SerializeField] private AutoSpriteOrderOnStart sorter;
    [SerializeField] private GameObject outline;
    [SerializeField] private SortingGroup sortingGroup;

    private bool wasSelected;
    private bool selected;
    private Deck deck;
    private int number;
    private bool removed;
    
    private readonly List<Card> covers = new();
    private List<Card> marked = new();

    public Action click;

    public bool IsSelected => selected;
    public int Number => number;
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);

    public void Setup(CardData data, Deck d)
    {
        number = data.number;
        numberLabel.text = number.ToString();
        deck = d;
        numberLabel.gameObject.SetActive(false);
        draggable.CanDrag = false;
        backSprite.color = Color.red;
    }

    public void SetDepth(Card target, int dir)
    {
        if (!target) return;
        sortingGroup.sortingOrder = target.sortingGroup.sortingOrder + dir;
    }

    public void SetDepth()
    {
        sortingGroup.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
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
        draggable.preview += OnPreview;
        draggable.hidePreview += OnHidePreview;
    }

    private void OnHidePreview()
    {
        marked.ForEach(t => t.ToggleOutline(false));
        ToggleOutline(false);
    }

    private void OnPreview(List<Card> targets)
    {
        if (!deck) return;
        marked.ForEach(t => t.ToggleOutline(false));
        marked = targets.Where(t => deck.CanCombine(this, t)).ToList();
        ToggleOutline(marked.Any());
        marked.ForEach(t => t.ToggleOutline(true));
        // Debug.Log($"Preview for {number} => {string.Join(",", marked.Select(m => m.number))}");
    }

    private void OnDrop(List<Collider2D> objects)
    {
        foreach (var obj in objects)
        {
            var slot = obj.GetComponent<Slot>();
            if (slot && slot.Accepts && deck)
            {
                deck.DropToSlot(this, slot);
                return;
            }

            if (slot && deck &&deck.TryCombine(this, slot.TopCard))
            {
                return;
            } 
        
            var card = obj.GetComponent<Card>();
            if (card && deck && deck.TryCombine(this, card))
            {
                return;
            }
        }

        draggable.CancelDrop();
    }

    private void OnPick()
    {
        wasSelected = selected;
        UpdateSelection(false);
        click?.Invoke();
    }

    private void OnClick()
    {
        UpdateSelection(!wasSelected);
    }

    private void UpdateSelection(bool state)
    {
        ChangeSelection(state);
        if (!deck) return;
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

    public void ToggleOutline(bool state)
    {
        outline.SetActive(state);
    }

    public void Lock()
    {
        draggable.CanDrag = false;
    }

    public void DisableCollider()
    {
        coll.enabled = false;
    }
}

public class CardData
{
    public int number;

    public CardData(int value)
    {
        number = value;
    }
}