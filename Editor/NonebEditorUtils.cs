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
    }
}