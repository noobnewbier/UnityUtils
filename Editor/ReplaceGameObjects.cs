using UnityEditor;
using UnityEngine;
using UnityUtils.Constants;

namespace UnityUtils.Editor
{
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010

//Modified by Kristian Helle Jespersen
//June 2011

    public class ReplaceGameObjects : ScriptableWizard
    {
        public GameObject newType;
        public GameObject[] oldObjects;

        private void OnWizardCreate()
        {
            foreach (var go in oldObjects)
            {
                var newObject = (GameObject) PrefabUtility.InstantiatePrefab(newType);
                newObject.transform.position = go.transform.position;
                newObject.transform.rotation = go.transform.rotation;
                newObject.transform.parent = go.transform.parent;

                DestroyImmediate(go);
            }
        }

        [MenuItem(MenuName.Custom + "Replace GameObjects")]
        private static void CreateWizard()
        {
            DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace");
        }
    }
}