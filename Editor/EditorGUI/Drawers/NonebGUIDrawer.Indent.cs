using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public partial class NonebGUIDrawer
    {
        private static readonly Lazy<PropertyInfo> IndentMethod = new
        (
            () =>
            {
                var method = typeof(EditorGUI).GetProperty("indent", BindingFlags.Static | BindingFlags.NonPublic);
                return method!;
            }
        );

        public static float Indent
        {
            get
            {
                var getValue = IndentMethod.Value.GetValue(null);
                if (getValue is not float indent)
                {
                    Debug.LogError("Unity update probably broke this - it's written in 6000.0.35f1");
                    return EditorGUI.indentLevel * 15;
                }

                return indent;
            }
        }

        private Rect GetIndentedRect(string label, GUIStyle forStyle) => GetIndentedRect(new GUIContent(label), forStyle);

        private Rect GetIndentedRect(GUIContent content, GUIStyle forStyle)
        {
            var rect = GUILayoutUtility.GetRect(content, forStyle, GUILayout.ExpandWidth(true));
            if (CurrentLayoutGroupType == LayoutGroupType.Horizontal) return rect;

            var indentedRect = EditorGUI.IndentedRect(rect);
            return indentedRect;
        }
    }
}