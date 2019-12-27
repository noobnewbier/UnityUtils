using UnityEngine;

namespace UnityUtils.ActivityProviders
{
    public class ExactActivityProvider : ActivityProvider
    {
        [SerializeField] private bool isActive;
        public override bool ProvideIsActive() => isActive;
    }
}