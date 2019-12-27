using UnityEngine;

namespace UnityUtils.ActivityProviders
{
    public class ExactActivityProviders : ActivityProvider
    {
        [SerializeField] private bool isActive;
        public override bool ProvideIsActive() => isActive;
    }
}