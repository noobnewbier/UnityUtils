using UnityEngine;

namespace UnityUtils.Editor
{
    public static partial class NonebEditorGUI
    {
        private static readonly GUIContent CachedContent = new ();

        public static GUIContent TempContent(string text, string tooltip = "")
        {
            CachedContent.text = text;
            CachedContent.tooltip = tooltip;
            CachedContent.image = null;
            return CachedContent;
        }

        public static GUIContent TempContent(Texture image, string tooltip = "")
        {
            CachedContent.image = image;
            CachedContent.tooltip = tooltip;
            CachedContent.text = null;
            return CachedContent;
        }

        public static GUIContent TempContent(string text, Texture image, string tooltip = "")
        {
            CachedContent.text = text;
            CachedContent.image = image;
            CachedContent.tooltip = tooltip;
            return CachedContent;
        }
    }
}