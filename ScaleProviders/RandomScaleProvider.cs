using UnityEngine;

namespace UnityUtils.ScaleProviders
{
    public class RandomScaleProvider : ScaleProvider
    {
        [SerializeField] private Vector3 minScale;
        [SerializeField] private Vector3 maxScale;

        public override Vector3 ProvideScale()
        {
            return new Vector3(
                Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                Random.Range(minScale.z, maxScale.z)
            );
        }
    }
}