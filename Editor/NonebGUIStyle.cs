using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public static class NonebGUIStyle
    {
        public static readonly GUIStyle Normal = EditorStyles.label;

        public static readonly GUIStyle Error = new GUIStyle(EditorStyles.label)
        {
            normal = new GUIStyleState
            {
                textColor = Color.red
            }
        };

        public static readonly GUIStyle Hint = new GUIStyle(EditorStyles.label)
        {
            normal = new GUIStyleState
            {
                textColor = Color.grey
            }
        };
    }
}