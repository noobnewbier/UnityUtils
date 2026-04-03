using System;
using UnityEngine;

namespace NonebNi.EditorHacks
{
    public class FlowLayoutScope : GUI.Scope
    {
        private bool _disposed;

        public FlowLayoutScope()
        {
            BeginFlow();
        }

        private static void BeginFlow()
        {
            var group = GUILayoutUtility.BeginLayoutGroup(GUIStyle.none, Array.Empty<GUILayoutOption>(), typeof(NonebFlowLayout));
            group.isVertical = false;
        }

        private static void EndFlow()
        {
            GUILayoutUtility.EndLayoutGroup();
        }

        protected override void CloseScope()
        {
            EndFlow();
        }
    }
}