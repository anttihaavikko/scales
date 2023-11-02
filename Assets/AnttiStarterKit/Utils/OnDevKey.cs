using System;
using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Utils
{
    public class OnDevKey : MonoBehaviour
    {
        [SerializeField] private KeyCode key;
        [SerializeField] private UnityEvent action;

        private void Update()
        {
            if (DevKey.Down(key))
            {
                action?.Invoke();
            }
        }
    }
}