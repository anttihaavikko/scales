using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;

using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class ColorThemeHelper : EditorWindow
    {
        private List<ThemeGroup> groups;

        [MenuItem("Window/Color theme helper")]
        private static void ShowWindow()
        {
            var window = GetWindow<ColorThemeHelper>();
            window.titleContent = new GUIContent("Color theme helper");
            window.Show();
        }

        private void OnEnable()
        {
            groups ??= new List<ThemeGroup>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Add new group"))
            {
                var group = new ThemeGroup();
                AddObjectsToGroup(group);
                groups.Add(group);
            }

            foreach (var group in groups)
            {
                EditorGUILayout.Separator();
                DrawGroup(group);
            }

            groups.RemoveAll(g => g.removed);
        }

        private void DrawGroup(ThemeGroup group)
        {
            EditorGUILayout.LabelField($"Group of {group.ObjectCount}");
            
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select"))
            {
                Selection.objects = group.objects.ToArray();
            }
            
            if (GUILayout.Button("Set"))
            {
                group.objects.Clear();
                group.prefabObjects.Clear();
                AddObjectsToGroup(group);
            }
            
            if (GUILayout.Button("Add"))
            {
                AddObjectsToGroup(group);
            }

            if (GUILayout.Button("Remove"))
            {
                group.removed = true;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.BeginChangeCheck();

            group.color = EditorGUILayout.ColorField(group.color);
            
            if (EditorGUI.EndChangeCheck())
            {
                AssignColor(group);
            }
        }

        private static void AddObjectsToGroup(ThemeGroup group)
        {
            foreach (var go in Selection.gameObjects)
            {
                var prefabMode = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabMode)
                {
                    var data = new PrefabObject(prefabMode.assetPath, GetPath(go));
                    group.prefabObjects.Add(data);
                    continue;
                }
                
                group.objects.Add(go);
            }
        }

        private void AssignColor(ThemeGroup group)
        {
            var color = group.color;
            
            foreach(var obj in group.objects)
            {
                if (obj)
                {
                    AssignColor(obj, color);   
                }
            }
            
            foreach (var data in group.prefabObjects)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(data.asset);
                var t = FindNext(go.transform, WithoutFirst(data.path));
                AssignColor(t.gameObject, group.color);
                AssetDatabase.SaveAssets();
            }
        }

        private static string WithoutFirst(string path)
        {
            return path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1);
        }

        private static Transform FindNext(Transform t, string path)
        {
            if (path.Contains("/"))
            {
                var first = path.Split('/')[0];
                return FindNext(t.Find(first), WithoutFirst(path));
            }
            
            return t.Find(path);
        }

        private static void AssignColor(GameObject obj, Color color)
        {
            // sprite
            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite) sprite.color = color;

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

        private static string GetPath(GameObject obj)
        {
            var path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path.Substring(1);
        }
    }

    [Serializable]
    internal class ThemeGroup
    {
        public List<GameObject> objects;
        public Color color = Color.white;
        public bool removed;
        public List<PrefabObject> prefabObjects;

        public int ObjectCount => objects.Count + prefabObjects.Count;

        public ThemeGroup()
        {
            objects = new List<GameObject>();
            prefabObjects = new List<PrefabObject>();
        }
    }

    [Serializable]
    internal class PrefabObject
    {
        public string asset;
        public string path;

        public PrefabObject(string asset, string path)
        {
            this.asset = asset;
            this.path = path;
        }
    }
}