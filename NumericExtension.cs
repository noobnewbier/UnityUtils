using UnityEngine;

namespace Utils
{
    public static class FloatUtil
    {
        public static bool NearlyEqual(float a, float b, float epsilon = 0.005f)
        {
            var absA = Mathf.Abs(a);
            var absB = Mathf.Abs(b);
            var diff = Mathf.Abs(a - b);

            if (a == b)
            {
                // shortcut, handles infinities
                return true;
            }

            if (a == 0 || b == 0 || diff < float.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < epsilon * float.MinValue;
            }

            // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
}