using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityUtils
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private bool isInitialized;
        [SerializeField] private float threshold;
        [SerializeField] private float timer;

        public bool PassedThreshold => threshold > timer;

        public float NormalizedTime => timer / threshold;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public bool ResetInThisFrame => timer == 0f;

        public void Init(float newThreshold, float currentTime = 0)
        {
            Init(newThreshold, false, currentTime);
        }

        public void Init(float newThreshold, [UsedImplicitly] bool forceInit, float currentTime = 0)
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

        private void Update()
        {
            if (!isInitialized)
                throw new InvalidOperationException(
                    $"timer in {gameObject.name} is not initialized"
                );

            timer += Time.deltaTime;
        }
    }
}