using UnityEngine;

namespace AnttiStarterKit.Extensions
{
    public static class VectorExtension
    {
        public static Vector3 RandomOffset(this Vector3 v, float maxLength)
        {
            v += new Vector3(Random.Range(-maxLength, maxLength), Random.Range(-maxLength, maxLength), 0);
            return v;
        }

        public static Vector3 WhereX(this Vector3 v, float x) {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WhereY(this Vector3 v, float y) {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WhereZ(this Vector3 v, float z) {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 WhereX(this Vector2 v, float x) {
            return new Vector2(x, v.y);
        }
	
        public static Vector2 WhereY(this Vector2 v, float y) {
            return new Vector2(v.x, y);
        }
	
        public static Vector3 WhereZ(this Vector2 v, float z) {
            return new Vector3(v.x, v.y, z);
        }

        public static float RealAngle(this Vector3 v)
        {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        public static float RealAngle(this Vector2 v)
        {
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
    }
}