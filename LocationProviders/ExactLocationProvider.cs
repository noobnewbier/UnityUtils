using UnityEngine;

namespace UnityUtils.LocationProviders
{
    public class ExactLocationProvider : LocationProvider
    {
        public override Vector3 ProvideLocation() => transform.position;
    }
}