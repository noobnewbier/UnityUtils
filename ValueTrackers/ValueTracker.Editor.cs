using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Profiling;
using Unity.Profiling.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils.EditorHacks;

namespace UnityUtils
{
    public static partial class ValueTracker
    {
#if UNITY_EDITOR
        [Serializable, ProfilerModuleMetadata("Value Tracker"), InitializeOnLoad]
        public class ValueTrackerModule : AllowNegativeModule
        {
            private static readonly List<WeakReference<ValueTrackerModule>> Instances;
            private static readonly Lazy<PropertyInfo> ChartCountersField;

            private static readonly ProfilerCounterDescriptor[] DummyCountersSoUnityDoNotComplainWhenWeAreNotTrackingAnything =
            {
                new ("Dummy", Category)
            };

            static ValueTrackerModule()
            {
                ChartCountersField = new (() => typeof(ProfilerModule).GetProperty("ChartCounters", BindingFlags.Instance | BindingFlags.NonPublic));
                Instances = new ();
            }

            public ValueTrackerModule() : base(DummyCountersSoUnityDoNotComplainWhenWeAreNotTrackingAnything)
            {
                /*
                 * Note:
                 * There was a Peter who can make clean code and be sensible,
                 * but fuck you are just too stressed now to even code remotely properly, so fuck it just write *something* to get the ball rolling
                 *
                 * Future me, forgive my sin and I wish you all the best
                 */
                Instances.Add(new (this));
            }

            public void RefreshCounters(IReadOnlyDictionary<string, ProfilerCounterValue<float>> counters)
            {
                var descriptors = counters.Select(kvPair => new ProfilerCounterDescriptor(kvPair.Key, Category)).ToArray();
                ExposedChartCounters = descriptors;
                ExposedRebuild();
            }

            public static void Refresh(IReadOnlyDictionary<string, ProfilerCounterValue<float>> counters)
            {
                Instances.RemoveAll(w => !w.TryGetTarget(out _));
                foreach (var weakReference in Instances)
                    if (weakReference.TryGetTarget(out var instance))
                        instance.RefreshCounters(counters);
            }

            public override ProfilerModuleViewController CreateDetailsViewController() => new ViewController(ProfilerWindow);

            private class ViewController : ProfilerModuleViewController
            {
                public ViewController(ProfilerWindow profilerWindow) : base(profilerWindow) { }

                protected override VisualElement CreateView()
                {
                    var view = new IMGUIContainer();
                    view.onGUIHandler += OnGUI;

                    return view;
                }

                private void OnGUI()
                {
                    var selectedFrame = Convert.ToInt32(ProfilerWindow.selectedFrameIndex);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        foreach (var (id, _) in Counters)
                        {
                            var value = ProfilerDriver.GetFormattedCounterValue(selectedFrame, Category.Name, id);
                            /*
                             * Note - Why no NonebGUI
                             * Currently there are part of the code that's directly tangling with editor stuffs,
                             * i.e we rely on ValueTrackerModule.Refresh(Counters) to automatically refresh the graph without manual input.
                             *
                             * Using NonebGUI, an editor only package, will require us to split the value tracker code into editor/runtime.
                             * Which is good, but at the moment this thing is so small that I doubt the separation of concern is worth the effort.
                             *
                             * Luckily though, GUILayout is UnityEngine(no editor!) and is the only thing we need right now.
                             */

                            /*
                             * Note - Potential Improvement
                             * We probably want to allow manual input, so when working with a standalone profiler and a build, user can manually input a value name and track them,
                             * we don't really need automatic tracking refresh when working with an actual build. Having some sane configuration that remembers user input is more than enough.
                             */
                            GUILayout.Label($"{id}:  {value}");
                        }
                    }
                }
            }
        }
    }
#endif
}