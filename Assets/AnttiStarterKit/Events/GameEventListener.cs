using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent triggerEvent;
        [SerializeField] private UnityEvent response;

        private void OnEnable()
        {
            triggerEvent.AddListener(this);
        }

        private void OnDisable()
        {
            triggerEvent.RemoveListener(this);
        }

        public void OnTrigger()
        {
            response.Invoke();
        }
    }
}