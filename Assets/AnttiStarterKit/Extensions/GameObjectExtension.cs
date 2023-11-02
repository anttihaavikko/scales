using UnityEngine;

namespace AnttiStarterKit.Extensions
{
    public static class GameObjectExtension
    {
        public static void ToggleActive(this GameObject gameObject)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}