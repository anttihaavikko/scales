using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Color Collection", menuName = "Color collection", order = 0)]
    public class ColorCollection : ScriptableObject
    {
        [SerializeField] private List<Color> colors;

        public int Count => colors.Count;

        public Color Random()
        {
            return !colors.Any() ? Color.white : colors.Random();
        }

        public Color Get(int index)
        {
            return colors[index];
        }

        public List<Color> ToList()
        {
            return colors;
        }
    }
}