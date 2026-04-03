using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityUtils.Editor
{
    public partial class NonebGUIDrawer
    {
        private readonly Dictionary<SerializedProperty, Dictionary<string, SerializedProperty?>> _nestedPropertyCache = new ();
        private readonly Dictionary<string, SerializedProperty?> _propertyCache = new ();
        private readonly Dictionary<string, ReorderableList> _reorderableListCache = new ();
        private readonly SerializedObject _serializedObject;

        public void DrawProperty(string propertyPath, string label = "", bool isCompact = false)
        {
            var property = GetOrFindProperty(propertyPath);
            if (property == null)
            {
                DrawError($"Cannot find property with name == {propertyPath}");
                return;
            }

            if (string.IsNullOrEmpty(label)) label = property.displayName;

            if (isCompact)
                using (EditorLabelWidthScope(label))
                {
                    EditorGUILayout.PropertyField(property, new GUIContent(label));
                }
            else
                EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        public void DrawNestedProperty(string pathToObj, string relativePathToField, string label = "")
        {
            var objProperty = GetOrFindProperty(pathToObj);
            if (objProperty == null)
            {
                DrawError($"Cannot find property with name == {pathToObj}");
                return;
            }

            var fieldProperty = GetOrFindProperty(objProperty, relativePathToField);
            if (fieldProperty == null)
            {
                DrawError($"Cannot find property with name == {relativePathToField}");
                return;
            }

            if (string.IsNullOrEmpty(label)) label = fieldProperty.displayName;
            EditorGUILayout.PropertyField(fieldProperty, new GUIContent(label));
        }

        public void DrawEditorDataProperty(string dataFieldName, string label = "")
        {
            // I can/will regret later
            DrawNestedProperty("editorData", dataFieldName, label);
        }

        public SerializedProperty? FindProperty(string propertyName) => GetOrFindProperty(propertyName);

        private SerializedProperty? GetOrFindProperty(string propertyName)
        {
            if (!_propertyCache.TryGetValue(propertyName, out var targetProperty))
            {
                targetProperty = _serializedObject.FindProperty(propertyName);
                if (targetProperty == null)
                {
                    var backingFieldPath = NonebEditorGUI.GetPropertyBindingPath(propertyName);
                    targetProperty = _serializedObject.FindProperty(backingFieldPath);
                }

                _propertyCache[propertyName] = targetProperty;
            }

            return targetProperty;
        }

        private SerializedProperty? GetOrFindProperty(SerializedProperty nestedProperty, string propertyName)
        {
            if (!_nestedPropertyCache.TryGetValue(nestedProperty, out var cache)) _nestedPropertyCache[nestedProperty] = cache = new Dictionary<string, SerializedProperty?>();

            if (!cache.TryGetValue(propertyName, out var targetProperty))
            {
                targetProperty = nestedProperty.FindPropertyRelative(propertyName);
                if (targetProperty == null)
                {
                    var backingFieldPath = NonebEditorGUI.GetPropertyBindingPath(propertyName);
                    targetProperty = nestedProperty.FindPropertyRelative(backingFieldPath);
                }

                cache[propertyName] = targetProperty;
            }

            return targetProperty;
        }
    }
}