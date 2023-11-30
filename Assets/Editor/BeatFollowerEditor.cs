using AnttiStarterKit.Music;
using UnityEditor;
using UnityEngine;

namespace AnttiStarterKit.Editor
{
    [CustomEditor(typeof(BeatFollower))]
    public class BeatFollowerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            var script = (BeatFollower)target;
            
            var rect = GUILayoutUtility.GetLastRect();

            var style = new GUIStyle("button")
            {
                fontSize = 10,
                stretchWidth = false,
                stretchHeight = false,
                fixedWidth = (Screen.width * 0.5f - 30 - script.Divisions * 5) / script.Divisions,
                fixedHeight = 20,
                margin = new RectOffset(0, 5, 5, 0),
                padding = new RectOffset(0, 0, 0, 0),
                richText = true,
                border = new RectOffset(0, 0, 0, 0)
            };

            var buttonStyle = new GUIStyle("button")
            {
                stretchWidth = false,
                padding = new RectOffset(10, 15, 5, 5),
                margin = new RectOffset(0, 0, 10, 0)
            };

            var index = 0;

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            var toggled = -1;

            foreach (var b in script.Pattern)
            {
                var tile = b ? "<color=white>█</color>" : "<color=black>█</color>";
                if (GUILayout.Button(tile, style))
                {
                    toggled = index;
                }
                
                index++;

                if (index % script.Divisions == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            if (toggled >= 0)
            {
                script.Toggle(toggled);
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (GUILayout.Button("+ Add bar", buttonStyle))
            {
                script.AddBar();
            }
        }
    }
}