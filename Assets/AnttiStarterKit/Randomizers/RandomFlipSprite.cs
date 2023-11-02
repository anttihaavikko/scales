using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Randomizers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RandomFlipSprite : MonoBehaviour
    {
        [SerializeField] private bool canFlipX = true, canFlipY = true;
        
        private SpriteRenderer sprite;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            sprite.flipX = canFlipX && Random.value < 0.5f;
            sprite.flipY = canFlipY && Random.value < 0.5f;
        }
    }
}