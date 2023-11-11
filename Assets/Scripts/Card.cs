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

public class Card : Markable, IPointerClickHandler
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
    [SerializeField] private GameObject modifier;
    [SerializeField] private SpriteRenderer cheatMarkIcon;
    [SerializeField] private GameObject cheatMarkBg;

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
    private Markable marked;

    public Action click;

    public bool IsSelected => selected;
    public int Number => IsJoker ? deck.GetTotal() : number;
    public int ScoreValue => (IsJoker ? deck.GetTotal() : number) * stats.multiplier;
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);
    public bool IsModifier => stats.modifier != CardModifier.None;
    public bool IsValueModifier => stats.modifier is CardModifier.Minus or CardModifier.Multiply or CardModifier.Plus; 
    public Guid Id => id;
    public bool IsJoker => stats.type == CardType.Joker;
    public bool IsOpen => open;
    public int SortValue => GetSortValue();
    public bool NeedsFlattening => stats.type is CardType.Timer or CardType.Joker;
    public bool IsPlayable => stats.playable;
    public bool Is(CardType type) => stats.type == type;
    public bool IsUsed { get; set; }

    private int GetSortValue()
    {
        var val = stats.sort > 0 ? stats.sort : stats.multiplier * number;
        if (cardType == CardType.Timer) val = 60;
        if (stats.favourite) val += 200;
        if (stats.cheat) val += 200;
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
            cheatMarkIcon.sprite = icon.sprite;
            cheatMarkBg.SetActive(stats.cheat);
        }
        cheatLabels[0].transform.localPosition = data.cheatPos;
        cheatLabels.ForEach(t => t.text = data.cheat ? numberLabel.text : "");
        cheatIcon.SetActive(stats.cheat);
        favourite.SetActive(stats.favourite);
        var multiText = stats.multiplier > 1 ? $"x{stats.multiplier}" : "";
        multiTexts.ForEach(t => t.text = multiText);
        modifier.SetActive(IsModifier);
        deck = d;
        numberLabel.gameObject.SetActive(false);
        draggable.CanDrag = false;
        backSprite.color = backColor;
        back.gameObject.SetActive(true);

        StartTimer();
    }
    
    public void Setup(Skill skill, Deck d)
    {
        Setup(new CardData(0), d);
        icon.sprite = skill.icon;
        icon.gameObject.SetActive(true);
        numberLabel.text = "";
    }

    public void ChangeNumber(int value)
    {
        var d = GetData();
        if (!d.CanBeModified) return;
        d.number = value;
        Setup(d, deck);
        Flip();
    }

    private void StartTimer()
    {
        if (cardType != CardType.Timer) return;
        number = DateTime.Now.Second;
        numberLabel.fontSize = 2.5f;
        numberLabel.enableAutoSizing = false;
        numberLabel.transform.localPosition = new Vector3(0, 0.05f, 0);
        numberLabel.text = number.ToString();
        Invoke(nameof(StartTimer), 1f);
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
        if (!marked) return;
        marked.Mark(false, false);
        marked = default;
    }

    private void OnPreview(List<Collider2D> targets)
    {
        if (!deck) return;
        var p = transform.position;
        var nextMark = targets
            .Select(GetCardOrSlot)
            .Where(t => t.AcceptsCard(this, deck))
            .OrderBy(c => Vector3.Distance(c.transform.position, p))
            .FirstOrDefault();
        if (marked && marked != nextMark) marked.Mark(false, false);
        if (marked == nextMark) return;
        marked = nextMark;
        Mark(marked, false);
        if(marked) marked.Mark(true, true);
    }

    public override bool AcceptsCard(Card card, Deck d)
    {
        return deck.CanCombine(card, this);
    }

    private Markable GetCardOrSlot(Component component)
    {
        var card = component.GetComponent<Card>();
        if (card) return card;
        var slot = component.GetComponent<Slot>();
        return slot;
    }

    private void OnDrop(List<Collider2D> objects)
    {
        shadow.SetActive(false);

        var markedSlot = marked as Slot;
        var markedCard = marked as Card;

        if (markedSlot && markedSlot.Accepts && deck)
        {
            deck.DropToSlot(this, markedSlot);
            return;
        }

        if (markedSlot && deck && deck.TryCombine(this, markedSlot.TopCard))
        {
            return;
        }

        if (markedCard && deck && deck.TryCombine(this, markedCard))
        {
            return;
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
        if (IsModifier)
        {
            this.StartCoroutine(() => deck.Dragon.Tutorial.Show(TutorialMessage.ModInfo), 0.5f);
        }
        Nudge();
        wasSelected = selected;
        UpdateSelection(false);
        shadow.SetActive(true);
    }

    private void OnClick()
    {
        UpdateSelection(!wasSelected);
        deck.PlayInstant(this);
        click?.Invoke();
    }

    private void UpdateSelection(bool state)
    {
        ChangeSelection(state);
        if (!deck) return;
        deck.Select(this);
    }
    
    public void ChangeSelection(bool state, bool dark = false)
    {
        selected = state;
        Mark(selected, dark);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            deck.RightClick(this);
        }
    }

    public override void Mark(bool state, bool dark)
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

    public void Pop(bool shake = true)
    {
        if (shake && deck) deck.Shake(0.1f);
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
    Joker,
    Timer,
    Recall,
    Averager,
    Kill
}