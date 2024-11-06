﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityUtils.Editor
{
    public static class NonebEditorUtils
    {
        public static string GetPropertyBindingPath(string propertyName) => $"<{propertyName}>k__BackingField";

        public static T? FindPropertyOfTypeAtRoot<T>(this SerializedObject serializedObject) where T : class
        {
            var prop = serializedObject.GetIterator();
            prop.Next(true);

            do
            {
                if (prop.GetTargetType() == typeof(T)) return (T)prop.boxedValue;
            } while (prop.Next(false));

            return default;
        }

        /// <summary>
        ///     Getting the field type of a serialized property.
        /// </summary>
        public static Type? GetTargetType(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                #region Simply Concrete Type Conversion

                /*
                 * These are types that we can simply do a "typeof(TYPENAME)" without using reflection to find out the actual type.
                 */
                case SerializedPropertyType.Integer:
                {
                    return property.type == "long" ?
                        typeof(int) :
                        typeof(long);
                }
                case SerializedPropertyType.Boolean:
                {
                    return typeof(bool);
                }
                case SerializedPropertyType.Float:
                {
                    return property.type == "double" ?
                        typeof(double) :
                        typeof(float);
                }
                case SerializedPropertyType.String:
                {
                    return typeof(string);
                }
                case SerializedPropertyType.Color:
                {
                    return typeof(Color);
                }
                case SerializedPropertyType.LayerMask:
                {
                    return typeof(LayerMask);
                }
                case SerializedPropertyType.Vector2:
                {
                    return typeof(Vector2);
                }
                case SerializedPropertyType.Vector3:
                {
                    return typeof(Vector3);
                }
                case SerializedPropertyType.Vector4:
                {
                    return typeof(Vector4);
                }
                case SerializedPropertyType.Rect:
                {
                    return typeof(Rect);
                }
                case SerializedPropertyType.ArraySize:
                {
                    return typeof(int);
                }
                case SerializedPropertyType.Character:
                {
                    return typeof(char);
                }
                case SerializedPropertyType.AnimationCurve:
                {
                    return typeof(AnimationCurve);
                }
                case SerializedPropertyType.Bounds:
                {
                    return typeof(Bounds);
                }
                case SerializedPropertyType.Gradient:
                {
                    return typeof(Gradient);
                }
                case SerializedPropertyType.Quaternion:
                {
                    return typeof(Quaternion);
                }
                case SerializedPropertyType.FixedBufferSize:
                {
                    return typeof(int);
                }
                case SerializedPropertyType.Vector2Int:
                {
                    return typeof(Vector2Int);
                }
                case SerializedPropertyType.Vector3Int:
                {
                    return typeof(Vector3Int);
                }
                case SerializedPropertyType.RectInt:
                {
                    return typeof(RectInt);
                }
                case SerializedPropertyType.BoundsInt:
                {
                    return typeof(BoundsInt);
                }
                case SerializedPropertyType.Hash128:
                {
                    return typeof(Hash128);
                }

                #endregion

                #region Custom Types

                /*
                 * These are types that requires further processing if we were to find the "actual" type of the field this SerializedProperty represents
                 * Currently, we are just returning the base type that will definitely be correct, this is to avoid over complicating things if we aren't even using it.
                 *
                 * However this shouldn't be too difficult to implement either, just require slightly more work.
                 *
                 * If in doubt, reference the following code snippet to see how to implement this:
                 * https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs#L117
                 */

                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.ObjectReference:
                {
                    var foundFieldType = GetType(property);
                    if (foundFieldType != null) return foundFieldType;

                    return typeof(Object);
                }

                case SerializedPropertyType.Enum:
                {
                    return typeof(Enum);
                }

                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Generic:
                default:
                {
                    return GetType(property);
                }

                #endregion
            }

            static FieldInfo? GetFieldViaPath(SerializedObject serializedObject, Type type, string pathToFieldFromType, string pathToType)
            {
                /*
                 * In its most basic form, this method simply descend down the type hierarchy given the path (i.e. {pathToFieldFromType}.{pathToType})
                 * however this is not always possible, in particular when the path includes array,
                 * in which case to retrieve the field info we must find the instance of the array element we are "passing through", and walk down the type hierarchy of that element
                 * this is because an array of element can often contains element of more derived type, for example object[] can contains any object from Foo to Baz.
                 *
                 * Of course, this is true for fields as well! However we aren't having that issue for now, so we are kicking the can down the road at the moment as that could get a bit more complicated.
                 */
                pathToFieldFromType = pathToFieldFromType.Trim('.');
                if (!pathToFieldFromType.Contains('.')) return type.GetFieldIncludingParents(pathToFieldFromType);

                Regex arrayElementRegex = new(@"(\.Array\.data\[[0-9]+\])");
                var match = arrayElementRegex.Match(pathToFieldFromType);
                if (match.Success)
                {
                    var arrayElementEndIndex = match.Index + match.Length;
                    var pathToArrayElement = pathToFieldFromType[..arrayElementEndIndex];
                    if (!string.IsNullOrEmpty(pathToType)) pathToArrayElement = $"{pathToType}.{pathToArrayElement}";
                    var arrayElement = serializedObject.FindProperty(pathToArrayElement);

                    var elementValue = arrayElement.boxedValue;
                    var elementType = elementValue?.GetType();
                    if (elementType == null)
                    {
                        var pathToArray = pathToFieldFromType[..match.Index];
                        var arrayType = serializedObject.FindProperty(pathToArray).GetTargetType();
                        if (arrayType != null) elementType = arrayType.GetTypeWithinCollection();
                    }

                    if (elementType == null)
                    {
                        Debug.LogError("Something went wrong, should never have got here.");
                        return null;
                    }

                    return GetFieldViaPath(
                        serializedObject,
                        elementType,
                        pathToFieldFromType[arrayElementEndIndex..],
                        arrayElement.propertyPath
                    );
                }

                // If the field is nested in subclass, going down the path to find the field at the leaf level.
                var parentType = type;
                FieldInfo? currentFieldInfo = null;
                foreach (var fieldNameInEachLevel in pathToFieldFromType.Split('.'))
                {
                    currentFieldInfo = parentType.GetFieldIncludingParents(fieldNameInEachLevel);
                    if (currentFieldInfo == null)
                    {
                        Debug.LogError(
                            $"Couldn't find type given the field path({pathToFieldFromType}). This is unexpected, is the path correct?"
                        );
                        return null;
                    }

                    parentType = currentFieldInfo.FieldType;
                }

                return currentFieldInfo;
            }

            static Type? GetType(SerializedProperty property)
            {
                var parentType = property.serializedObject.targetObject.GetType();
                var fi = GetFieldViaPath(property.serializedObject, parentType, property.propertyPath, string.Empty);
                return fi?.FieldType;
            }
        }

        /// <summary>
        ///     Getting the element type within an array or a list. This assumes the given type <paramref name="t" /> is an array
        /// or a list.
        /// </summary>
        public static Type? GetTypeWithinCollection(this Type t)
        {
            if (t.IsArray) return t.GetElementType();

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) return t.GetGenericArguments()[0];

            Debug.LogError($"Type({t}) is not a List<> or array - couldn't retrieve the type within it!");
            return null;
        }

        /// <summary>
        ///     <see cref="Type.GetField(string)" /> but also looking from its base type recursively as well.
        /// <see cref="BindingFlags" /> includes non public field by default, as that's usually what I expect when working with
        /// type hierarchy.
        /// </summary>
        public static FieldInfo? GetFieldIncludingParents(
            this Type type,
            string fieldName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var fi = type.GetField(fieldName, bindingFlags);
            if (fi != null) return fi;

            var baseType = type.BaseType;
            return baseType?.GetField(fieldName, bindingFlags);
        }

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