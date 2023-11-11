using System;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public bool playable;
    public int sort;

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
        playable = from.playable;
        sort = from.sort;
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
            GetCheat(),
            new CardData(CardModifier.Favourite, 0) { icon = 1 },
            new CardData(CardModifier.Scorer, 2) { icon = 2 }
        }.Random();
    }

    public static CardData GetCheat()
    {
        return new CardData(CardModifier.Cheat, 0) { icon = 0 };
    }

    private static CardData GetRandomSpecial()
    {
        return new []
        {
            new CardData(0),
            new CardData(99),
            new CardData(Random.Range(1, 99)),
            new CardData(Random.Range(1, 20)),
            new CardData(CardType.Joker) { sort = 998 },
            new CardData(CardType.Timer) { icon = 3 },
            new CardData(CardType.Recall) { icon = 4, playable = true, sort = 999 }
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

        if (type == CardType.Recall)
        {
            number = 0;
        }
    }
}