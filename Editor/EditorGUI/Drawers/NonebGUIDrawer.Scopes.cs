using System;
using System.Collections.Generic;
using NonebNi.EditorHacks;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public partial class NonebGUIDrawer
    {
        /// <summary>
        /// Not/can't deal with flow layout atm.
        /// </summary>
        private readonly Stack<LayoutGroupType> _layoutGroupStack = new ();

        private LayoutGroupType CurrentLayoutGroupType
        {
            get
            {
                if (!_layoutGroupStack.TryPeek(out var type)) return LayoutGroupType.Vertical;

                return type;
            }
        }

        public IDisposable HorizontalScope()
        {
            var toReturn = new NHorizontalScope(this);
            GUILayout.Space(Indent);
            return toReturn;
        }

        public IDisposable VerticalScope() => new NVerticalScope(this);

        public IDisposable FlowLayoutScope() => new FlowLayoutScope();

        public IDisposable BoxScope(string heading = "")
        {
            var boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.padding.left *= 4;

            var verticalScope = new NVerticalScope(this, boxStyle);
            if (!string.IsNullOrEmpty(heading)) DrawHeader(heading);

            return verticalScope;
        }

        public IDisposable DisabledScope(bool isDisabled) => new EditorGUI.DisabledScope(isDisabled);
        public IDisposable IndentScope(int indent = 1) => new EditorGUI.IndentLevelScope(indent);
        public IDisposable HandlesColorScope(Color color) => new NonebEditorGUI.HandlesColorScope(color);
        public IDisposable GizmosColorScope(Color color) => new NonebEditorGUI.GizmosColorScope(color);
        public IDisposable EditorLabelWidthScope(string targetLabel) => new NonebEditorGUI.EditorLabelWidthScope(targetLabel);
        public IDisposable AssetDatabaseEditingScope() => new NonebEditorGUI.AssetDatabaseEditingScope();

        private enum LayoutGroupType
        {
            Vertical,
            Horizontal
        }

        /*
         * Backward gymnastic to figure out how we can do auto layout better.
         */
        private class NHorizontalScope : EditorGUILayout.HorizontalScope
        {
            private readonly NonebGUIDrawer _drawer;

            public NHorizontalScope(NonebGUIDrawer drawer, params GUILayoutOption[] options) : base(options)
            {
                _drawer = drawer;
                _drawer._layoutGroupStack.Push(LayoutGroupType.Horizontal);
            }

            protected override void CloseScope()
            {
                base.CloseScope();
                _drawer._layoutGroupStack.Pop();
            }
        }

        private class NVerticalScope : EditorGUILayout.VerticalScope
        {
            private readonly NonebGUIDrawer _drawer;

            public NVerticalScope(NonebGUIDrawer drawer, params GUILayoutOption[] options) : base(options)
            {
                _drawer = drawer;
                _drawer._layoutGroupStack.Push(LayoutGroupType.Vertical);
            }

            public NVerticalScope(NonebGUIDrawer drawer, GUIStyle style, params GUILayoutOption[] options) : base(style, options)
            {
                _drawer = drawer;
                _drawer._layoutGroupStack.Push(LayoutGroupType.Vertical);
            }

            protected override void CloseScope()
            {
                base.CloseScope();
                _drawer._layoutGroupStack.Pop();
            }
        }
    }
}