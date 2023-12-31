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
            CardModifier.Duplicator => "",
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
            new CardData(CardModifier.Scorer, 2) { icon = 2 },
            new CardData(CardModifier.Scorer, 2) { icon = 2 },
            new CardData(CardModifier.Scorer, 3) { icon = 10 },
            new CardData(CardModifier.Scorer, 3) { icon = 10 },
            new CardData(CardModifier.Multiply, 0),
            new CardData(CardModifier.Swapper, 0) { icon = 7 },
            new CardData(CardModifier.Duplicator, 0) { icon = 11 },
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
            new CardData(CardType.Joker) { sort = 994 },
            new CardData(CardType.Timer) { icon = 3 },
            new CardData(CardType.Recall) { icon = 4, playable = true, sort = 999 },
            new CardData(CardType.Averager) { icon = 5, playable = true, sort = 998 },
            new CardData(CardType.Lotus) { icon = 8, playable = true, sort = 997 },
            GetRandomGem(),
            new CardData(CardType.TrueJoker) { icon = 12, sort = 995 },
            new CardData(CardType.Pedometer) { icon = 13, sort = 99, number = 1 },
            new CardData(CardType.MultiValue) { icon = 14, sort = 993 },
            new CardData(CardType.Ace) { number = 1 }
        }.Random();
    }
    
    public static CardData GetRandomNonModifier()
    {
        if (Random.value < 0.7f) return GetBasic();
        return GetRandomSpecial();
    }

    public static CardData GetRandom()
    {
        if (Random.value < 0.5f) return GetBasic();
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
            case CardModifier.Duplicator:
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
                CardModifier.Scorer => number == 2 ? "Doubler" : "Tripler",
                CardModifier.Favourite => "Favourite",
                CardModifier.Swapper => "Swapper",
                CardModifier.Duplicator => "Duplicator",
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
            CardType.TrueJoker => "True Joker",
            CardType.Pedometer => "Pedometer",
            CardType.MultiValue => "Valuenator",
            CardType.Ace => "Ace",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private string GetNameSuffix()
    {
        return number is >= 0 and <= 10 ? GetNumberAsText() : "Thing";
    }

    private string GetNumberAsText()
    {
        var names = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
        return number is >= 0 and <= 10 ? names[number] : number.ToString();
    }

    public List<TooltipExtra> GetExtras()
    {
        return type switch
        {
            CardType.Kill => new List<TooltipExtra> { TooltipExtra.Strike },
            CardType.Mox => new List<TooltipExtra> { TooltipExtra.Basic },
            CardType.TrueJoker => new List<TooltipExtra> { TooltipExtra.Joker },
            _ => new List<TooltipExtra>()
        };
    }
    
    public string GetDescription(int value = 0)
    {
        if (modifier != CardModifier.None)
        {
            var scoreVerb = number == 2 ? "Doubles" : "Triples";
            
            return modifier switch
            {
                CardModifier.None => "",
                CardModifier.Plus => "(Adds) the value to (other card).",
                CardModifier.Minus => "(Subtracts) the value from (other card).",
                CardModifier.Multiply => "(Multiplies) other (card value) with this value.",
                CardModifier.Cheat => ExtraInfo.GetDescription(TooltipExtra.Cheat),
                CardModifier.Scorer => $"{scoreVerb} the (score value) of the selected card." ,
                CardModifier.Favourite => "Mark the (selected card) as (favourite). Favourite cards are placed on (the top) of the deck after (shuffling).",
                CardModifier.Swapper => "Swap the (value) and (score multiplier) values of the selected card.",
                CardModifier.Duplicator => "Add a (copy) of the selected (card) to your deck.",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        var jokerAddition = type == CardType.Joker && State.Instance.Has(Effect.JokerSight) ? $"\n\nThe (value) is currently ({value})." : "";

        return type switch
        {
            CardType.Normal => "",
            CardType.Joker => ExtraInfo.GetDescription(TooltipExtra.Joker) + jokerAddition,
            CardType.Timer => "Changes the (value) of the card to reflect the (current time).",
            CardType.Recall => "Draw (three extra) cards. Resets the (multiplier) afterwards.",
            CardType.Averager => "Change (all visible) card values to their (total average).",
            CardType.Kill => ExtraInfo.GetDescription(TooltipExtra.Death),
            CardType.Lotus => "Instantly gain (30 points) for each card in your hand. Resets the (multiplier) afterwards.",
            CardType.Mox => ExtraInfo.GetDescription(TooltipExtra.Gem),
            CardType.TrueJoker => "Not like other (jokers). The (value) is always what it (needs to be).",
            CardType.Pedometer => "Permanently (increase the value) of this (card) by one every time you (play) it.",
            CardType.MultiValue => "The (value) of this (card) is always equal to your (multiplier).",
            CardType.Ace => $"Just like a normal ({GetNumberAsText().ToLower()}) card but (increases) your (multiplier) when played.",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static CardData GetRandomGem()
    {
        return new CardData(Random.Range(1, 6))
        {
            icon = 9,
            multiplier = Random.Range(2, 6) + State.Instance.GetCount(Effect.Miner),
            type = CardType.Mox
        };
    }

    public static CardData GetBasic()
    {
        return State.Instance.Has(Effect.Gemology) ? GetRandomGem() : new CardData(Random.Range(1, 11));
    }
}