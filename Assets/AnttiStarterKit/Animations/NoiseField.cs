using System.Collections.Generic;
using UnityEngine;

namespace AnttiStarterKit.Animations
{
    public class NoiseField : MonoBehaviour
    {
        public List<Transform> objects;
        public bool mirrored = true;
        public float xAmount = 1f;
        public float yAmount = 1f;
        public float scaleAmount = 1f;

        private Vector3[] sizes;
        private float offset;
        private Vector3[] positions;
        
        private void Awake()
        {
            sizes = new Vector3[objects.Count];
            positions = new Vector3[objects.Count];

            var i = 0;
            objects.ForEach(o =>
            {
                positions[i] = o.position;
                if(mirrored)
                    o.localScale = new Vector3(Random.value < 0.5f ? -1 : 1, Random.value < 0.5f ? -1 : 1, 1);
                sizes[i] = o.localScale;
                i++;
            });
        }

        private void Update()
        {
            var i = 0;
            objects.ForEach(o =>
            {
                var p = o.position;
                var noise = Mathf.PerlinNoise(p.x + offset, p.y);
                var x = Mathf.PerlinNoise(p.x - offset, p.y) * xAmount;
                var y = Mathf.PerlinNoise(p.x, p.y + offset) * yAmount;
                o.transform.localScale = sizes[i] * (1f - 0.4f * Mathf.Abs(noise) * scaleAmount);
                p = positions[i] + Vector3.left * x * 0.15f + Vector3.up * y * 0.1f;
                o.position = p;
                i++;
            });

            offset += Time.deltaTime;
        }
    }
}
