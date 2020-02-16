using UnityEngine;
using UnityEngine.Serialization;

namespace UnityUtils.BooleanProviders
{
    public class RandomBooleanProvider : BooleanProvider
    {
        [FormerlySerializedAs("chanceToBeActive")] [Range(0f, 1f)] [SerializeField] private float chanceToBeTrue = 0.5f;

        public override bool ProvideBoolean()
        {
            return Random.value < chanceToBeTrue;
        }
    }
}