using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class PaletteAssigner : EditorWindow
    {
        private List<Color> _palette;
        private Color _newColor = Color.white;
        private string _newHex = "#ffffff";

        private static string SaveFile => "Assets/Editor/palette.json";

        [MenuItem("Window/Palette")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(PaletteAssigner));
            window.minSize = new Vector2(30f, 50f);
            var icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Editor/Icons/palette.png");
            var titleContent = new GUIContent("Palette", icon);
            window.titleContent = titleContent;
        }

        private void OnEnable()
        {
            _palette = new List<Color>();
            LoadPalette();
        }

        private void OnGUI()
        {
            var altHeld = Event.current.alt;
            var shiftHeld = Event.current.shift;
            
            EditorGUILayout.BeginVertical(new GUIStyle
            {
                padding = new RectOffset(10, 10, 5, 5)
            });

            EditorGUILayout.LabelField("Assign color");
            EditorGUILayout.BeginHorizontal();
            var index = 0;
            var perRow = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth - 20)/ 50);

            var removeIndex = -1;

            foreach (var color in _palette)
            {
                var style = new GUIStyle
                {
                    fontSize = 40,
                    stretchWidth = false,
                    padding = new RectOffset(5, 5, 0, 0)
                };
                var hex = ColorUtility.ToHtmlStringRGB(color);
                if (GUILayout.Button("<color=#"+hex+">█</a>", style))
                {
                    if (altHeld)
                    {
                        removeIndex = index;
                    }
                    else if (shiftHeld)
                    {
                        EditorGUIUtility.systemCopyBuffer = hex;
                    }
                    else
                    {
                        AssignColor(color);
                    }
                }

                index++;

                if (index % perRow == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }
            EditorGUILayout.EndHorizontal();

            if(removeIndex > -1)
            {
                _palette.RemoveAt(removeIndex);
                SavePalette();
            }
            
            EditorGUILayout.EndVertical();

            ShowControls();
        }

        private void ShowControls()
        {
            EditorGUILayout.BeginVertical(new GUIStyle
            {
                padding = new RectOffset(10, 10, 30, 5)
            });

            EditorGUILayout.HelpBox("Alt+click to delete.\nShift+click to copy hex.", MessageType.Info);
            EditorGUILayout.Space();

            ShowAddColor();

            EditorGUILayout.Space();

            ShowAddHex();

            EditorGUILayout.EndVertical();
        }

        private void ShowAddHex()
        {
            EditorGUILayout.LabelField("Add color with hex");
            EditorGUILayout.BeginHorizontal();
            _newHex = EditorGUILayout.TextField(_newHex);
            EditorGUILayout.Space();
            if (GUILayout.Button("Add"))
            {
                if (!_newHex.StartsWith("#")) _newHex = "#" + _newHex;

                if (ColorUtility.TryParseHtmlString(_newHex, out var fromHex))
                {
                    _palette.Add(fromHex);
                    SavePalette();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowAddColor()
        {
            EditorGUILayout.LabelField("Add color");
            EditorGUILayout.BeginHorizontal();
            _newColor = EditorGUILayout.ColorField(_newColor);
            EditorGUILayout.Space();
            if (GUILayout.Button("Add"))
            {
                _palette.Add(_newColor);
                SavePalette();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void LoadPalette()
        {
            var key = GetKeyName();
            if (File.Exists(SaveFile))
            {
                // var json = EditorPrefs.GetString(key);
                var json = File.ReadAllText(SaveFile);
                var data = JsonUtility.FromJson<Palette>(json);
                _palette = data.ToList();

            }
            else
            {
                _palette.Add(Color.black);
                _palette.Add(Color.white);
            }
        }

        private void SavePalette()
        {
            var data = new Palette(_palette);
            var json = JsonUtility.ToJson(data);
            // EditorPrefs.SetString(GetKeyName(), json);
            File.WriteAllText(SaveFile, json);
        }

        private static string GetKeyName()
        {
            var dp = Application.dataPath;
            var s = dp.Split("/"[0]);
            return s[s.Length - 2] + "SavedPalette";
        }

        private static void AssignColor(Color color)
        {
            foreach(var obj in Selection.gameObjects)
            {
                // sprite
                var sprite = obj.GetComponent<SpriteRenderer>();
                if(sprite) sprite.color = color;

                // textmeshpro
                var text = obj.GetComponent<TMPro.TMP_Text>();
                if (text) text.color = color;

                // ui image
                var image = obj.GetComponent<Image>();
                if (image) image.color = color;
                
                // camera
                var cam = obj.GetComponent<Camera>();
                if (cam) cam.backgroundColor = color;
                
                EditorUtility.SetDirty(obj);
            }
        }
    }

    [System.Serializable]
    public class Palette
    {
        public List<string> colors;

        public Palette(List<Color> values)
        {
            colors = values.Select(ColorUtility.ToHtmlStringRGB).ToList();
        }

        public List<Color> ToList()
        {
            return colors.Select(c => {
                ColorUtility.TryParseHtmlString("#" + c, out var parsed);
                return parsed;
            }).ToList();
        }
    }
}