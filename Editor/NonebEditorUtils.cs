using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityUtils.Editor
{
    public static class NonebEditorUtils
    {
        public static string GetPropertyBindingPath(string propertyName) => $"<{propertyName}>k__BackingField";

        public static T? FindPropertyObjectReferenceInSameDepth<T>(
            SerializedProperty property,
            string targetPropertyName)
            where T : class
        {
            var targetProperty = FindPropertyInSameDepth(property, targetPropertyName);
            if (targetProperty == null)
            {
                Debug.LogError("Cannot find property with the given path, likely unintended!");
                return null;
            }

            return targetProperty.objectReferenceValue as T;
        }

        public static int FindPropertyIntInSameDepth(SerializedProperty property, string targetPropertyName)
        {
            var targetProperty = FindPropertyInSameDepth(property, targetPropertyName);
            if (targetProperty == null)
            {
                Debug.LogError("Cannot find property with the given path, likely unintended!");
                return 0;
            }

            return targetProperty.intValue;
        }

        private static SerializedProperty? FindPropertyInSameDepth(SerializedProperty property, string targetPropertyName)
        {
            var fieldProperty = DoFindProperty(targetPropertyName);
            if (fieldProperty != null) return fieldProperty;

            var propertyBindingPath = GetPropertyBindingPath(targetPropertyName);
            var propertyBackingFieldProperty = DoFindProperty(propertyBindingPath);

            return propertyBackingFieldProperty;

            SerializedProperty? DoFindProperty(string propName)
            {
                var targetPath = propName;
                var objectPath = property.propertyPath;
                var lastDotIndex = objectPath.LastIndexOf(".", StringComparison.Ordinal);
                var isNestedProperty = lastDotIndex != -1;
                if (isNestedProperty) targetPath = objectPath[..lastDotIndex] + "." + targetPath;

                var targetProperty = property.serializedObject.FindProperty(targetPath);
                return targetProperty;
            }
        }

        /// <summary>
        /// Note: this includes sub-asset!
        /// </summary>
        public static IEnumerable<T> LoadAllAssetsInFolder<T>(string folderPath) where T : Object
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);

                yield return asset;
            }
            //todo: combine
        }

        public static IEnumerable<T> LoadAllMainAssetsInFolder<T>(string folderPath) where T : Object
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (AssetDatabase.IsValidFolder(path)) continue;

                if (!AssetDatabase.IsMainAsset(asset)) continue;

                yield return asset;
            }
        }

        public static IEnumerable<Object> LoadAllMainAssetsInFolder(string folderPath) => LoadAllAssetsInFolder<Object>(folderPath);

        public static string ToProjectRelativePath(this string path) => path.Replace(Application.dataPath[..^6], string.Empty);

        public class AssetDatabaseEditingScope : IDisposable
        {
            private bool _disposed;

            public AssetDatabaseEditingScope()
            {
                AssetDatabase.StartAssetEditing();
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private static void CloseScope()
            {
                AssetDatabase.StopAssetEditing();
            }

            ~AssetDatabaseEditingScope()
            {
                DoDispose(false);
            }
        }

        public class EditorLabelWidthScope : IDisposable
        {
            private readonly float _cacheLabelWidth;
            private bool _disposed;

            public EditorLabelWidthScope(string targetLabel) : this(EditorStyles.label.CalcSize(new GUIContent(targetLabel)).x) { }

            public EditorLabelWidthScope(float targetWidth)
            {
                _cacheLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = targetWidth;
            }

            public void Dispose()
            {
                DoDispose(true);
                GC.SuppressFinalize(this);
            }

            private void DoDispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                    CloseScope();

                _disposed = true;
            }

            private void CloseScope()
            {
                EditorGUIUtility.labelWidth = _cacheLabelWidth;
            }

            ~EditorLabelWidthScope()
            {
                DoDispose(false);
            }
        }
    }
}