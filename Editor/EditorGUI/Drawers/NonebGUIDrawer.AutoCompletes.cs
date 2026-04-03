using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace UnityUtils.Editor
{
    public partial class NonebGUIDrawer
    {
        private readonly Dictionary<string, AutoCompleteField> _autoCompleteFieldsCache = new ();

        public void DrawAutoCompleteProperty(string propertyPath, Func<IEnumerable<string>> autoCompleteOptionsFactory, string label = "")
        {
            var property = GetOrFindProperty(propertyPath);
            if (property == null)
            {
                DrawError($"Cannot find property with name == {propertyPath}");
                return;
            }

            var editedType = property.GetTargetType();
            if (editedType == null)
            {
                DrawError($"I have no idea what is a {propertyPath}");
                return;
            }

            if (editedType != typeof(string) && editedType.GetTypeWithinCollection() != typeof(string))
            {
                DrawError($"We can only work with strings here! And {propertyPath} a {editedType}");
                return;
            }

            if (string.IsNullOrEmpty(label)) label = property.displayName;
            if (editedType.IsArray || editedType.IsUnitySupportedCollection())
            {
                if (!_reorderableListCache.TryGetValue(propertyPath, out var reorderableList))
                {
                    _reorderableListCache[propertyPath] = reorderableList = new ReorderableList(property.serializedObject, property, true, false, true, true);
                    reorderableList.drawElementCallback = (rect, index, _, _) =>
                    {
                        var subElementPath = property.GetArrayElementAtIndex(index);
                        if (subElementPath.propertyPath == null)
                        {
                            DrawError(rect, $"Cannot find element at index == {index}");
                            return;
                        }

                        var subAutoCompleteField = GetOrCreateAutoCompleteField(subElementPath.propertyPath, autoCompleteOptionsFactory);
                        subAutoCompleteField.OnGUI(rect, false);
                    };
                    reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
                }

                // property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, EditorStyles.foldoutHeader);
                DrawListFoldoutHeader(property, label);
                if (property.isExpanded) reorderableList.DoLayoutList();

                return;
            }

            var autoCompleteField = GetOrCreateAutoCompleteField(propertyPath, autoCompleteOptionsFactory);
            autoCompleteField.OnGUI();
        }

        private AutoCompleteField GetOrCreateAutoCompleteField(string propertyPath, Func<IEnumerable<string>> autoCompleteOptionsFactory)
        {
            var property = GetOrFindProperty(propertyPath);
            if (property == null)
            {
                const string errorFieldKey = ",InvalidPropertyPath,";
                if (!_autoCompleteFieldsCache.TryGetValue(errorFieldKey, out var errorField))
                    _autoCompleteFieldsCache[errorFieldKey] = errorField = new (
                        null,
                        null,
                        Array.Empty<string>,
                        new ("ERROR_NO_PROPERTY"),
                        "cannot find property - should neve get here!"
                    );

                return errorField;
            }

            if (!_autoCompleteFieldsCache.TryGetValue(propertyPath, out var autoCompleteField)) _autoCompleteFieldsCache[propertyPath] = autoCompleteField = new AutoCompleteField(property, autoCompleteOptionsFactory);

            return autoCompleteField;
        }
    }
}