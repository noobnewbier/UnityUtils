using UnityEngine;

namespace UnityUtils
{
    public abstract class ActivityProvider : MonoBehaviour
    {
        public abstract bool ProvideIsActive();
    }
}