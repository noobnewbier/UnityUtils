using UnityEngine;

namespace UnityUtils.Timers
{
    public class BouncingTimer : ThresholdTimer
    {
        private bool _isIncreasing = true;

        protected override void Count()
        {
            if (ReachedThreshold && _isIncreasing) _isIncreasing = false;
            else if (timer <= 0f && !_isIncreasing) _isIncreasing = true;

            if (_isIncreasing) timer += Time.deltaTime;
            else timer -= Time.deltaTime;

            timer = Mathf.Clamp(timer, 0f, threshold);
        }
    }
}