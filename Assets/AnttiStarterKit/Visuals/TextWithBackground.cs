using System;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace AnttiStarterKit.Visuals
{
    [ExecuteInEditMode]
    public class TextWithBackground : MonoBehaviour
    {
        [SerializeField] private Color color = Color.black;
        [SerializeField] private int paddingLeft = 10, paddingRight = 10, paddingTop = 10, paddingBottom = 10;
        
        [SerializeField] private TMP_Text original;
        [SerializeField] private TextMeshProUGUI inner;

        private void OnEnable()
        {
            Setup();
        }

        private void Setup()
        {
            if (inner) return;
            
            var o = new GameObject("Inner Text", typeof(RectTransform));
            o.transform.parent = transform;
            var rt = o.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            o.transform.position = transform.position;
            inner = o.AddComponent<TextMeshProUGUI>();
            original = GetComponent<TMP_Text>();
            inner.font = original.font;
            inner.color = original.color;
            inner.text = original.text;
            inner.fontSize = original.fontSize;
            inner.alignment = original.alignment;
            inner.transform.localScale = Vector3.one;

            var wobble = original.GetComponent<WobblingText>();
            if (wobble)
            {
                var innerWobble = o.AddComponent<WobblingText>();
                innerWobble.Clone(wobble);
            }

            SetText(original.text);
        }

        public void SetText(string text)
        {
            Setup();
            var hex = ColorUtility.ToHtmlStringRGB(color);
            original.text = $"<mark=#{hex} padding='{paddingLeft}, {paddingRight}, {paddingTop}, {paddingBottom}'>{text}</mark>";
            inner.text = text;
        }
    }
}