using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    public void Setup(Skill skill)
    {
        icon.sprite = skill.icon;
    }
}