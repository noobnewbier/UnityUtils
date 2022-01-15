using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public static class NonebEditorGUI
    {
        public static void ShowStringPopup(Rect rect,
                                           SerializedProperty property,
                                           string label,
                                           string[] options)
        {
            var currentIndex = Array.IndexOf(options, property.stringValue);
            var newIndex = EditorGUI.Popup(
                rect,
                label,
                currentIndex,
                options
            );
            if (currentIndex != newIndex) property.stringValue = options[newIndex];
        }
    }
}