using UnityEngine;

namespace UnityUtils.LocationProviders
{
    public class RandomPositionWithinCircleProvider : LocationProvider
    {
        [SerializeField] private float radius;
        public override Vector3 ProvideLocation() => Random.insideUnitCircle * radius;
    }
}