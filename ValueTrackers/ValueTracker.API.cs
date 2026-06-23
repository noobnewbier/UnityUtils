using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Unity.Profiling;
using UnityEngine;

namespace UnityUtils
{
    public static partial class ValueTracker
    {
        [PublicAPI, Conditional("ENABLE_PROFILER")]
        public static void Track(string name, float value)
        {
            /*
             * Note:
             * Unity treats -1 as a special invalid value, we need to tweak it so the chart can draw them properly
             * https://github.com/Unity-Technologies/UnityCsReference/blob/59b03b8a0f179c0b7e038178c90b6c80b340aa9f/Modules/ProfilerEditor/ProfilerWindow/Chart.cs#L770
             */
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value == -1f)
                // Not sure why, float.Epsilon wouldn't work - probably precision issue and I don't want to think about it now.
                value += 0.0001f;

            if (!Counters.TryGetValue(name, out var counter))
            {
                Counters[name] = counter = new (Category, name, ProfilerMarkerDataUnit.Count);
                ValueTrackerModule.Refresh(Counters);
            }

            counter.Value = value;
        }
#if ENABLE_PROFILER

        private static readonly ProfilerCategory Category = new ("ValueTracker");

        private static readonly Dictionary<string, ProfilerCounterValue<float>> Counters = new ();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            //todo: re-enter play mode may need clear as well, but for the most part current version is good enough for testing and debugging the camera.
            Counters.Clear();
        }

#endif
    }
}