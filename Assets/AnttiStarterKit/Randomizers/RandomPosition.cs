using UnityEngine;

namespace AnttiStarterKit.Randomizers
{
    public class RandomPosition : MonoBehaviour
    {
        public Vector3 min, max;

        private void Start()
        {
            transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }
    }
}
