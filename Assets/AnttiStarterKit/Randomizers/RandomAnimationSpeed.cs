using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Randomizers
{
    public class RandomAnimationSpeed : MonoBehaviour
    {
        [SerializeField] private float min = 0.9f;
        [SerializeField] private float max = 1.1f;
        
        private void Start()
        {
            var anim = GetComponent<Animator>();
            anim.speed = Random.Range(min, max);
        }
    }
}