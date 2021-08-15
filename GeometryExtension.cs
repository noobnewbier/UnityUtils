using UnityEngine;

namespace UnityUtils
{
    public static class GeometryExtension
    {
        //Seriously, I forgot the math behind it. Perhaps I should ask
        public static bool InFrontOf(this Transform self, Transform value) =>
            Vector3.Dot((value.position - self.position).normalized, self.forward) < 0;
    }
}