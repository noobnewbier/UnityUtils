using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public static class NonebGUIStyle
    {
        private static readonly Lazy<Texture2D> SceneHelpBoxBackgroundTexture = new Lazy<Texture2D>(CreateBackgroundTexture);

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
                textColor = new Color(0.62f, 0.62f, 0.62f)
            }
        };

        public static readonly GUIStyle Title = new GUIStyle(EditorStyles.whiteLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };

        public static readonly GUIStyle SceneHelpBox = new GUIStyle(EditorStyles.helpBox)
        {
            normal = new GUIStyleState
            {
                background = SceneHelpBoxBackgroundTexture.Value
            }
        };

        private static Texture2D CreateBackgroundTexture()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(
                0,
                0,
                new Color(
                    0.19f,
                    0.19f,
                    0.19f,
                    0.65f
                )
            );
            texture.Apply();

            return texture;
        }
    }
}