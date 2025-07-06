using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace UnityUtils.Editor
{
    [InitializeOnLoad]
    public static partial class NonebEditorUtils
    {
        static NonebEditorUtils()
        {
            CustomPropertyDrawerTypeCache.Clear();
        }

        /// <summary>
        /// I really can't be asked to do reflection just to get that, if future me really want to look, go check
        /// UnityEditor.IMGUI.Controls.TreeViewGUI.cs
        /// </summary>
        public static float FoldoutIconWidth
        {
            get
            {
                const float foldoutIconWidth = 11f;
                const float padding = 3f;

                return foldoutIconWidth + padding;
            }
        }

        public static bool CanShowInOneLine(this Type type) =>
            type.IsPrimitive ||
            type.IsEnum ||
            type == typeof(string) ||
            type == typeof(Color) ||
            type == typeof(AnimationCurve) ||
            type == typeof(Gradient) ||
            type.IsSameOrSubclass(typeof(Object)) ||
            type.IsSameOrSubclass(typeof(Addressables));

        public static Rect ShiftRect(Rect a, Rect b) => ShiftRect(a, b.xMin - a.xMin + b.width);

        public static bool HasCustomPropertyDrawer(this SerializedProperty property)
        {
            var type = property.GetTargetType();
            if (type == null)
            {
                Debug.LogWarning($"Can't work with this property for whatever reason, can't get the type... {property.type}");
                return false;
            }

            return CustomPropertyDrawerTypeCache.GetCustomPropertyDrawerType(type) != null;
        }

        public static bool HasDefaultDrawer(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Float:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Hash128:
                case SerializedPropertyType.RenderingLayerMask:
                    return true;

                case SerializedPropertyType.Generic:
                default:
                    return false;
            }
        }

        public static Rect ShiftRect(Rect rect, float shiftInX)
        {
            const float padding = 2;
            var toReturn = rect;
            toReturn.width -= shiftInX + padding;
            toReturn.x += shiftInX + padding;

            return toReturn;
        }

        /// <summary>
        /// EditorGUI.PropertyField without also the foldout to group them up.
        /// </summary>
        public static Rect DrawDefaultPropertyWithoutFoldout(Rect position, SerializedProperty property)
        {
            property.Next(true); // get first child of your property
            var depth = property.depth;
            do
            {
                EditorGUI.PropertyField(position, property, true); // Include children
                position.y += EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.standardVerticalSpacing;
            } while (property.Next(false) && depth <= property.depth);

            return position;
        }

        public static void DrawDefaultPropertyWithoutFoldout(SerializedProperty property)
        {
            property.Next(true); // get first child of your property
            var depth = property.depth;
            do
            {
                EditorGUILayout.PropertyField(property, true); // Include children
            } while (property.Next(false) && depth <= property.depth);
        }

        public static float GetDefaultPropertyDrawerWithoutHeight(SerializedProperty property)
        {
            property.Next(true);
            var depth = property.depth;
            float result = 0;
            do
            {
                result += EditorGUI.GetPropertyHeight(property, true) + 2; // include children
            } while (property.Next(false) && depth <= property.depth);

            return result;
        }

        private static class CustomPropertyDrawerTypeCache
        {
            private static Dictionary<Type, Type>? _cache;

            private static IEnumerable<(Type drawnType, Type drawerType)> AllPropertyDrawers()
            {
                var typeFieldInfo = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic);
                if (typeFieldInfo == null)
                {
                    Debug.LogError("Failed - Reflection gone wrong");
                    yield break;
                }

                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in asm.GetTypes())
                {
                    if (!type.IsSameOrSubclass(typeof(PropertyDrawer))) continue;

                    foreach (var attribute in Attribute.GetCustomAttributes(type))
                    {
                        if (attribute is not CustomPropertyDrawer) continue;

                        var reflectedValue = typeFieldInfo.GetValue(attribute);
                        if (reflectedValue is not Type drawnType) continue;

                        var drawerType = type;
                        yield return (drawnType, drawerType);
                    }
                }
            }

            private static Dictionary<Type, Type> CreateCache()
            {
                var toReturn = new Dictionary<Type, Type>();
                foreach (var (drawnType, drawerType) in AllPropertyDrawers()) toReturn[drawnType] = drawerType;

                return toReturn;
            }

            public static Type? GetCustomPropertyDrawerType(Type type)
            {
                _cache ??= CreateCache();

                return _cache.GetValueOrDefault(type);
            }

            public static void Clear()
            {
                _cache = null;
            }
        }
    }
}