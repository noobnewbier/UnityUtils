using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils.Constants;

namespace UnityUtils.Editor
{
    //source: https://gist.github.com/zaki/f9ecd9a6bbc83d764edd93940b9316bc
    public class FindMissingScriptsEditor : EditorWindow
    {
        private static int _missingCount = -1;

        [MenuItem(MenuName.Custom + "Find Missing Scripts")]
        public static void FindMissingScripts()
        {
            GetWindow(typeof(FindMissingScriptsEditor));
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Missing Scripts:");
                EditorGUILayout.LabelField("" + (_missingCount == -1 ? "---" : _missingCount.ToString()));
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Find missing scripts"))
            {
                _missingCount = 0;
                EditorUtility.DisplayProgressBar("Searching Prefabs", "", 0.0f);

                var files = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
                EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Found " + files.Length + " prefabs", 0.0f);

                var currentScene = SceneManager.GetActiveScene();
                var scenePath = currentScene.path;
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

                for (var i = 0; i < files.Length; i++)
                {
                    var prefabPath = files[i].Replace(Application.dataPath, "Assets");
                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Processing Prefabs " + i + "/" + files.Length,
                        prefabPath,
                        i / (float) files.Length
                    ))
                    {
                        break;
                    }

                    var go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                    if (go != null)
                    {
                        FindInGo(go);
                        EditorUtility.UnloadUnusedAssetsImmediate(true);
                    }
                }

                EditorUtility.DisplayProgressBar("Cleanup", "Cleaning up", 1.0f);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                EditorUtility.UnloadUnusedAssetsImmediate(true);
                GC.Collect();

                EditorUtility.ClearProgressBar();
            }
        }

        private static void FindInGo(GameObject go, string prefabName = "")
        {
            var components = go.GetComponents<Component>();
            foreach (var t1 in components)
                if (t1 == null)
                {
                    _missingCount++;
                    var t = go.transform;

                    var componentPath = go.name;
                    while (t.parent != null)
                    {
                        var parent = t.parent;
                        componentPath = parent.name + "/" + componentPath;
                        t = parent;
                    }
                    Debug.Log("Prefab " + prefabName + " has an empty script attached:\n" + componentPath, go);
                }

            foreach (Transform child in go.transform) FindInGo(child.gameObject, prefabName);
        }
    }
}