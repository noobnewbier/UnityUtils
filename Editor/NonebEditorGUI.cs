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

        public static int ShowStringPopup(Rect rect,
                                          string currentValue,
                                          string label,
                                          string[] options)
        {
            var currentIndex = Array.IndexOf(options, currentValue);
            var newIndex = EditorGUI.Popup(
                rect,
                label,
                currentIndex,
                options
            );

            return newIndex;
        }

        public static void ShowHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}