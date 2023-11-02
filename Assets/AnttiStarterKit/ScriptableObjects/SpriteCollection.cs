using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Sprite Collection", menuName = "Sprite collection", order = 0)]
    public class SpriteCollection : ScriptableObject
    {
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private List<Sprite> sprites;

        public Sprite Random()
        {
            return !sprites.Any() ? defaultSprite : sprites.Random();
        }

        public Sprite Get(int index)
        {
            return sprites.Count > index ? sprites[index] : defaultSprite;
        }
    }
}