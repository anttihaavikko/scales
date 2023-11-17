using System;
using System.Linq;
using UnityEngine;

public class SkillIcons : MonoBehaviour
{
    [SerializeField] private SkillIcon prefab;
    [SerializeField] private Tooltip tooltip;
    [SerializeField] private int zoomLimit = 32;
    
    private int count;

    private void Start()
    {
        State.Instance.Skills.ToList().ForEach(Add);
    }

    public void Add(Skill skill)
    {
        var icon = Instantiate(prefab, transform);
        icon.Setup(skill, tooltip);
        count++;

        if (count > zoomLimit)
        {
            transform.localScale = Vector3.one * 0.5f;
        }
    }
}