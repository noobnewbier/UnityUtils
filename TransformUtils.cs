using UnityEngine;

namespace UnityUtils
{
    public class TransformUtils
    {
        /// <summary>
        /// https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/5841fe08c0b404df4c834980372d8c11a5a33bc5/Framework/com.spacepuppy.core/Runtime/src/Utils/TransformUtil.cs#L9
        /// </summary>
        public static Vector3 GetTranslation(Matrix4x4 m)
        {
            var col = m.GetColumn(3);
            return new (col.x, col.y, col.z);
        }

        /// <summary>
        /// https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/5841fe08c0b404df4c834980372d8c11a5a33bc5/Framework/com.spacepuppy.core/Runtime/src/Utils/TransformUtil.cs#L9
        /// </summary>
        public static Quaternion GetRotation(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            var q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }

        /// <summary>
        /// https://github.com/lordofduct/spacepuppy-unity-framework-4.0/blob/5841fe08c0b404df4c834980372d8c11a5a33bc5/Framework/com.spacepuppy.core/Runtime/src/Utils/TransformUtil.cs#L9
        /// </summary>
        public static Vector3 GetScale(Matrix4x4 m) => new (m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
    }
}