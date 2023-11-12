using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Visuals;
using UnityEngine;

public class StrikeBox : MonoBehaviour
{
    [SerializeField] private Appearer box, cross;

    private EffectCamera cam;

    private void Start()
    {
        cam = Camera.main.GetComponent<EffectCamera>();
    }

    public void Show(bool filled = false)
    {
        box.Show();
        if (filled) Fill();
    }

    public void FillAndShake()
    {
        cam.BaseEffect(0.3f);
        cross.Show();
    }

    public void Fill()
    {
        cross.Show();
    }

    public void Clear()
    {
        cross.Hide();
    }
}