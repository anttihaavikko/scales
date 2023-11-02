using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class StyledText : MonoBehaviour
{
    [SerializeField] private float amount = 0.01f;
    [SerializeField] private float speed = 2f;

    [TextArea][SerializeField] private string textOnStart;
    

    private TMP_Text _textField;

    private readonly List<TextEffect> _effects = new List<TextEffect>();

    private void Awake()
    {
        _textField = GetComponent<TMP_Text>();

        if (!string.IsNullOrEmpty(textOnStart))
        {
            SetText(textOnStart);
        }
    }

    public void SetText(string text)
    {
        _effects.Clear();
        
        var stripped = StripTags(text);
        var plain = StripEffectTags(stripped);

        foreach (var effectType in EnumUtils.ToList<EffectType>())
        {
            AddEffects(stripped, plain, effectType);    
        }

        _textField.text = StripEffectTags(text);
    }
    
    private void OnEnable()
    {
        _textField.richText = false;
        this.StartCoroutine(() => _textField.richText = true, 0.1f);
    }

    private void AddEffects(string text, string plain, EffectType type)
    {
        var pos = 0;
        var safety = 0;
        var tagName = GetEffectTag(type);
        var len = tagName.Length;

        if (!text.Contains($"<{tagName}>")) return;
        
        while (pos < text.Length)
        {
            var start = text.IndexOf($"<{tagName}>", pos, StringComparison.Ordinal) + len + 2;
            if (start < 0) break;
            var end = text.IndexOf($"</{tagName}>", start, StringComparison.Ordinal);
            if (end < 0) break;

            var inside = text.Substring(start, end - start);
            var before = text.Substring(0, start);
            var countBefore = Regex.Matches(before, inside).Count;

            
            var posInPlain = countBefore == 0 ? 
                plain.IndexOf(inside, StringComparison.Ordinal) : 
                plain.NthIndexOf(inside, countBefore + 1);

            if (posInPlain < 0) break;
            
            // Debug.Log($"Inside word is: {inside}, pos {posInPlain} => {plain.Substring(posInPlain, inside.Length)}");

            _effects.Add(new TextEffect(type, posInPlain, posInPlain + inside.Length));
            pos = end;
            
            safety++;
            if (safety > 100) break;
        }
    }

    private static string GetEffectTag(EffectType type)
    {
        return type switch
        {
            EffectType.Wobble => "wobble",
            EffectType.Bounce => "bounce",
            EffectType.Shake => "shake",
            EffectType.Bulge => "bulge",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static string StripTags(string text)
    {
        text = RemoveRichTextDynamicTag(text, "color");
        return text;
    }

    private static string StripEffectTag(string text, string effect)
    {
        return RemoveRichTextTag(text, effect);
    }
    
    private static string StripEffectTags(string text)
    {
        foreach (var effectType in EnumUtils.ToList<EffectType>())
        {
            text = RemoveRichTextTag(text, GetEffectTag(effectType));
        }
        
        return text;
    }

    private static string RemoveRichTextDynamicTag (string input, string tag)
    {
        var index = -1;
        while (true)
        {
            index = input.IndexOf($"<{tag}=");
            //Debug.Log($"{{{index}}} - <noparse>{input}");
            if (index != -1)
            {
                int endIndex = input.Substring(index, input.Length - index).IndexOf('>');
                if (endIndex > 0)
                    input = input.Remove(index, endIndex + 1);
                continue;
            }
            input = RemoveRichTextTag(input, tag, false);
            return input;
        }
    }
    private static string RemoveRichTextTag (string input, string tag, bool isStart = true)
    {
        while (true)
        {
            var index = input.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>");
            if (index != -1)
            {
                input = input.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
                continue;
            }
            if (isStart)
                input = RemoveRichTextTag(input, tag, false);
            return input;
        }
    }

    private void Update()
    {
        if (_effects.Count == 0) return;
        
        _textField.ForceMeshUpdate();
        
        var mesh = _textField.mesh;
        var verts = mesh.vertices;

        foreach (var effect in _effects)
        {
             effect.Apply(_textField, ref verts, speed, amount);
        }

        mesh.vertices = verts;

        if (_textField.canvasRenderer)
        {
            _textField.canvasRenderer.SetMesh(mesh);
        }
    }
}

public class TextEffect
{
    private readonly int _start;
    private readonly int _end;
    private readonly EffectType _type;

    public TextEffect(EffectType type, int start, int end)
    {
        _type = type;
        _start = start;
        _end = end;
        
        // Debug.Log($"Adding effect from {start} to {end}");
    }

    public void Apply(TMP_Text field, ref Vector3[] verts, float speed, float amount)
    {
        for (var i = _start; i < _end; i++)
        {
            if (_start < _end && _start > 0 && _end > 0)
            {
                OffsetCharacter(i, field.textInfo.characterInfo[i], ref verts, speed, amount);   
            }
        }
    }
    
    private void OffsetCharacter(int index, TMP_CharacterInfo info, ref Vector3[] verts, float speed, float amount)
    {
        if (_type == EffectType.Bulge)
        {
            var len = Mathf.Sin(Time.time * speed * 0.75f + index * 0.3f) * amount * 0.3f;
            verts[info.vertexIndex] += new Vector3(-len, -len, 0);
            verts[info.vertexIndex + 1] += new Vector3(-len, len, 0);
            verts[info.vertexIndex + 2] += new Vector3(len, len, 0);
            verts[info.vertexIndex + 3] += new Vector3(len, -len, 0);
            return;
        }
        
        var offset = GetOffset(index, speed, amount);
        verts[info.vertexIndex] += offset;
        verts[info.vertexIndex + 1] += offset;
        verts[info.vertexIndex + 2] += offset;
        verts[info.vertexIndex + 3] += offset;
    }

    private Vector3 GetOffset(int index, float speed, float amount)
    {
        return _type switch
        {
            EffectType.Wobble => Vector3.zero.WhereY(Mathf.Sin(Time.time * speed + index * 0.5f) * amount),
            EffectType.Bounce => Vector3.zero.WhereY(Mathf.Abs(Mathf.Sin(Time.time * speed + index * 0.5f)) * amount),
            EffectType.Shake => Vector3.zero.WhereY(Random.Range(-amount, amount)),
            EffectType.Bulge => Vector3.zero, // not needed
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum EffectType
{
    Wobble,
    Bounce,
    Shake,
    Bulge
}