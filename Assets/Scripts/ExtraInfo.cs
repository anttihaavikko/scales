using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExtraInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text title, description;
    [SerializeField] private RectTransform rectTransform;

    public void Show(TooltipExtra extra)
    {
        title.text = GetTitle(extra);
        description.text = Tooltip.Decorate(GetDescription(extra));
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private static string GetTitle(TooltipExtra extra)
    {
        return extra switch
        {
            TooltipExtra.Cheat => "Cheat",
            TooltipExtra.Strike => "Strike",
            TooltipExtra.Joker => "Joker",
            TooltipExtra.Basic => "Basic cards",
            TooltipExtra.Modifier => "Modifiers",
            _ => throw new ArgumentOutOfRangeException(nameof(extra), extra, null)
        };
    }
    
    public static string GetDescription(TooltipExtra extra)
    {
        return extra switch
        {
            TooltipExtra.Cheat => "Discreetly (mark) the value of the card on the (back side).",
            TooltipExtra.Strike => "Mark of a (failure). If you get (three), you lose.",
            TooltipExtra.Joker => "The (value) of joker is equal to the (sum of all) other (visible cards).",
            TooltipExtra.Basic => "The most (basic) of cards with (value ranging) from (0 to 10).",
            TooltipExtra.Modifier => "Cards that (manipulate) other (chosen cards).",
            _ => throw new ArgumentOutOfRangeException(nameof(extra), extra, null)
        };
    }
}

public enum TooltipExtra
{
    Cheat,
    Strike,
    Joker,
    Basic,
    Modifier
}