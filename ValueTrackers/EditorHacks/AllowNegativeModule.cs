using System;
using System.Linq;
using Unity.Profiling.Editor;
using UnityEngine;

namespace UnityUtils.EditorHacks
{
    [Serializable]
    public abstract class AllowNegativeModule : ProfilerModule
    {
        protected AllowNegativeModule(ProfilerCounterDescriptor[] chartCounters, ProfilerModuleChartType defaultChartType = ProfilerModuleChartType.Line, string[] autoEnabledCategoryNames = null) : base(chartCounters, defaultChartType, autoEnabledCategoryNames) { }

        protected ProfilerCounterDescriptor[] ExposedChartCounters
        {
            get => ChartCounters;
            set => InternalSetChartCounters(value);
        }

        internal override void Rebuild()
        {
            base.Rebuild();
            m_Chart.graphRange = new (float.NegativeInfinity, float.PositiveInfinity); //todo: chart cannot handle negative... might have to reimplment the chart so it can do that.
            m_Chart.labelRange = new (float.NegativeInfinity, float.PositiveInfinity);
            m_Chart.m_SharedScale = true;
        }

        protected void ExposedRebuild()
        {
            Rebuild();
        }

        internal override void Update()
        {
            base.Update();

            /*
             * Note:
             * Overriding range axis so we can display negative value
             * https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/ProfilerEditor/ProfilerWindow/ProfilerChart.cs
             */
            if (!m_Chart.m_Series.Any()) return;

            // expanding all series so there range axis are the same - we only need to deal with minimum as Unity already works with the max. 
            var totalMin = 0f;
            foreach (var s in m_Chart.m_Series)
            {
                //todo: this is probably, pricey - 2000 (ProfilerUserSettings.m_FrameCount) per series. wbn if we can cache that fucker.!
                var minYInSeries = s.yValues.Min();
                totalMin = Mathf.Min(totalMin, minYInSeries);
            }

            foreach (var s in m_Chart.m_Series) s.rangeAxis = new (totalMin, s.rangeAxis.y);
        }
    }
}