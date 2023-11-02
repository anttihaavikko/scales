using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Randomizers
{
    public class GroupRandomizer : MonoBehaviour
    {
        [SerializeField] private List<Transform> objects;
        [SerializeField] private bool rotate;
        [SerializeField] private float minRotation = 0f, maxRotation = 360f;
        [SerializeField] private bool scale;
        [SerializeField] private float minScale = 0.9f, maxScale = 1.1f;

        private void Start()
        {
            objects.ForEach(Apply);
        }

        private void Apply(Transform t)
        {
            if (rotate)
            {
                t.Rotate(new Vector3(0, 0, Random.Range(minRotation, maxRotation)));
            }

            if (scale)
            {
                t.localScale *= Random.Range(minScale, maxScale);
            }
        }
    }
}