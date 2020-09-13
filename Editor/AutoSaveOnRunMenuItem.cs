using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//https://forum.unity.com/threads/we-need-auto-save-feature.483853/
namespace UnityUtils.Editor
{
    [InitializeOnLoad]
    public class AutoSaveOnRunMenuItem
    {
        private const string MenuName = "Custom/Autosave On Run";
        private static bool _isToggled;

        static AutoSaveOnRunMenuItem()
        {
            EditorApplication.delayCall += () =>
            {
                _isToggled = EditorPrefs.GetBool(MenuName, false);
                Menu.SetChecked(MenuName, _isToggled);
                SetMode();
            };
        }

        [MenuItem(MenuName)]
        private static void ToggleMode()
        {
            _isToggled = !_isToggled;
            Menu.SetChecked(MenuName, _isToggled);
            EditorPrefs.SetBool(MenuName, _isToggled);
            SetMode();
        }

        private static void SetMode()
        {
            if (_isToggled)
                EditorApplication.playModeStateChanged += AutoSaveOnRun;
            else
                EditorApplication.playModeStateChanged -= AutoSaveOnRun;
        }

        private static void AutoSaveOnRun(PlayModeStateChange state)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("Auto-Saving before entering Play mode");

                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    }
}