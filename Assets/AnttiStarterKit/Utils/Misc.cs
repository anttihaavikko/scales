using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public static class Misc
    {
        public static int PlusMinusOne()
        {
            return Random.value < 0.5f ? 1 : -1;
        }
        
        public static Vector3 VectorFromAngle(float deg)
        {
            var angle = deg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        }
    }
}