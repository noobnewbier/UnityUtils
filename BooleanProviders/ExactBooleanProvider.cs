using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUtils.BooleanProviders
{
    public class ExactBooleanProvider : BooleanProvider
    {
        [FormerlySerializedAs("isActive")] [SerializeField]
        private bool boolean;

        public override bool ProvideBoolean() => boolean;
    }
}