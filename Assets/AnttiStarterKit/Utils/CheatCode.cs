using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Utils
{
    public class CheatCode : MonoBehaviour
    {
        [SerializeField] private string code;
        [SerializeField] private UnityEvent action;

        private readonly Queue<string> letters = new();

        private void Update()
        {
            foreach (var c in Input.inputString)
            {
                letters.Enqueue(c.ToString());
            }

            if (letters.Count > code.Length)
            {
                letters.Dequeue();
            }

            if (string.Equals(code, string.Join(string.Empty, letters), StringComparison.CurrentCultureIgnoreCase))
            {
                action?.Invoke();
            }
        }
    }
}