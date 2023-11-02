using UnityEngine;

namespace AnttiStarterKit.Randomizers
{
    public class RandomMirror : MonoBehaviour
    {
        private void Start()
        {
            var t = transform;
            var scale = t.localScale;
            scale = new Vector3(Random.value < 0.5f ? 1f : -1f * scale.x, scale.y, scale.z);
            t.localScale = scale;
        }
    }
}
