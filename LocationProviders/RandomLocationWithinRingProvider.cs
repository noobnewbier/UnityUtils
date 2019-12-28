using UnityEngine;

namespace UnityUtils.LocationProviders
{
    public class RandomLocationWithinRingProvider : LocationProvider
    {
        [SerializeField] private float innerRadius;
        [SerializeField] private float outerRadius;

        public override Vector3 ProvideLocation()
        {
            var point = RandomUtils.RandomPointWithinRing(innerRadius, outerRadius);

            var position = transform.position;
            return new Vector3(point.x + position.x, position.y, point.y + position.z);
        }
    }
}