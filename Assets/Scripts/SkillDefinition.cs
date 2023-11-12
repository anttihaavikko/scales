using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill", order = 0)]
public class SkillDefinition : ScriptableObject
{
    [SerializeField] private Skill data;

    public string Title => data.title;

    public Skill Spawn()
    {
        return data;
    }

    public bool CanGet(IEnumerable<Skill> existing)
    {
        return existing.Count(s => s.title == Title) <= data.allowedRepeats;
    }
}

public enum Effect
{
    None,
    Cheater,
    Heal,
    Shield,
    Fiver,
    JokerHeal,
    HandSize,
    MoreOptions,
    Modifiers,
    Basics
}

[Serializable]
public struct Skill
{
    public Effect effect;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    public int allowedRepeats;
    public List<TooltipExtra> extras;
}