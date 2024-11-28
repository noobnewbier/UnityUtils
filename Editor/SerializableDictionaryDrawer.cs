using UnityEditor;
using UnityEngine;
using UnityUtils.Serialization;

namespace UnityUtils.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const string DuplicateKeyErrorMessage = "Duplicated Keys! You must resolve them for a valid dictionary";

        private SerializedProperty _invalidPairsProp = null!;
        private SerializedProperty _validPairsProp = null!;

        private void FindProperties(SerializedProperty property)
        {
            _validPairsProp = property.FindPropertyRelative("validKvPairs");
            _invalidPairsProp = property.FindPropertyRelative("invalidKvPairs");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, _validPairsProp, label, true);
            if (_invalidPairsProp.arraySize <= 0) return;

            position.y += EditorGUI.GetPropertyHeight(_validPairsProp, true);
            using (new EditorGUI.IndentLevelScope())
            {
                var helpBoxRect = position;
                helpBoxRect.height = GetHelpBoxHeight();
                EditorGUI.HelpBox(helpBoxRect, DuplicateKeyErrorMessage, MessageType.Error);

                position.y += helpBoxRect.height;
                EditorGUI.PropertyField(position, _invalidPairsProp, new GUIContent("Duplicated Key"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            var height = EditorGUI.GetPropertyHeight(_validPairsProp, true);
            if (_invalidPairsProp.arraySize <= 0) return height;

            height += GetHelpBoxHeight();
            height += EditorGUI.GetPropertyHeight(_invalidPairsProp, true);

            return height;
        }

        private static float GetHelpBoxHeight() => EditorGUIUtility.standardVerticalSpacing * EditorGUIUtility.singleLineHeight;
    }
}