using System;
using UnityEditor;

namespace UnityUtils.Editor
{
    public static class NonebEditorUtils
    {
        public static string GetPropertyBindingPath(string propertyName)
        {
            return $"<{propertyName}>k__BackingField";
        }

        public static T? FindPropertyObjectReferenceInSameDepth<T>(
            SerializedProperty property,
            string targetPropertyName)
            where T : class
        {
            var targetPath = targetPropertyName;
            var objectPath = property.propertyPath;
            var lastDotIndex = objectPath.LastIndexOf(".", StringComparison.Ordinal);
            var isNestedProperty = lastDotIndex != -1;
            if (isNestedProperty) targetPath = objectPath.Substring(0, lastDotIndex) + "." + targetPath;

            var targetProperty = property.serializedObject.FindProperty(targetPath);
            return targetProperty?.objectReferenceValue as T;
        }

        public static int FindPropertyIntInSameDepth(SerializedProperty property, string targetPropertyName)
        {
            var targetPath = targetPropertyName;
            var objectPath = property.propertyPath;
            var lastDotIndex = objectPath.LastIndexOf(".", StringComparison.Ordinal);
            var isNestedProperty = lastDotIndex != -1;
            if (isNestedProperty) targetPath = objectPath.Substring(0, lastDotIndex) + "." + targetPath;

            var targetProperty = property.serializedObject.FindProperty(targetPath);
            return targetProperty.intValue;
        }
    }
}