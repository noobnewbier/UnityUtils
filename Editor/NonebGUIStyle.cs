using System;
using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public static class NonebGUIStyle
    {
        private static readonly Lazy<Texture2D> SceneHelpBoxBackgroundTexture =
            new Lazy<Texture2D>(
                () => CreateBackgroundTexture(
                    new Color(
                        0.19f,
                        0.19f,
                        0.19f,
                        0.65f
                    )
                )
            );

        private static readonly Lazy<Texture2D> SceneErrorBoxBackgroundTexture =
            new Lazy<Texture2D>(
                () => CreateBackgroundTexture(
                    new Color(
                        0.19f,
                        0.19f,
                        0.19f,
                        0.35f
                    )
                )
            );

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

        public static readonly GUIStyle SceneHelpWindow = new GUIStyle(EditorStyles.helpBox)
        {
            normal = new GUIStyleState
            {
                background = SceneHelpBoxBackgroundTexture.Value
            }
        };

        public static readonly GUIStyle SceneErrorBox = new GUIStyle(EditorStyles.helpBox)
        {
            normal = new GUIStyleState
            {
                background = SceneErrorBoxBackgroundTexture.Value,
                textColor = Error.normal.textColor
            },
            alignment = TextAnchor.UpperLeft,
            fontSize = EditorStyles.label.fontSize
        };

        private static Texture2D CreateBackgroundTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();

            return texture;
        }
    }
}