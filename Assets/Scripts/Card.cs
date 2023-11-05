using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
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
    [SerializeField] private Color backColor;
    [SerializeField] private SpriteRenderer back;
    [SerializeField] private Color selectColor;
    [SerializeField] private GameObject shadow;

    private Guid id;
    private bool wasSelected;
    private bool selected;
    private Deck deck;
    private int number;
    private bool removed;
    private CardModifier modifier;
    private CardType cardType;
    private bool open;
    
    private readonly List<Card> covers = new();
    private List<Card> marked = new();

    public Action click;

    public bool IsSelected => selected;
    public int Number => IsJoker ? deck.GetTotal() : number;
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);
    public bool IsModifier => modifier != CardModifier.None;
    public Guid Id => id;
    public bool IsJoker => cardType == CardType.Joker;
    public bool IsOpen => open;
    public int SortValue => IsJoker ? 999 : number;

    public void Setup(CardData data, Deck d)
    {
        id = data.id;
        cardType = data.type;
        number = data.number;
        modifier = data.modifier;
        numberLabel.text = data.GetPrefix() + number;
        if (cardType == CardType.Joker) numberLabel.text = "J";
        deck = d;
        numberLabel.gameObject.SetActive(false);
        draggable.CanDrag = false;
        backSprite.color = backColor;
        back.gameObject.SetActive(true);
    }

    public void SetDeck(Deck d)
    {
        deck = d;
    }

    public void Flatten()
    {
        number = Number;
        cardType = CardType.Normal;
    }

    public CardData GetData()
    {
        return new CardData(modifier, number)
        {
            id = id,
            type = cardType
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
        open = true;
        draggable.CanDrag = true;
        backSprite.color = Color.white;
        numberLabel.gameObject.SetActive(true);
        coll.enabled = true;
        back.gameObject.SetActive(false);
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
        marked.ForEach(t => t.ToggleMarking(false));
        // ToggleMarking(false);
    }

    private void OnPreview(List<Card> targets)
    {
        if (!deck) return;
        var nextMarks = targets.Where(t => deck.CanCombine(this, t)).ToList();
        marked.Where(m => !nextMarks.Contains(m)).ToList().ForEach(t => t.ToggleMarking(false));
        marked = nextMarks;
        ToggleMarking(marked.Any());
        var p = transform.position;
        marked.OrderBy(c => Vector3.Distance(c.transform.position, p)).Take(1).ToList().ForEach(t => t.ToggleMarking(true));
        // Debug.Log($"Preview for {number} => {string.Join(",", marked.Select(m => m.number))}");
    }

    private void OnDrop(List<Collider2D> objects)
    {
        shadow.SetActive(false);
        var p = transform.position;
        foreach (var obj in objects.OrderBy(c => Vector3.Distance(c.transform.position, p)))
        {
            var slot = obj.GetComponent<Slot>();
            if (slot && slot.Accepts && deck)
            {
                deck.DropToSlot(this, slot);
                return;
            }

            if (slot && deck && deck.TryCombine(this, slot.TopCard))
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

    public void ReturnToPrevious()
    {
        draggable.CancelDrop();
        shadow.SetActive(false);
    }

    private void OnPick()
    {
        Nudge();
        wasSelected = selected;
        UpdateSelection(false);
        shadow.SetActive(true);
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
        ToggleMarking(selected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            deck.RightClick(this);
        }
    }

    private void ToggleMarking(bool state)
    {
        outline.SetActive(state);
        backSprite.color = state ? selectColor : Color.white;
    }

    public void Lock(bool state = true)
    {
        draggable.CanDrag = !state;
        coll.enabled = !state;
    }

    public void MoveTo(Vector3 pos, float speed = 1f)
    {
        Tweener.MoveToBounceOut(transform, pos, 0.1f / speed);
        Nudge();
    }

    public void Lift(float delay = 1f)
    {
        draggable.SetSortOrder("Picked");
        this.StartCoroutine(() => draggable.SetSortOrder("Default"), 0.1f * delay);
    }

    public void Detach()
    {
        transform.SetParent(null);
    }

    public void Nudge()
    {
        var rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3f, 3f)));
        Tweener.RotateToBounceOut(transform, rot, 0.1f);
    }
}

public enum CardModifier
{
    None,
    Plus,
    Minus,
    Multiply
}

public enum CardType
{
    Normal,
    Joker
}

public class CardData
{
    public Guid id;
    public int number;
    public CardModifier modifier;
    public CardType type;

    public CardData(int value)
    {
        id = Guid.NewGuid();
        number = value;
        modifier = CardModifier.None;
        type = CardType.Normal;
    }

    public CardData(CardType specialType) : this(0)
    {
        type = specialType;
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
            new CardData(Random.Range(1, 20)),
            new CardData(CardType.Joker)
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