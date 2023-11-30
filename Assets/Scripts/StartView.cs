using System;
using AnttiStarterKit.Managers;
using UnityEngine;

public class StartView : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.ChangeMusic(0);
    }

    public void Play()
    {
        State.Instance.Reset();
    }

    public void BackToStart()
    {
        AudioManager.Instance.TargetPitch = 1f;
        SceneChanger.Instance.ChangeScene("Start");
    }
}