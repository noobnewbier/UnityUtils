using UnityEngine;

namespace UnityUtils.FloatProvider
{
    public class ExactFloatProvider : FloatProvider
    {
        [SerializeField] private float value;

        public override float ProvideFloat()
        {
            return value;
        }
    }
}