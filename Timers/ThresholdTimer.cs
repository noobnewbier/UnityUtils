using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityUtils.Timers
{
    public class ThresholdTimer : MonoBehaviour
    {
        [SerializeField] private bool isInitialized;
        [SerializeField] protected float threshold;
        [SerializeField] protected float timer;

        public bool PassedThreshold => timer > threshold;

        public bool ReachedThreshold => timer >= threshold;

        public float NormalizedTime => timer / threshold;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public bool ResetInThisFrame => timer == 0f;

        public void Reset()
        {
            timer = 0f;
        }

        private void Update()
        {
            Count();
        }

        public void Init(float newThreshold, float currentTime = 0)
        {
            Init(newThreshold, false, currentTime);
        }

        public virtual void Init(float newThreshold, [UsedImplicitly] bool forceInit, float currentTime = 0)
        {
            if (isInitialized && !forceInit)
                throw new InvalidOperationException(
                    $"timer in {gameObject.name} is already initialized, but is initialized again"
                );

            timer = currentTime;
            threshold = newThreshold;
            isInitialized = true;
        }

        public bool TryResetIfPassedThreshold()
        {
            if (timer > threshold)
            {
                timer = 0;
                return true;
            }

            return false;
        }

        protected virtual void Count()
        {
            if (!isInitialized)
                throw new InvalidOperationException($"timer in {gameObject.name} is not initialized");

            timer += Time.deltaTime;
        }
    }
}