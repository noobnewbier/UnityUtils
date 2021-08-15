using UnityEngine;
using UnityUtils.BooleanProviders;

namespace UnityUtils.Timers
{
    public class CountInConditionTimer : ThresholdTimer
    {
        [SerializeField] private BooleanProvider booleanProvider;

        [Tooltip("Should we use the opposite of what the boolean provider provides?")] [SerializeField]
        private bool isNot;

        [SerializeField] private bool shouldResetWhenConditionNotMet;

        protected override void Count()
        {
            if (isNot ^ booleanProvider.ProvideBoolean()) base.Count();
            else if (shouldResetWhenConditionNotMet) Reset();
        }
    }
}