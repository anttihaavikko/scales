using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnttiStarterKit.Utils
{
    public class DevReload : MonoBehaviour
    {
        private void Update()
        {
            if (!Application.isEditor) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneChanger.Instance.ChangeScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}