using System;
using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnttiStarterKit.Music
{
    public class BeatFollower : Manager<BeatFollower>
    {
        [SerializeField] private int bpm = 100;
        [SerializeField] int divisions = 8; 
        [SerializeField] List<bool> pattern;
        [SerializeField] private float offset;

        public Action onBeat;
        
        private int beat;
        private float position;
        private Coroutine loop;

        private float Delay => 60f / bpm / divisions * 4;

        public List<bool> Pattern => pattern;
        public int Divisions => divisions;

        private void Start()
        {
            position = AudioManager.Instance ? AudioManager.Instance.curMusic.time : 0;
            beat = Mathf.FloorToInt(position / Delay) + 1;
            loop = StartCoroutine(Loop());
        }

        private IEnumerator Loop()
        {
            if (position > 0)
            {
                yield return new WaitForSecondsRealtime(Delay - position % Delay);   
            }

            if (offset < 0)
            {
                yield return new WaitForSecondsRealtime(Delay - offset * 0.001f);   
                NextBeat();
            }

            while (true)
            {
                OnBeat();
                yield return new WaitForSecondsRealtime(Delay);
            }
        }

        private void OnBeat()
        {
            if (pattern[beat])
            {
                onBeat?.Invoke();
            }

            NextBeat();
        }

        private void NextBeat()
        {
            beat = (beat + 1) % pattern.Count;
        }

        public void Toggle(int index)
        {
            pattern[index] = !pattern[index];
        }

        public void Reset()
        {
            StopCoroutine(loop);
            Start();
        }

        public void AddBar()
        {
            for (var i = 0; i < divisions; i++)
            {
                pattern.Add(false);   
            }
        }
    }
}