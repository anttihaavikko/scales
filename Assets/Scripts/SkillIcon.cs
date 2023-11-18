using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon, shadow;
    [SerializeField] private Pulsater pulsater;

    private Tooltip tooltip;
    private Skill skill;

    public void Setup(Skill s, Tooltip tt)
    {
        icon.sprite = shadow.sprite = s.icon;
        skill = s;
        tooltip = tt;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.2f;
        tooltip.Show(skill, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        tooltip.Hide();
    }

    public void Pulsate(Effect effect)
    {
        if (skill.effect == effect)
        {
            pulsater.Pulsate();
        }
    }
    
    public void Pulsate(Effect effect, int value)
    {
        if (skill.effect == effect && skill.value == value)
        {
            pulsater.Pulsate();
        }
    }
}