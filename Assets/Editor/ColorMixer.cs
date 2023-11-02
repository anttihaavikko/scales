using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ColorMixer : EditorWindow
    {
        private Color firstColor, secondColor;
            
        [MenuItem("Window/ColorMixer")]
        private static void ShowWindow()
        {
            var window = GetWindow<ColorMixer>();
            var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/Icons/palette.png");
            window.titleContent = new GUIContent("Color mixer", icon);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Select colors to blend");
            firstColor = EditorGUILayout.ColorField(firstColor);
            secondColor = EditorGUILayout.ColorField(secondColor);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Add");
            EditorGUILayout.ColorField(firstColor + secondColor);
            EditorGUILayout.LabelField("Multiply");
            EditorGUILayout.ColorField(firstColor * secondColor);
            EditorGUILayout.LabelField("Subtract");
            EditorGUILayout.ColorField(firstColor - secondColor);
        }
    }
}