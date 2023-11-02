using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PlayerPrefsPreviewer : EditorWindow
    {
        private string _display = "No property selected";
        private string _propertyName;
        private Vector2 _scrollPos;

        [MenuItem("Window/PlayerPrefs Previewer")]
        private static void ShowWindow()
        {
            var window = GetWindow<PlayerPrefsPreviewer>();
            window.titleContent = new GUIContent("PlayerPrefs Previewer");
            window.Show();
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey("PreviewedProperty"))
            {
                _propertyName = EditorPrefs.GetString("PreviewedProperty");
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (!PlayerPrefs.HasKey(_propertyName))
            {
                _display = "No property found!";
                return;
            }
            
            _display = PlayerPrefs.GetString(_propertyName);

            EditorPrefs.SetString("PreviewedProperty", _propertyName);
        }

        private void OnGUI()
        {
            var style = new GUIStyle
            {
                wordWrap = true,
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                }
            };
            
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });
            
            EditorGUILayout.BeginHorizontal();

            _propertyName = EditorGUILayout.TextField(_propertyName);
            
            if (GUILayout.Button("Show"))
            {
                UpdateDisplay();
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);
            EditorGUILayout.LabelField(_display, style);
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();   
        }
    }
}