using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityUtils.Editor
{
    public static class NonebGUIStyle
    {
        public static readonly Color HintTextColor = new(0.62f, 0.62f, 0.62f, 1f);
        public static readonly Color ErrorTextColor = Color.red;

        public static readonly Color SceneHelpBoxColor = new(
            0.19f,
            0.19f,
            0.19f,
            0.65f
        );

        private static readonly Lazy<Texture2D> SceneHelpBoxBackgroundTexture =
            new(
                () => CreateBackgroundTexture(
                    SceneHelpBoxColor
                )
            );

        private static readonly Lazy<Texture2D> SceneErrorBoxBackgroundTexture =
            new(
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

        public static readonly GUIStyle Error = new(EditorStyles.label)
        {
            normal = new GUIStyleState
            {
                textColor = ErrorTextColor
            }
        };

        public static readonly GUIStyle Hint = new(EditorStyles.label)
        {
            normal = new GUIStyleState
            {
                textColor = HintTextColor
            }
        };

        public static readonly GUIStyle Box = new(GUI.skin.box)
        {
            padding =
            {
                // Magic values that looks right -> might be a terrible idea causing misalignment later but we will deal with it later.
                left = 12,
                top = 8,
                bottom = 8
            }
        };

        public static readonly GUIStyle Title = new(EditorStyles.whiteLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };

        public static readonly GUIStyle SceneHelpWindow = new(EditorStyles.helpBox)
        {
            normal = new GUIStyleState
            {
                background = SceneHelpBoxBackgroundTexture.Value
            }
        };

        public static readonly GUIStyle SceneErrorBox = new(EditorStyles.helpBox)
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
            var texture = new Texture2D(1, 1)
            {
                hideFlags = HideFlags.DontSaveInEditor
            };

            texture.SetPixel(0, 0, color);
            texture.Apply();

            return texture;
        }

        [InitializeOnLoad]
        private static class TemporaryAssetDestroyer
        {
            private static readonly bool IsEventRegistered;

            static TemporaryAssetDestroyer()
            {
                if (!IsEventRegistered)
                {
                    EditorApplication.quitting += OnQuitting;
                    IsEventRegistered = true;
                }
            }

            private static void OnQuitting()
            {
                if (SceneHelpBoxBackgroundTexture.IsValueCreated)
                    Object.DestroyImmediate(SceneHelpBoxBackgroundTexture.Value);

                if (SceneErrorBoxBackgroundTexture.IsValueCreated)
                    Object.DestroyImmediate(SceneErrorBoxBackgroundTexture.Value);
            }
        }
    }
}