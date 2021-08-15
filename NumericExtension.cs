using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnityUtils
{
    public static class FloatUtil
    {
        [SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "The exact comparison is only used for edge cases where it is indeed the same, which makes comparison faster"
        )]
        public static bool NearlyEqual(float a, float b, float epsilon = 0.005f)
        {
            var absA = Mathf.Abs(a);
            var absB = Mathf.Abs(b);
            var diff = Mathf.Abs(a - b);

            if (a == b)
                // shortcut, handles infinities
                return true;

            if (a == 0 || b == 0 || diff < float.Epsilon)
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon * float.MinValue;

            // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
}