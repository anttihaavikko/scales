using UnityEngine;

namespace AnttiStarterKit.Randomizers
{
    public class RandomScale : MonoBehaviour
    {
        public float min = 0.9f;
        public float max = 1.1f;

        private void Start()
        {
            transform.localScale *= Random.Range(min, max);
        }
    }
}
