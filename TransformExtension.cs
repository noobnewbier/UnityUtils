using System.Linq;
using UnityEngine;

namespace UnityUtils
{
    public static class TransformExtension
    {
        public static Transform FindChildWithTag(this Transform transform, string tag)
        {
            return transform.Cast<Transform>().FirstOrDefault(child => child.CompareTag(tag));
        }
    }
}