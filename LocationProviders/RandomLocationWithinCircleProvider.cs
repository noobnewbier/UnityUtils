using UnityEngine;

namespace UnityUtils.LocationProviders
{
    public class RandomLocationWithinCircleProvider : LocationProvider
    {
        [SerializeField] private float radius;

        public override Vector3 ProvideLocation()
        {
            var point = Random.insideUnitCircle * radius;
            var position = transform.position;
            return new Vector3(point.x + position.x, position.y, point.y + position.z);
        }
    }
}