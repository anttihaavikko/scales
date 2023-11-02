using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public static class DebugDraw
    {
        public static void Square(Vector3 pos, Color color, float len = 1f, float duration = 1f)
        {
            var half = len * 0.5f;
            Debug.DrawRay(pos + Vector3.left * half + Vector3.up * half, Vector3.right * len, color, duration);
            Debug.DrawRay(pos + Vector3.left * half + Vector3.up * half, Vector3.down * len, color, duration);
            Debug.DrawRay(pos + Vector3.right * half + Vector3.down * half, Vector3.up * len, color, duration);
            Debug.DrawRay(pos + Vector3.right * half + Vector3.down * half, Vector3.left * len, color, duration);
        }
        
        public static void Square(Vector3 pos)
        {
            Square(pos, Color.red);
        }
    }
}
