using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUtils.Serialization
{
    //TODO: imgui drawer, no way I can validate everything here and it won't be necessary either....?
    //TODO: test?
    [Serializable]
    public class SerializableDictionary<TK, TV> : IDictionary<TK, TV>, ISerializationCallbackReceiver
    {
        private readonly Dictionary<TK, TV> _dictionary = new();
        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<TK, TV> item)
        {
            _dictionary.Add(item.Key, item.Value);

            AddToSerialization(item.Key, item.Value);
        }

        public void Clear()
        {
            validKvPairs.Clear();
            invalidKvPairs.Clear();
        }

        public bool Contains(KeyValuePair<TK, TV> item) => _dictionary.Contains(item);

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TK, TV>>)_dictionary).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TK, TV> item) => ((ICollection<KeyValuePair<TK, TV>>)_dictionary).Remove(item);

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TK key, TV value)
        {
            _dictionary.Add(key, value);

            AddToSerialization(key, value);
        }

        public bool ContainsKey(TK key) => _dictionary.ContainsKey(key);

        public bool Remove(TK key)
        {
            RemoveFromSerialization(key);
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TK key, out TV value) => _dictionary.TryGetValue(key, out value);

        public TV this[TK key]
        {
            get => _dictionary[key];
            set
            {
                SetSerialization(key, value);
                _dictionary[key] = value;
            }
        }

        public ICollection<TK> Keys => _dictionary.Keys;

        public ICollection<TV> Values => _dictionary.Values;

        #region Serialziation

        [SerializeField] private List<SerializableKvPair> validKvPairs = new();

        [SerializeField] private List<SerializableKvPair> invalidKvPairs = new();

        [Serializable]
        private class SerializableKvPair
        {
            [field: SerializeField] public TK Key { get; private set; }
            [field: SerializeField] public TV Value { get; set; }

            public SerializableKvPair(TK key, TV value)
            {
                Key = key;
                Value = value;
            }
        }

        public void OnBeforeSerialize()
        {
            //NOTE: I guess I can try catch some inconsistencies here... Would it be necessary?
        }

        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            var allPairs = validKvPairs.Concat(invalidKvPairs).ToArray();

            validKvPairs.Clear();
            invalidKvPairs.Clear();
            foreach (var pair in allPairs)
            {
                if (_dictionary.TryAdd(pair.Key, pair.Value))
                {
                    validKvPairs.Add(pair);
                    continue;
                }

                invalidKvPairs.Add(pair);
            }
        }

        /*
         * TODO: editor be aware this doesn't fuck with dirtiness
         */

        [Conditional("UNITY_EDITOR")]
        private void SetSerialization(TK key, TV value)
        {
            void SetValues(IEnumerable<SerializableKvPair> kvPairs)
            {
                foreach (var pair in kvPairs.Where(pair => pair.Key.Equals(key))) pair.Value = value;
            }

            if (!validKvPairs.Any(p => p.Key.Equals(key)) &&
                !invalidKvPairs.Any(p => p.Key.Equals(key)))
            {
                AddToSerialization(key, value);
            }
            else
            {
                SetValues(validKvPairs);
                SetValues(invalidKvPairs);
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void AddToSerialization(TK key, TV value)
        {
            var newPair = new SerializableKvPair(key, value);
            if (validKvPairs.Any(p => p.Key.Equals(key)))
            {
                invalidKvPairs.Add(newPair);
                return;
            }

            validKvPairs.Add(newPair);
        }

        [Conditional("UNITY_EDITOR")]
        private void RemoveFromSerialization(TK key)
        {
            validKvPairs.RemoveAll(p => p.Key.Equals(key));
            invalidKvPairs.RemoveAll(p => p.Key.Equals(key));
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const string DuplicateKeyErrorMessage = "Duplicated Keys! You must resolve them for a valid dictionary";
        private SerializedProperty _invalidPairsProp;
        private SerializedProperty _validPairsProp;

        private void FindProperties(SerializedProperty property)
        {
            _validPairsProp = property.FindPropertyRelative("validKvPairs");
            _invalidPairsProp = property.FindPropertyRelative("invalidKvPairs");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            EditorGUI.PropertyField(position, _validPairsProp, label);
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
#endif
}