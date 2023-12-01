using System;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class HideOnWeb : MonoBehaviour
    {
        private void Start()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                gameObject.SetActive(false);
            }
        }
    }
}