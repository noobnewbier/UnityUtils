using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityUtils.Serialization;

namespace UnityUtils.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>.SerializableKvPair))]
    public class SerializableKvPairDrawer : PropertyDrawer
    {
        //One day we might want to put this on the editor folder to use our utils, but for now this suffice 
        private SerializedProperty _keyProperty = null!;
        private SerializedProperty _valueProperty = null!;

        private bool IsKeyCompact
        {
            get
            {
                var targetType = _keyProperty.GetTargetType();

                if (targetType == null) return false;

                return targetType.CanShowInOneLine();
            }
        }

        private void FindProperties(SerializedProperty property)
        {
            _keyProperty = property.NFindPropertyRelative("Key")!;
            _valueProperty = property.NFindPropertyRelative("Value")!;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            //overriding the label - it's never that useful to have "Element {num}" over this.
            const string labelPrefix = "Element "; // Unity's label by default - "Element {Index}".
            var indexString = int.TryParse(new string(label.text.Skip(labelPrefix.Length).ToArray()), out var result) ?
                result.ToString() :
                "?";
            var keyInfo = _keyProperty.GetTargetObjectOfProperty()?.ToString();
            if (!string.IsNullOrWhiteSpace(keyInfo)) label.text = $"({indexString}) {keyInfo}";

            if (!IsKeyCompact)
            {
                //Need to draw child manually otherwise indentation isn't right.
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property, label);
                if (property.isExpanded)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        position = EditorGUI.IndentedRect(position);

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        position.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(position, _keyProperty, true);

                        position.y += EditorGUI.GetPropertyHeight(_keyProperty, true);
                        position.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(position, _valueProperty, true);
                    }

                return;
            }

            //Inlining the foldout
            var foldoutRect = position;
            foldoutRect.height = EditorGUIUtility.singleLineHeight;
            foldoutRect.width = EditorGUIUtility.labelWidth;
            _valueProperty.isExpanded = EditorGUI.Foldout(foldoutRect, _valueProperty.isExpanded, label);

            //Drawing the key next to the foldout tab
            var keyRect = NonebEditorUtils.ShiftRect(position, foldoutRect);
            keyRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(keyRect, _keyProperty, GUIContent.none);

            //Draw value's content
            if (_valueProperty.isExpanded)
                using (new EditorGUI.IndentLevelScope())
                {
                    var valueRect = EditorGUI.IndentedRect(position);
                    valueRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    valueRect.height = EditorGUIUtility.singleLineHeight;

                    if (_valueProperty.HasCustomPropertyDrawer() || _valueProperty.HasDefaultDrawer())
                        EditorGUI.PropertyField(valueRect, _valueProperty, new GUIContent(_valueProperty.displayName));
                    else
                        NonebEditorUtils.DrawDefaultPropertyWithoutFoldout(valueRect, _valueProperty);
                }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            if (!IsKeyCompact) return EditorGUI.GetPropertyHeight(property, label, true);
            if (_valueProperty.isExpanded)
            {
                if (_valueProperty.HasCustomPropertyDrawer() || _valueProperty.HasDefaultDrawer()) return EditorGUI.GetPropertyHeight(_valueProperty, new GUIContent("Value")) + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                //Reduce height by one line as it's eaten by the shared foldout by top level
                return NonebEditorUtils.GetDefaultPropertyDrawerWithoutHeight(property) - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}