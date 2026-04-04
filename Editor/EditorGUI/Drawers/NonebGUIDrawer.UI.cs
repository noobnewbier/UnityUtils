using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityUtils.Editor
{
    public partial class NonebGUIDrawer
    {
        private readonly Dictionary<string, bool> _foldoutStates = new ();
        private readonly Dictionary<string, int> _toolbarStates = new ();

        public NonebGUIDrawer(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
        }

        public NonebGUIDrawer(Object obj)
        {
            _serializedObject = new (obj);
        }


        //todo: maybe? https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute?view=net-6.0

        public bool DrawButton(string label)
        {
            var rect = GetIndentedRect(label, GUI.skin.button);
            return GUI.Button(rect, label);
        }

        public void DrawError(string errorText)
        {
            var rect = GetIndentedRect(errorText, NonebGUIStyle.Error);
            DrawError(rect, errorText);
        }

        public void DrawError(Rect rect, string errorText)
        {
            GUI.Label(rect, errorText, NonebGUIStyle.Error);
        }

        public void DrawHeader(string headerText)
        {
            // The inconsistent use of editor styles here feels weird
            var rect = GetIndentedRect(headerText, EditorStyles.boldLabel);
            GUI.Label(rect, headerText, EditorStyles.boldLabel);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }

        public void DrawLabel(string label)
        {
            var rect = GetIndentedRect(label, NonebGUIStyle.Normal);
            GUI.Label(rect, label, NonebGUIStyle.Normal);
        }

        public void DrawHint(string label)
        {
            var rect = GetIndentedRect(label, NonebGUIStyle.Hint);
            GUI.Label(rect, label, NonebGUIStyle.Hint);
        }

        public bool DrawNonebInspector(UnityEditor.Editor editor)
        {
            bool flag;
            using (new LocalizationGroup(editor.target))
            {
                // Copied from DoDrawDefaultInspector and changed content of for loop
                EditorGUI.BeginChangeCheck();
                var serializedObj = editor.serializedObject;
                serializedObj.UpdateIfRequiredOrScript();
                var iterator = serializedObj.GetIterator();
                for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
                {
                    if (iterator.propertyPath.StartsWith("editorData")) continue;

                    using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }

                serializedObj.ApplyModifiedProperties();
                flag = EditorGUI.EndChangeCheck();
            }

            return flag;
        }

        public bool DrawDefaultInspector(UnityEditor.Editor editor) => DoDrawDefaultInspector(editor);

        private static bool DoDrawDefaultInspector(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            var iterator = obj.GetIterator();
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (iterator.propertyPath.StartsWith("editorData")) continue;

                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }

            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        private static bool DoDrawDefaultInspector(UnityEditor.Editor editor)
        {
            bool flag;
            using (new LocalizationGroup(editor.target))
            {
                flag = DoDrawDefaultInspector(editor.serializedObject);

                /*
                 * Commented out as we don't need it for our custom inspector,
                 * we can regret and use reflection(these are all internal) later if it becomes a problem
                 */
                // MonoBehaviour target = this.target as MonoBehaviour;
                // if ((UnityEngine.Object) target == (UnityEngine.Object) null || !AudioUtil.HasAudioCallback(target) || AudioUtil.GetCustomFilterChannelCount(target) <= 0)
                //     return flag;
                // if (this.m_AudioFilterGUI == null)
                //     this.m_AudioFilterGUI = new AudioFilterGUI();
                // this.m_AudioFilterGUI.DrawAudioFilterGUI(target);
            }

            return flag;
        }

        public bool Foldout(string label) => Foldout(label, label);

        public bool Foldout(string label, string id)
        {
            if (!_foldoutStates.TryGetValue(id, out var state)) _foldoutStates[id] = state = false;

            return _foldoutStates[id] = EditorGUILayout.Foldout(state, label);
        }

        public void DrawListFoldoutHeader(SerializedProperty listProperty, string label)
        {
            //Copying from https://github.com/Unity-Technologies/UnityCsReference/blob/b1cf2a8251cce56190f455419eaa5513d5c8f609/Editor/Mono/Inspector/ReorderableListWrapper.cs#L209, with some tweaks to make everything compiles
            const float headerPadding = 3f; // todo: maybe handle this as well if not too hassle
            const float arraySizeWidth = 48f;
            const float defaultFoldoutHeaderHeight = 18;

            var style = EditorStyles.foldoutHeader;
            var rect = GetIndentedRect(label, style);
            var headerRect = new Rect(rect.x, rect.y, rect.width - arraySizeWidth, defaultFoldoutHeaderHeight);
            var sizeRect = new Rect
            (
                headerRect.xMax - Indent * EditorGUI.indentLevel,
                headerRect.y,
                arraySizeWidth + Indent * EditorGUI.indentLevel,
                defaultFoldoutHeaderHeight
            );

            if (!listProperty.isArray) DrawError(rect, "This is not even a list, wtf are you doing");

            listProperty.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerRect, listProperty.isExpanded, label);
            EditorGUI.EndFoldoutHeaderGroup();
            var sizeProperty = GetOrFindProperty(listProperty, "Array.size");
            EditorGUI.PropertyField(sizeRect, sizeProperty, GUIContent.none);
        }

        public bool DrawToggle(string label, bool value) => EditorGUILayout.Toggle(label, value);

        public void DrawToolbar(params (string tabName, Action drawTabFunc)[] tabs)
        {
            if (tabs.Length == 0)
            {
                return;
            }

            var id = string.Join("_", tabs.Select(t => t.tabName));
            DrawToolbar(id, tabs);
        }

        public void DrawToolbar(string id, params (string tabName, Action drawTabFunc)[] tabs)
        {
            if (tabs.Length == 0) return;
            
            if (!_toolbarStates.TryGetValue(id, out var currSelectedIndex))
            {
                _toolbarStates[id] = currSelectedIndex = 0;
            }
            
            // in case the tabs length changed we want to make sure it remains in the valid range of values.
            currSelectedIndex = Mathf.Clamp(currSelectedIndex, 0, tabs.Length - 1);

            var tabNames = tabs.Select(t => t.tabName).ToArray();
            var newSelectedIndex = GUILayout.Toolbar(currSelectedIndex, tabNames);
            var newSelectedTab = tabs[newSelectedIndex];
            
            newSelectedTab.drawTabFunc.Invoke();
            _toolbarStates[id] = newSelectedIndex;
        }
    }
}