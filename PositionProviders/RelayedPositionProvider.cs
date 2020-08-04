using UnityEngine;

namespace UnityUtils.PositionProviders
{
    public class RelayedPositionProvider : PositionProvider
    {
        [SerializeField] private PositionProvider relayedPositionProvider;
        
        public override Vector3 ProvideLocation()
        {
            return relayedPositionProvider.ProvideLocation();
        }
    }
}