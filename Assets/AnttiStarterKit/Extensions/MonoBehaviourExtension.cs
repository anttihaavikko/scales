using System.Collections;
using UnityEngine;

namespace AnttiStarterKit.Extensions
{
    public static class MonoBehaviourExtension
    {
        public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Action action, float delay)
        {
            return behaviour.StartCoroutine(WaitAndDo(delay, action));
        }

        private static IEnumerator WaitAndDo(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }
}