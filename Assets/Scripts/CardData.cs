using System;
using System.Collections.Generic;
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

    public bool CanBeModified => type is not CardType.Recall or CardType.Averager or CardType.Kill;

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
            CardModifier.Swapper => "",
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
            new CardData(CardModifier.Multiply, 2),
            GetCheat(),
            new CardData(CardModifier.Favourite, 0) { icon = 1 },
            new CardData(CardModifier.Scorer, Random.Range(2, 4)) { icon = 2 },
            new CardData(CardModifier.Multiply, 0),
            new CardData(CardModifier.Swapper, 0) { icon = 7 }
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
            new CardData(CardType.Recall) { icon = 4, playable = true, sort = 999 },
            new CardData(CardType.Averager) { icon = 5, playable = true, sort = 997 },
            new CardData(CardType.Lotus) { icon = 8, playable = true, sort = 996 },
            new CardData(Random.Range(1, 6)) { icon = 9, multiplier = Random.Range(2, 6), type = CardType.Mox },
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
            case CardModifier.Swapper:
                (multiplier, number) = (number, multiplier);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (multiplier > 99)
        {
            multiplier = 1;
            number = 0;
            icon = 6;
            type = CardType.Kill;
            playable = true;
        }

        if (!CanBeModified)
        {
            number = 0;
        }
    }

    public string GetTitle()
    {
        if (modifier != CardModifier.None)
        {
            return modifier switch
            {
                CardModifier.None => "",
                CardModifier.Plus => "Plus",
                CardModifier.Minus => "Minus",
                CardModifier.Multiply => "Multiply",
                CardModifier.Cheat => "Cheat",
                CardModifier.Scorer => "Scorer",
                CardModifier.Favourite => "Favourite",
                CardModifier.Swapper => "Swapper",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return type switch
        {
            CardType.Normal => "",
            CardType.Joker => "Joker",
            CardType.Timer => "Timer",
            CardType.Recall => "Forebear's Memory",
            CardType.Averager => "Averager",
            CardType.Kill => "Death",
            CardType.Lotus => "Dark Blossom",
            CardType.Mox => $"Mox {GetNameSuffix()}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string GetNameSuffix()
    {
        var names = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
        return number is >= 0 and <= 10 ? names[number] : "Thing";
    }

    public List<TooltipExtra> GetExtras()
    {
        if (type == CardType.Kill) return new List<TooltipExtra> { TooltipExtra.Strike };
        return new List<TooltipExtra>();
    }
    
    public string GetDescription()
    {
        if (modifier != CardModifier.None)
        {
            return modifier switch
            {
                CardModifier.None => "",
                CardModifier.Plus => "(Adds) the value to (other card).",
                CardModifier.Minus => "(Subtracts) the value from (other card).",
                CardModifier.Multiply => "(Multiplies) other (card value) with this value.",
                CardModifier.Cheat => ExtraInfo.GetDescription(TooltipExtra.Cheat),
                CardModifier.Scorer => "Doubles the (score value) of the selected card.",
                CardModifier.Favourite => "Mark the (selected card) as (favourite).",
                CardModifier.Swapper => "Swap the (value) and (score multiplier) values of the selected card.",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return type switch
        {
            CardType.Normal => "",
            CardType.Joker => ExtraInfo.GetDescription(TooltipExtra.Joker),
            CardType.Timer => "Changes the (value) of the card to reflect the (current time).",
            CardType.Recall => "Draw (three extra) cards. Resets the (multiplier) afterwards.",
            CardType.Averager => "Change (all visible) card values to their (total average).",
            CardType.Kill => ExtraInfo.GetDescription(TooltipExtra.Death),
            CardType.Lotus => "Instantly gain (30 points) for each card in your hand. Resets the (multiplier) afterwards.",
            CardType.Mox => "Starts with an extra (score multiplier).",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}