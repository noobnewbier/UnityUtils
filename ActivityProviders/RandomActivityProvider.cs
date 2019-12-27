using UnityEngine;

namespace UnityUtils.ActivityProviders
{
    public class RandomActivityProvider : ActivityProvider
    {
        [Range(0f, 1f)]
        [SerializeField] private float chanceToBeActive = 0.5f;
        public override bool ProvideIsActive() => Random.value < chanceToBeActive;
    }
}