using UnityEngine;

namespace UnityUtils
{
    public static class Vector3Extensions
    {
        public static Vector2 XZ(this Vector3 vector3) => new Vector2(vector3.x, vector3.z);
    }
}