using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill", order = 0)]
public class SkillDefinition : ScriptableObject
{
    [ScriptableObjectId] [SerializeField] private string id;
    [SerializeField] private Skill data;

    public string Title => data.title;

    public Skill Spawn()
    {
        var d = data;
        d.id = id;
        return d;
    }

    public bool CanGet(IEnumerable<Skill> existing)
    {
        return existing.Count(s => s.id == id) <= data.allowedRepeats;
    }
}

public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        GUI.enabled = false;
        if (string.IsNullOrEmpty(property.stringValue)) {
            property.stringValue = Guid.NewGuid().ToString();
        }
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

public enum Effect
{
    None,
    Cheater,
    Heal,
    Shield,
    Fiver
}

[Serializable]
public struct Skill
{
    [HideInInspector] public string id;
    public Effect effect;
    public string title;
    [TextArea] public string description;
    public Sprite icon;
    public int allowedRepeats;
}