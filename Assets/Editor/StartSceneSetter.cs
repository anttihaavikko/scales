using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StartSceneSetter : EditorWindow
{
    void OnGUI()
    {
        // Use the Object Picker to select the start SceneAsset
        EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"),
            EditorSceneManager.playModeStartScene,
            typeof(SceneAsset),
            false);
    }

    [MenuItem("Scenes/Open")]
    static void Open()
    {
        GetWindow<StartSceneSetter>();
    }
}