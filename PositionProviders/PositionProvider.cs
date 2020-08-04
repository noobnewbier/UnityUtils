using UnityEngine;

namespace UnityUtils.PositionProviders
{
    // in theory, it's nice if it is an interface, but we aren't using Odin :(
    public abstract class PositionProvider : MonoBehaviour
    {
        public abstract Vector3 ProvideLocation();
    }
}