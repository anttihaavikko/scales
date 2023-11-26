using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.Visuals;
using UnityEngine;

public class StrikeBox : MonoBehaviour
{
    [SerializeField] private Appearer box, cross;

    private EffectCamera cam;
    private Camera rawCam;

    private void Start()
    {
        rawCam = Camera.main;
        cam = rawCam.GetComponent<EffectCamera>();
    }

    public void Show(bool filled = false)
    {
        box.Show();
        if (filled) Fill();
    }

    public void FillAndShake()
    {
        var p = rawCam.ScreenToWorldPoint(transform.position).WhereY(0);
        AudioManager.Instance.PlayEffectFromCollection(7, p);
        AudioManager.Instance.PlayEffectFromCollection(6, p, 1.2f);
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