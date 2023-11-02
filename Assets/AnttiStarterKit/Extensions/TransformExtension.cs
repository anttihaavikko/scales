using UnityEngine;

namespace AnttiStarterKit.Extensions
{
    public static class TransformExtension
    {
        public static void Mirror(this Transform t, bool shouldMirror = true)
        {
            var scale = t.localScale;
            t.localScale = new Vector3((shouldMirror ? -1f : 1f) * scale.x, scale.y, scale.z);
        }
        
        public static void Flip(this Transform t, bool shouldMirror = true)
        {
            var scale = t.localScale;
            t.localScale = new Vector3(scale.x, (shouldMirror ? -1f : 1f) * scale.y, scale.z);
        }

        public static Vector3 DirectionTo(this Transform t, Transform target)
        {
            return target.position - t.position;
        }
    }
}