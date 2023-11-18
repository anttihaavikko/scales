using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillIcons : MonoBehaviour
{
    [SerializeField] private SkillIcon prefab;
    [SerializeField] private Tooltip tooltip;
    [SerializeField] private int zoomLimit = 32;
    
    private readonly List<SkillIcon> icons = new();

    private void Start()
    {
        State.Instance.SkillIcons = this;
        State.Instance.Skills.ToList().ForEach(Add);
    }

    public void Add(Skill skill)
    {
        var icon = Instantiate(prefab, transform);
        icons.Add(icon);
        icon.Setup(skill, tooltip);

        if (icons.Count > zoomLimit)
        {
            transform.localScale = Vector3.one * 0.5f;
        }
    }

    public void Trigger(Effect skill)
    {
        icons.ForEach(i => i.Pulsate(skill));
    }
    
    public void Trigger(Effect skill, int value)
    {
        icons.ForEach(i => i.Pulsate(skill, value));
    }
}