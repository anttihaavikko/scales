using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Card : Markable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
    [SerializeField] private GameObject trail;
    [SerializeField] private CursorChanger cursorChanger;

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
    private Skill? linkedSkill;

    public Action click, picked, dropped;

    public bool IsSelected => selected;
    public int Number => GetNumberValue();
    public int ScoreValue => Mathf.Abs(GetScoreValue());
    public bool IsRemoved => removed;
    public bool IsCovered => covers.Any(c => c != default && !c.removed);
    public bool IsModifier => stats.modifier != CardModifier.None;
    public bool IsValueModifier => stats.modifier is CardModifier.Minus or CardModifier.Multiply or CardModifier.Plus or CardModifier.Swapper; 
    public Guid Id => id;
    public bool IsJoker => stats.type == CardType.Joker;
    public bool IsAce => stats.type == CardType.Ace;
    public bool IsOpen => open;
    public int SortValue => GetSortValue();
    public bool NeedsFlattening => stats.type is CardType.Timer or CardType.Joker;
    public bool IsPlayable => stats.playable;
    public bool Is(CardType type) => stats.type == type;
    public bool IsUsed { get; set; }
    public string Title => linkedSkill.HasValue ? linkedSkill.Value.title : stats.GetTitle();
    public string Description => linkedSkill.HasValue ? linkedSkill.Value.description : stats.GetDescription(Number);
    public bool HasTooltip => linkedSkill.HasValue || stats.type != CardType.Normal || stats.modifier != CardModifier.None;
    public List<TooltipExtra> Extras => stats.GetExtras();
    public int Multiplier => stats.multiplier;
    public bool IsDuplicator => stats.modifier == CardModifier.Duplicator;
    public bool IsTimer => stats.type == CardType.Timer;
    public bool IsTrueJoker => cardType == CardType.TrueJoker;

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
        numberLabel.fontSize = 7f;
        numberLabel.enableAutoSizing = true;
        numberLabel.transform.localPosition = new Vector3(0, 0.1f);
        
        stats = new CardData(data);
        cardType = data.type;
        id = data.id;
        number = data.number;
        numberLabel.text = data.GetPrefix() + number;
        if (stats.type == CardType.Joker) numberLabel.text = "J";
        if (stats.type == CardType.Ace) numberLabel.text = "A";
        if (stats.icon >= 0)
        {
            numberLabel.text = "";
            icon.sprite = icons[stats.icon];
            icon.gameObject.SetActive(true);
            cheatMarkIcon.sprite = icon.sprite;
            cheatMarkBg.SetActive(stats.cheat);
        }
        else
        {
            icon.sprite = null;
        }

        if (stats.type == CardType.Mox)
        {
            numberLabel.fontSize = 2.5f;
            numberLabel.enableAutoSizing = false;
            numberLabel.transform.localPosition = new Vector3(0, 0.075f, 0);
            numberLabel.text = data.GetPrefix() + number;
        }
        
        if (stats.type == CardType.Pedometer)
        {
            numberLabel.fontSizeMax = 2.5f;
            numberLabel.transform.localPosition = new Vector3(0, 0.35f, 0);
            numberLabel.text = data.GetPrefix() + number;
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

    public void IncreaseNumber()
    {
        if (cardType != CardType.Pedometer) return;
        if (!State.Instance.Has(Id)) return;
        var data = State.Instance.GetCard(Id);
        data.number++;
        Setup(data, deck);
        Flip();
    }
    
    public void Setup(Skill skill, Deck d)
    {
        linkedSkill = skill;
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
        deck.Tooltip.Hide(this);
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
        cursorChanger.Change(1);
        dropped?.Invoke();
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
        cursorChanger.Change(1);
        dropped?.Invoke();
        
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
        AudioManager.Instance.PlayEffectFromCollection(1, transform.position, 0.6f);
        cursorChanger.Change(2);
        
        picked?.Invoke();
        deck.Tooltip.Hide();
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
            AudioManager.Instance.PlayEffectFromCollection(1, transform.position, 0.6f);
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
        AudioManager.Instance.PlayEffectFromCollection(2, transform.position, 0.6f);
        deck.Tooltip.Hide(this);
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
        AudioManager.Instance.PlayEffectFromCollection(0, transform.position, 0.2f);
        var rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3f, 3f)));
        Tweener.RotateToBounceOut(transform, rot, 0.1f);
    }

    public void Pop(bool shake = true)
    {
        cursorChanger.Change(0);
        
        var p = transform.position;
        AudioManager.Instance.PlayEffectFromCollection(0, p, 1f);
        AudioManager.Instance.PlayEffectFromCollection(5, p, 1.3f);
        if (shake)
        {
            AudioManager.Instance.PlayEffectFromCollection(4, p, 1f);   
        }
        AudioManager.Instance.NudgePitch(1.3f, 0.2f);

        if (shake && deck)
        {
            deck.Shake(0.1f);
            EffectManager.AddEffect(3, transform.position);
        }
        EffectManager.AddEffects(new []{ 0, 1, 2 }, transform.position);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (draggable.IsDragging) return;
        
        cursorChanger.Change(1);
        
        Nudge();
        transform.localScale = Vector3.one * 1.1f; 
        
        if (linkedSkill.HasValue)
        {
            deck.Tooltip.Show(linkedSkill.Value, deck.Camera.WorldToScreenPoint(transform.position));
            return;
        }
        deck.Tooltip.Show(this, deck.Camera.WorldToScreenPoint(transform.position));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        deck.Tooltip.Hide();
        cursorChanger.Change(0);
    }
    
    private int GetExtraMultipliers()
    {
        var multi = 1;
        if (Number > 10) multi *= 1 + State.Instance.GetCount(Effect.BigMulti) * 2;
        if (Number % 2 == 0) multi *= 1 + State.Instance.GetCount(Effect.EvenScorer);
        if (Number > 20) multi *= 1 + State.Instance.GetCount(Effect.UkkoPowerPlant) * 10;
        return multi;
    }
    
    private int GetScoreValue()
    {
        var baseValue = IsJoker ? deck.GetTotal() : number;
        if (IsTrueJoker) baseValue = deck.GetTrueJokerValue();
        return baseValue * stats.multiplier * GetExtraMultipliers();
    }
    
    private int GetNumberValue()
    {
        if (cardType == CardType.MultiValue) return deck.GetMultiplier();
        if (IsTrueJoker) return deck.GetTrueJokerValue();
        return IsJoker ? deck.GetTotal() : number;
    }

    public void ToggleTrail(bool state)
    {
        trail.SetActive(state);
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
    Favourite,
    Swapper,
    Duplicator
}

public enum CardType
{
    Normal,
    Joker,
    Timer,
    Recall,
    Averager,
    Kill,
    Lotus,
    Mox,
    TrueJoker,
    Pedometer,
    MultiValue,
    Ace
}