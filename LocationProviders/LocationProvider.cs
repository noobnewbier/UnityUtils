using UnityEngine;

namespace UnityUtils.LocationProviders
{
    // in theory, it's nice if it is an interface, but we aren't using Odin :(
    public abstract class LocationProvider : MonoBehaviour
    {
        public abstract Vector3 ProvideLocation();
    }
}