using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Visuals;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Draggable draggable;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private TMP_Text numberLabel;
    [SerializeField] private Collider2D coll;
    [SerializeField] private AutoSpriteOrderOnStart sorter;
    [SerializeField] private GameObject outline;
    [SerializeField] private SortingGroup sortingGroup;

    private Guid id;
    private bool wasSelected;
    private bool selected;
    private Deck deck;
    private int number;
    private bool removed;
    private CardModifier modifier;
    
    private readonly List<Card> covers = new();
    private List<Card> marked = new();

    public Action click;

    public bool IsSelected => selected;
    public int Number => number;
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);
    public bool IsModifier => modifier != CardModifier.None;
    public Guid Id => id;

    public void Setup(CardData data, Deck d)
    {
        id = data.id;
        number = data.number;
        modifier = data.modifier;
        numberLabel.text = data.GetPrefix() + number;
        deck = d;
        numberLabel.gameObject.SetActive(false);
        draggable.CanDrag = false;
        backSprite.color = Color.red;
    }
    
    public CardData GetData()
    {
        return new CardData(modifier, number)
        {
            id = id
        };
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
        var p = transform.position;
        marked.OrderBy(c => Vector3.Distance(c.transform.position, p)).Take(1).ToList().ForEach(t => t.ToggleOutline(true));
        // Debug.Log($"Preview for {number} => {string.Join(",", marked.Select(m => m.number))}");
    }

    private void OnDrop(List<Collider2D> objects)
    {
        var p = transform.position;
        foreach (var obj in objects.OrderBy(c => Vector3.Distance(c.transform.position, p)))
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
    }

    private void OnClick()
    {
        UpdateSelection(!wasSelected);
        click?.Invoke();
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

public enum CardModifier
{
    None,
    Plus,
    Minus,
    Multiply
}

public class CardData
{
    public Guid id;
    public int number;
    public CardModifier modifier;

    public CardData(int value)
    {
        id = Guid.NewGuid();
        number = value;
        modifier = CardModifier.None;
    }
    
    public CardData(CardModifier mod, int value) : this(value)
    {
        modifier = mod;
    }

    public string GetPrefix()
    {
        return modifier switch
        {
            CardModifier.None => "",
            CardModifier.Plus => "+",
            CardModifier.Minus => "-",
            CardModifier.Multiply => "x",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static CardData GetRandomModifier()
    {
        return new []
        {
            new CardData(CardModifier.Plus, 1),
            new CardData(CardModifier.Plus, 2),
            new CardData(CardModifier.Minus, 1),
            new CardData(CardModifier.Multiply, 2)
        }.Random();
    }
    
    public static CardData GetRandomSpecial()
    {
        return new []
        {
            new CardData(0),
            new CardData(99),
            new CardData(Random.Range(1, 99)),
            new CardData(Random.Range(1, 20))
        }.Random();
    }
    
    public static CardData GetRandom()
    {
        if (Random.value < 0.5f) return new CardData(Random.Range(1, 11));
        if (Random.value < 0.5f) return GetRandomModifier();
        return GetRandomSpecial();
    }

    public void Modify(CardData card)
    {
        if(card.modifier == CardModifier.Plus) number += card.number;
        if(card.modifier == CardModifier.Minus) number -= card.number;
        if(card.modifier == CardModifier.Multiply) number *= card.number;
    }
}