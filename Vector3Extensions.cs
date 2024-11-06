using UnityEngine;

namespace UnityUtils
{
    public static class Vector3Extensions
    {
        public static Vector2 XZ(this Vector3 vector3) => new Vector2(vector3.x, vector3.z);

        public static bool NearlyEqual(this Vector3 a, Vector3 b, float epsilon = 0.005f)
        {
            var dist = Vector3.Distance(a, b);
            return dist.NearlyEqual(0f, epsilon);
        }
    }
}