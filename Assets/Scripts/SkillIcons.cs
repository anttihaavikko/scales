using System;
using System.Linq;
using UnityEngine;

public class SkillIcons : MonoBehaviour
{
    [SerializeField] private SkillIcon prefab;

    private void Start()
    {
        State.Instance.Skills.ToList().ForEach(Add);
    }

    public void Add(Skill skill)
    {
        var icon = Instantiate(prefab, transform);
        icon.Setup(skill);
    }
}