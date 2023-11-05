using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Visuals;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
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
    [SerializeField] private Color selectColor, darkerSelectColor;
    [SerializeField] private GameObject shadow;
    [SerializeField] private List<TMP_Text> cheatLabels;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private GameObject cheatIcon;
    [SerializeField] private List<TMP_Text> multiTexts;
    [SerializeField] private GameObject favourite;

    private Guid id;
    private bool wasSelected;
    private bool selected;
    private Deck deck;
    private int number;
    private bool removed;
    private bool open;

    private CardData stats;
    private CardType cardType;
    
    private readonly List<Card> covers = new();
    private Card marked;

    public Action click;

    public bool IsSelected => selected;
    public int Number => IsJoker ? deck.GetTotal() : number;
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);
    public bool IsModifier => stats.modifier != CardModifier.None;
    public Guid Id => id;
    public bool IsJoker => stats.type == CardType.Joker;
    public bool IsOpen => open;
    public int SortValue => GetSortValue();

    private int GetSortValue()
    {
        var val = IsJoker ? 999 : stats.multiplier * number;
        if (stats.favourite) val += 999;
        if (stats.cheat) val += 999;
        return val;
    }

    public void Setup(CardData data, Deck d)
    {
        stats = new CardData(data);
        cardType = data.type;
        id = data.id;
        number = data.number;
        numberLabel.text = data.GetPrefix() + number;
        if (stats.type == CardType.Joker) numberLabel.text = "J";
        if (stats.icon >= 0)
        {
            numberLabel.text = "";
            icon.sprite = icons[stats.icon];
            icon.gameObject.SetActive(true);
        }
        cheatLabels[0].transform.localPosition = data.cheatPos;
        cheatLabels.ForEach(t => t.text = data.cheat ? numberLabel.text : "");
        cheatIcon.SetActive(stats.cheat);
        favourite.SetActive(stats.favourite);
        if (stats.multiplier > 1)
        {
            multiTexts.ForEach(t => t.text = $"x{stats.multiplier}");
        }
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
        return stats;
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
        EffectManager.AddEffect(2, transform.position);
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
        draggable.dropCancelled += OnDropCancel;
    }

    private void OnDropCancel()
    {
        shadow.SetActive(false);
    }

    private void OnHidePreview()
    {
        if (marked)
        {
            marked.ToggleMarking(false);
            marked = null;
        }
        // ToggleMarking(false);
    }

    private void OnPreview(List<Card> targets)
    {
        if (!deck) return;
        var p = transform.position;
        var nextMark = targets
            .Where(t => deck.CanCombine(this, t))
            .OrderBy(c => Vector3.Distance(c.transform.position, p))
            .FirstOrDefault();
        if (marked && marked != nextMark) marked.ToggleMarking(false);
        marked = nextMark;
        ToggleMarking(marked);
        if (marked) marked.ToggleMarking(true, true);
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

        ReturnToPrevious();
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

    private void ToggleMarking(bool state, bool dark = false)
    {
        outline.SetActive(state);
        var color = dark ? darkerSelectColor : selectColor;
        backSprite.color = state ? color : Color.white;
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
        shadow.SetActive(true);
        this.StartCoroutine(() =>
        {
            draggable.SetSortOrder("Default");
            shadow.SetActive(false);
        }, 0.1f * delay);
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

    public void Pop()
    {
        EffectManager.AddEffects(new []{ 0, 1, 2 }, transform.position);
    }
}

public enum CardModifier
{
    None,
    Plus,
    Minus,
    Multiply,
    Cheat,
    Scorer,
    Favourite
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
    public bool cheat;
    public int icon;
    public int multiplier;
    public bool favourite;

    public bool isInitialized;
    public Vector2 cheatPos;

    public CardData(int value)
    {
        id = Guid.NewGuid();
        number = value;
        modifier = CardModifier.None;
        type = CardType.Normal;
        cheatPos = Vector2.zero;
        icon = -1;
        multiplier = 1;
    }
    
    public CardData(CardData from)
    {
        id = from.id;
        number = from.number;
        modifier = from.modifier;
        type = from.type;
        cheat = from.cheat;
        icon = from.icon;
        multiplier = from.multiplier;
        favourite = from.favourite;
    }

    public void Init()
    {
        if (isInitialized) return;
        isInitialized = true;
        cheatPos = new Vector2(Random.Range(-0.75f, 0.75f), Random.Range(-0.9f, 1.4f));
    }

    private CardData(CardType specialType) : this(0)
    {
        type = specialType;
    }

    private CardData(CardModifier mod, int value) : this(value)
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
            CardModifier.Cheat => "!",
            CardModifier.Scorer => "!",
            CardModifier.Favourite => "!",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static CardData GetRandomModifier()
    {
        return new []
        {
            new CardData(CardModifier.Plus, 1),
            new CardData(CardModifier.Plus, 2),
            new CardData(CardModifier.Minus, 1),
            new CardData(CardModifier.Multiply, 2),
            new CardData(CardModifier.Cheat, 0) { icon = 0 },
            new CardData(CardModifier.Favourite, 0) { icon = 1 },
            new CardData(CardModifier.Scorer, 2) { icon = 2 }
        }.Random();
    }

    private static CardData GetRandomSpecial()
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
        switch (card.modifier)
        {
            case CardModifier.Plus:
                number += card.number;
                break;
            case CardModifier.Minus:
                number -= card.number;
                break;
            case CardModifier.Multiply:
                number *= card.number;
                break;
            case CardModifier.Cheat:
                cheat = true;
                break;
            case CardModifier.None:
                break;
            case CardModifier.Scorer:
                multiplier *= card.number;
                break;
            case CardModifier.Favourite:
                favourite = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}