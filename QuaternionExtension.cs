using UnityEngine;

namespace UnityUtils
{
    public static class QuaternionExtension
    {
        public static void Decompose(this Quaternion quaternion, Vector3 direction, out Quaternion swing, out Quaternion twist)
        {
            var vector = new Vector3(quaternion.x, quaternion.y, quaternion.z);
            var projection = Vector3.Project(vector, direction);

            twist = new Quaternion(projection.x, projection.y, projection.z, quaternion.w).normalized;
            swing = quaternion * Quaternion.Inverse(twist);
        }
        
        public static Quaternion Constrain(this Quaternion quaternion, float angle)
        {
            var magnitude = Mathf.Sin(0.5F * angle);
            var sqrMagnitude = magnitude * magnitude;

            var vector = new Vector3(quaternion.x, quaternion.y, quaternion.z);

            if (vector.sqrMagnitude > sqrMagnitude)
            {
                vector = vector.normalized * magnitude;

                quaternion.x = vector.x;
                quaternion.y = vector.y;
                quaternion.z = vector.z;
                quaternion.w = Mathf.Sqrt(1.0F - sqrMagnitude) * Mathf.Sign(quaternion.w);
            }

            return quaternion;
        }
    }
}