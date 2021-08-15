using UnityEngine;

namespace UnityUtils.PositionProviders
{
    public class ExactPositionProvider : PositionProvider
    {
        public override Vector3 ProvideLocation() => transform.position;
    }
}