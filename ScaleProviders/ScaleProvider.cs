using UnityEngine;

namespace UnityUtils.ScaleProviders
{
    public abstract class ScaleProvider : MonoBehaviour
    {
        public abstract Vector3 ProvideScale();
    }
}