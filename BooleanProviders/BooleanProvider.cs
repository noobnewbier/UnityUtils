using UnityEngine;

namespace UnityUtils.BooleanProviders
{
    public abstract class BooleanProvider : MonoBehaviour
    {
        public abstract bool ProvideBoolean();
    }
}