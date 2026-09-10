using System;
using System.Linq;
using System.Reflection;
using Unity.Profiling.Editor;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtils.EditorHacks
{
    [Serializable]
    public abstract class AllowNegativeModule : ProfilerModule
    {
        /// <summary>
        /// May god grant me peace in my final days.
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/9d487cab41b00c50af020b56d27a3c768d54f770/Modules/ProfilerEditor/ProfilerWindow/ModuleChart/ChartSelectionLabelsWidget.cs#L40
        /// </summary>
        private static readonly FieldInfo LabelRangeField = typeof(ChartSelectionLabelsWidget).GetField("labelRange", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("Probably an Unity update that breaks this, future me embrace your eternal punishment for your past sins.")!;
        private static readonly Vector2 NegativeLabelRange = new (Mathf.NegativeInfinity, Mathf.Infinity);

        protected AllowNegativeModule(ProfilerCounterDescriptor[] chartCounters, ProfilerModuleChartType defaultChartType = ProfilerModuleChartType.Line, string[] autoEnabledCategoryNames = null!) : base(chartCounters, defaultChartType, autoEnabledCategoryNames) { }

        protected ProfilerCounterDescriptor[] ExposedChartCounters
        {
            get => ChartCounters;
            set => InternalSetChartCounters(value);
        }

        internal override void OnEnable()
        {
            SetPinnedStateWithoutCallback(ReadPinnedState());
            active = ReadActiveState();
            SaveActiveState();
            ProfilerWindow.SelectedFrameIndexChanged += SelectedFrameIndexChanged;
        }

        internal override void OnDisable()
        {
            ProfilerWindow.SelectedFrameIndexChanged -= SelectedFrameIndexChanged;
            SaveViewSettings();
        }

        private void SelectedFrameIndexChanged(long selectedFrameIndex)
        {
            var cachedValue = (Vector2)LabelRangeField.GetValue(null);
            LabelRangeField.SetValue(null, NegativeLabelRange);

            ChartModelBuilder.UpdateSelectedData(selectedFrameIndex);
            ChartViewController.NotifySelectedFrameIndexChanged(selectedFrameIndex);

            LabelRangeField.SetValue(null, cachedValue);
        }

        internal override ChartViewController CreateChartViewController()
        {
            return new ViewControllerWithCustomChart(this, ChartType, ChartModelBuilder.Model)
            {
                ModuleSelected = (Action)(() => ProfilerWindow.selectedModule = this),
                CountersEnabledStateChanged = ChartModelBuilder.OnCountersEnableChange,
                CountersOrderChanged = ChartModelBuilder.OnCountersOrderChange,
                SelectedFrameChanged = (Action<int>)(x => ProfilerWindow.SetCurrentFrame(x))
            };
        }

        protected void ExposedRebuild()
        {
            Rebuild();
        }
        
        internal override ChartModelBuilder CreateChartModelBuilder()
        {
            var chartModelBuilder = new AllowNegativeChartModelBuilder(SettingsService, ChartType, ChartCounters.Length, Identifier, DisplayName, Tooltip, IconPath);
            chartModelBuilder.SetArea(area);
            chartModelBuilder.ConfigureChartSeries(ProfilerUserSettings.frameCount, ChartCounters);

            return chartModelBuilder;
        }

        private class ViewControllerWithCustomChart : ChartViewController
        {
            /*
             * May god rest my soul when judgment day comes
             * m_ChartWidget is private, I couldn't find a better way to hatch this together without just duplicating Unity's code from their repo and modify it nicely
             * which is bonkers but oh well I am well in the "you really shouldn't be here mate" territory.
             */
            private readonly FieldInfo _chartWidgetField = typeof(ChartViewController).GetField("m_ChartWidget", BindingFlags.Instance | BindingFlags.NonPublic)!;
            private readonly ChartModel _sameModelTheBaseClassIsUsingButINeedAccess;

            public ViewControllerWithCustomChart(ProfilerModule module, ProfilerModuleChartType type, ChartModel model) : base(module, type, model)
            {
                _sameModelTheBaseClassIsUsingButINeedAccess = model;
            }

            protected override void ViewLoaded()
            {
                base.ViewLoaded();

                // Clear previous callback
                Chart.generateVisualContent = null;
                var customLineChart = new AllowNegativeLineChart(_sameModelTheBaseClassIsUsingButINeedAccess, Chart);
                _chartWidgetField.SetValue(this, customLineChart);
            }

            private class AllowNegativeLineChart : ChartWidget
            {
                public AllowNegativeLineChart(ChartModel model, VisualElement root) : base(model, root) { }

                protected override void UpdateGeometry(MeshGenerationContext mgc)
                {
                    var workArea = Root.contentRect;

                    var painter = mgc.painter2D;

                    var model = Model;
                    for (var series = 0; series < model.numSeries; series++)
                    {
                        var seriesValues = model.series[series];
                        if (!seriesValues.enabled)
                            continue;

                        var seriesLength = seriesValues.yValues.Length;
                        if (seriesLength == 0)
                            continue;
                        var seriesRange = seriesValues.rangeAxis;

                        painter.strokeColor = seriesValues.color;
                        painter.lineCap = LineCap.Butt;

                        painter.BeginPath();
                        var posY = (seriesValues.yValues[0] - seriesRange.x) / seriesRange.y;
                        painter.MoveTo(new (workArea.x, (1 - posY) * workArea.height + workArea.y));
                        var firstValue = true;

                        for (var i = 1; i < seriesLength; i++)
                        {
                            if (seriesValues.yValues[i] == -1)
                            {
                                // skip this value
                            }
                            else
                            {
                                var unitX = (float)i / seriesLength;
                                var unitY = (seriesValues.yValues[i] - seriesRange.x) / (seriesRange.y - seriesRange.x);
                                if (firstValue)
                                {
                                    painter.MoveTo(new (unitX * workArea.width + workArea.x, (1 - unitY) * workArea.height + workArea.y));
                                    firstValue = false;
                                }
                                else
                                {
                                    painter.LineTo(new (unitX * workArea.width + workArea.x, (1 - unitY) * workArea.height + workArea.y));
                                }
                            }
                        }

                        painter.Stroke();
                    }
                }
            }
        }

        private class AllowNegativeChartModelBuilder : ChartModelBuilder
        {
            public AllowNegativeChartModelBuilder(IProfilerPersistentSettingsService settingsService, ProfilerModuleChartType chartType, int seriesCount, string name, string localizedName, string tooltip, string iconName) : base(settingsService, chartType, seriesCount, name, localizedName, tooltip, iconName) { }

            public override void UpdateData(int firstEmptyFrame, int firstFrame, int frameCount)
            {
                base.UpdateData(firstEmptyFrame, firstFrame, frameCount);


                if (!Model.series.Any()) return;

                // expanding all series so there range axis are the same - we only need to deal with minimum as Unity already works with the max. 
                var totalMin = -1f;
                var totalMax = 1f;
                foreach (var s in Model.series)
                {
                    //todo: this is probably, pricey - 2000 (ProfilerUserSettings.m_FrameCount) per series. wbn if we can cache that fucker.!
                    var minYInSeries = s.yValues.Min();
                    var maxYInSeries = s.yValues.Max();
                    totalMin = Mathf.Min(totalMin, minYInSeries);
                    totalMax = Mathf.Max(totalMax, maxYInSeries);
                }

                foreach (var s in Model.series) s.rangeAxis = new (totalMin, totalMax); //todo: manually sharing scale, is this the right thing?
            }
        }
    }
}