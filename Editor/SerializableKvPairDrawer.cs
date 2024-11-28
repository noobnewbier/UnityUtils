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
            if (!IsKeyCompact)
            {
                EditorGUI.PropertyField(position, property, label, true);

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
                    valueRect.y += EditorGUIUtility.singleLineHeight;
                    valueRect.height -= EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(valueRect, _valueProperty, new GUIContent(_valueProperty.displayName));
                }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);
            if (!IsKeyCompact) return EditorGUI.GetPropertyHeight(property, label, true);
            if (_valueProperty.isExpanded) return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(_valueProperty, new GUIContent("Value"));

            return EditorGUIUtility.singleLineHeight;
        }
    }
}