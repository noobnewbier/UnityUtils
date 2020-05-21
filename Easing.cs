using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnityUtils
{
    public static class Easing
    {
        //https://math.stackexchange.com/questions/121720/ease-in-out-function
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static float ExponentialEaseInOut(float x, float exponent)
        {
            var x_exponent = Mathf.Pow(x, exponent);

            return x_exponent / (x_exponent + Mathf.Pow(1f - x, exponent));
        }


        public static float ExponentialEaseOut(float x, float exponent)
        {
            var y = Flip(Mathf.Pow(Flip(x), exponent));
            return y;
        }

        public static float Flip(float x)
        {
            return 1 - x;
        }

        public static float EaseOutElastic(float t)
        {
            var tSquare = t * t;
            var tCube = tSquare * t;
            return 33 * tCube * tSquare + -106 * tSquare * tSquare + 126 * tCube + -67 * tSquare + 15 * t;
        }

        public static Vector3 QuadraticBezierLerping(Vector3 start, Vector3 middle, Vector3 target, float t)
        {
            return Vector3.Lerp(
                Vector3.Lerp(start, middle, t),
                Vector3.Lerp(middle, target, t),
                t
            );
        }
    }
}