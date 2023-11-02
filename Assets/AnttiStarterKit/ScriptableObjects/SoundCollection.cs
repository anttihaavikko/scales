using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Sound Collection", menuName = "Sound collection", order = 0)]
    public class SoundCollection : ScriptableObject
    {
        [SerializeField] private float volume = 1f;
        [SerializeField] private List<AudioClip> clips;
        
        public int Count => clips.Count;
        public float Volume => volume;

        public AudioClip Random()
        {
            return !clips.Any() ? null : clips.Random();
        }

        public AudioClip At(int i)
        {
            return clips[i];
        }
    }
}