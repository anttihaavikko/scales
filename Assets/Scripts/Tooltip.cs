using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject node;
    [SerializeField] private TMP_Text titleField, contentField;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private List<ExtraInfo> extraFields;
    
    private object shown;

    public void Show(Skill skill, Vector3 pos)
    {
        shown = skill;
        node.SetActive(true);
        Show(skill.title, skill.description);
        Fix();
        transform.position = pos;

        var i = 0;
        extraFields.ForEach(e =>
        {
            var has = skill.extras.Count > i;
            e.gameObject.SetActive(has);
            if (has) e.Show(skill.extras[i]);
            i++;
        });
    }
    
    public void Show(Card card, Vector3 pos)
    {
        if (!card.HasTooltip) return;
        shown = card;
        Show(card.Title, card.Description);
        node.SetActive(true);
        Fix();
        transform.position = pos;
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