using System.Collections.Generic;
using System.Text;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject node;
    [SerializeField] private TMP_Text titleField, contentField;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private List<ExtraInfo> extraFields;
    [SerializeField] private Transform leftEdge, rightEdge;

    private object shown;

    public void Show(Skill skill, Vector3 pos)
    {
        shown = skill;
        node.SetActive(true);
        Show(skill.title, skill.description);
        AddExtras(skill.extras);
        Fix();
        Reposition(pos);
    }

    private void AddExtras(List<TooltipExtra> extras)
    {
        var i = 0;
        extraFields.ForEach(e =>
        {
            var has = extras.Count > i;
            e.gameObject.SetActive(has);
            if (has) e.Show(extras[i]);
            i++;
        });
    }

    private void Reposition(Vector3 pos)
    {
        var refPos = leftEdge.position;
        var flipped = pos.y > refPos.y;
        transform.position = pos.WhereX(Mathf.Clamp(pos.x, refPos.x, rightEdge.position.x));
        rectTransform.pivot = new Vector2(0.5f, flipped ? 1 : 0);
        rectTransform.anchoredPosition = new Vector2(0, flipped ? 20 : 100);
    }

    public void Show(Card card, Vector3 pos)
    {
        if (!card.HasTooltip) return;
        shown = card;
        Show(card.Title, card.Description);
        node.SetActive(true);
        AddExtras(card.Extras);
        Fix();
        Reposition(pos);
    }

    private void Show(string title, string text)
    {
        titleField.text = title.ToUpper();
        contentField.text = Decorate(text);
    }

    public static string Decorate(string text)
    {
        var sb = new StringBuilder(text);
        sb.Replace("(", "<color=#BE6E46>");
        sb.Replace(")", "</color>");
        return sb.ToString();
    }

    private void Fix()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void Hide(object sender = default)
    {
        if (shown == sender || sender == default)
        {
            node.SetActive(false);
            shown = default;
        }
    }
}