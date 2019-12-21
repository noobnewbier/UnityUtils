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
            
            return new Vector3(point.x, transform.position.y, point.y);
        }
    }
}