using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text content;
    [SerializeField] private RectTransform rectTransform;

    public void Show(string text)
    {
        content.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}