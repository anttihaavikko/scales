using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AnttiStarterKit.Music
{
    public class BeatAction : MonoBehaviour
    {
        [SerializeField] private UnityEvent action;

        private void Start()
        {
            BeatFollower.Instance.onBeat += Act;
        }

        private void OnDestroy()
        {
            BeatFollower.Instance.onBeat -= Act;
        }

        private void Act()
        {
            action?.Invoke();
        }
    }
}