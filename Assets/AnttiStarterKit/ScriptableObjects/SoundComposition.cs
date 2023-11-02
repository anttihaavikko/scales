using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AnttiStarterKit.Managers;
using UnityEditor;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Sound composition", menuName = "Composite sound", order = 0)]
    public class SoundComposition : ScriptableObject
    {
        public List<SoundCompositionRow> rows;
        public List<SoundCollectionRow> collections;

        public void Play()
        {
            Play(Vector3.zero);
        }

        public void Play(Vector3 pos, float volume = 1f)
        {
            var am = AudioManager.Instance;
            if (!am) return;
            
            foreach (var row in rows)
            {
                am.PlayEffectAt(row.clip, pos, row.volume * volume);
            }
            
            foreach (var row in collections)
            {
                am.PlayEffectFromCollection(row.collection, pos, row.volume * volume);
            }
        }
    }

    [Serializable]
    public class SoundCompositionRow
    {
        public AudioClip clip;
        public float volume = 1f;
    }
    
    [Serializable]
    public class SoundCollectionRow
    {
        public SoundCollection collection;
        public float volume = 1f;
    }
}