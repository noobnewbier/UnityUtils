using UnityEngine;

namespace UnityUtils.ScaleProviders
{
    public class ExactScaleProvider : ScaleProvider
    {
        [SerializeField] private Vector3 scale;

        public override Vector3 ProvideScale() => scale;
    }
}