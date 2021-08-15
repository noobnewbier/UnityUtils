using UnityEngine;

namespace UnityUtils
{
    public static class RandomUtils
    {
        //based on this: https://math.stackexchange.com/questions/2530527/how-to-generate-a-uniformly-distributed-point-in-an-annular-area
        public static Vector2 RandomPointWithinRing(float innerRadius, float outerRadius)
        {
            var angle = 2 * Mathf.PI * Random.value;
            var outerRadiusSquared = outerRadius * outerRadius;
            var innerRadiusSquared = innerRadius * innerRadius;
            var distance = Mathf.Sqrt(Random.value * (outerRadiusSquared - innerRadiusSquared) + innerRadiusSquared);

            var xPos = distance * Mathf.Cos(angle);
            var yPos = distance * Mathf.Sin(angle);

            return new Vector2(xPos, yPos);
        }
    }
}