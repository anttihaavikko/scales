using AnttiStarterKit.Extensions;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class SceneChangerObject : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void Quit()
    {
        SceneChanger.Instance.blinders.Close();
        this.StartCoroutine(() => Application.Quit(), 1f);
    }
}
