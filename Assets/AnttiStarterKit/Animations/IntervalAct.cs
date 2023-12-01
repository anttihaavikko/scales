using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Animations
{
    public class IntervalAct : MonoBehaviour
    {
        [SerializeField] private UnityEvent action;
        [SerializeField] private float minDelay = 1f;
        [SerializeField] private float maxDelay = 10f;
        
        private void Start()
        {
            Invoke(nameof(Act), Random.Range(minDelay, maxDelay));
        }

        private void Act()
        {
            action?.Invoke();
            Invoke(nameof(Act), Random.Range(minDelay, maxDelay));
        }
    }
}