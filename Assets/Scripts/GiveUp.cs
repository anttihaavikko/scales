using System;
using AnttiStarterKit.Animations;
using UnityEngine;

public class GiveUp : MonoBehaviour
{
    [SerializeField] private GameMode mode;
    
    private Appearer appearer;
    private bool shown;

    private void Start()
    {
        appearer = GetComponent<Appearer>();
    }

    private void Update()
    {
        if (mode.HasEnded) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shown) Hide();
            else Show();
        }
    }

    public void Show()
    {
        shown = true;
        appearer.Show();
    }

    public void Hide()
    {
        shown = false;
        appearer.Hide();
    }

    public void End()
    {
        Hide();
        mode.Ended();
    }
}