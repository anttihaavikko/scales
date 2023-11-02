using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnttiStarterKit.Music
{
    public class Music : MonoBehaviour
    {
        [SerializeField] private int bpm = 100;
        [SerializeField] private List<AudioSource> musicLayers;
        [SerializeField] private AudioSource noise;
        [SerializeField] private List<AudioClip> impacts;
        [SerializeField] private float impactVolume = 1f;

        public Action onBeat, onStrongBeat, onWeakBeat;

        private readonly List<AudioSource> activateOnNext = new();
        private readonly List<AudioSource> deactivateOnNext = new();

        private bool rising;
        private float timeLeft;
        private float noiseStart;
        private float noiseVolume;

        private int beat;

        private float FourBarLength => 60f / bpm * 4 * 4;

        private void Start()
        {
            noiseVolume = noise.volume;
            StopAll();
            PlayImmediately(new [] { "drums" });
            Invoke(nameof(OnFourBar), FourBarLength);
            timeLeft = FourBarLength;
            OnBeat();
        }

        private void Update()
        {
            if (rising)
            {
                noise.volume = Mathf.Clamp01(1 - timeLeft / noiseStart) * noiseVolume;
            }
            
            timeLeft -= Time.deltaTime;
        }

        private void OnBeat()
        {
            onBeat?.Invoke();
            if(beat is 0 or 2) onStrongBeat?.Invoke();
            if(beat is 1 or 3) onWeakBeat?.Invoke();
            
            beat = (beat + 1) % 4;
            Invoke(nameof(OnBeat), 60f / bpm);
        }

        private void OnFourBar()
        {
            timeLeft = FourBarLength;
            
            activateOnNext.ForEach(a => a.volume = 1);
            deactivateOnNext.ForEach(a => a.volume = 0);
            activateOnNext.Clear();
            deactivateOnNext.Clear();
            Invoke(nameof(OnFourBar), FourBarLength);

            if (!rising) return;

            if (impacts.Any())
            {
                var p = Camera.main.transform.position.WhereZ(0);
                // AudioSource.PlayClipAtPoint(impacts.Random(), p, impactVolume);
                AudioManager.Instance.PlayEffectAt(impacts.Random(), p, impactVolume, false);
            }
            
            rising = false;
            noise.Stop();
            noise.volume = 0;
        }

        private void StopAll()
        {
            musicLayers.ForEach(l => l.volume = 0);
        }

        public void PlayImmediately(ICollection<string> layers)
        {
            musicLayers.ForEach(l => l.volume = layers.Contains(l.name) ? 1 : 0);
        }

        public void Play(ICollection<string> layers)
        {
            activateOnNext.AddRange(musicLayers.Where(l => layers.Contains(l.name)));
            deactivateOnNext.AddRange(musicLayers.Where(l => !layers.Contains(l.name)));
            Sweep();
        }

        private void Sweep()
        {
            rising = true;
            noiseStart = timeLeft;
            noise.Stop();
            noise.volume = 0;
            noise.Play();
        }
    }
}