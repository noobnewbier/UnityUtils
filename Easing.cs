using UnityEngine;

namespace UnityUtils
{
    public static class Easing
    {
        public static float ExponentialEaseOut(float x, float exponent)
        {
            var y = Flip(Mathf.Pow(Flip(x), exponent));
            return y;
        }

        private static float Flip(float x)
        {
            return 1 - x;
        }

        public static float EaseOutElastic(float t)
        {
            var tSquare = t * t;
            var tCube = tSquare * t;
            return (33 * tCube * tSquare + -106 * tSquare * tSquare + 126 * tCube + -67 * tSquare + 15 * t);
        }
    }
}