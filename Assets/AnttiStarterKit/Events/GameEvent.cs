using System.Collections.Generic;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;

namespace AnttiStarterKit.Events
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

        public void Trigger()
        {
            _listeners.ForEach(l => l.OnTrigger());
        }

        public void AddListener(GameEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(GameEventListener listener)
        {
            _listeners.Remove(listener);
        }
    }
}