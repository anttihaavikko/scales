using UnityEngine;

namespace AnttiStarterKit.Extensions
{
    public static class ColorExtension
    {
        public static Color RandomTint(this Color color, float amount)
        {
            color += new Color(Random.Range(-amount, amount), Random.Range(-amount, amount), Random.Range(-amount, amount), 0);
            return color;
        }
    }
}