using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Animations
{
    public class OffTurner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objects;
        [SerializeField] private float offChance = 0.5f;

        private void Start()
        {
            objects.ForEach(o => o.SetActive(Random.value < offChance));
        }
    }
}