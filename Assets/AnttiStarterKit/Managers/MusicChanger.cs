using System;
using UnityEngine;

namespace AnttiStarterKit.Managers
{
    public class MusicChanger : MonoBehaviour
    {
        [SerializeField] private int musicIndex;
        [SerializeField] private float fadeIn = 1f;
        [SerializeField] private float fadeOut = 0.5f;
        [SerializeField] private float delay = 0.5f;
        
        
        private void Start()
        {
            AudioManager.Instance.Highpass(false);
            AudioManager.Instance.Lowpass(false);
            AudioManager.Instance.ChangeMusic(musicIndex, fadeIn, fadeOut, delay);
        }
    }
}