using System;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField] private Appearer bg, level, title;

    private void Start()
    {
        level.text.text = $"LEVEL {State.Instance.Level + 1}";
        Invoke(nameof(Hide), 2.5f); 
    }

    private void Hide()
    {
        bg.HideWithDelay(0.4f);
        level.HideWithDelay(0.1f);
        title.HideWithDelay(0.2f);  
    }
}