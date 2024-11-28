using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public static partial class NonebEditorUtils
    {
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
    }
}