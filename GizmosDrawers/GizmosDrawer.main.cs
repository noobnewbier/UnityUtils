using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace UnityUtils
{
    [InitializeOnLoad]
    public partial class GizmosDrawer
    {
        internal static readonly HashSet<DrawRequest> Requests = new ();
        private static readonly Stack<WeakReference> KeyStacks = new ();
        private static readonly Stack<string> CategoryStacks = new ();
        private const string DefaultCategory= "$Default";
        
        static GizmosDrawer()
        {
            Requests.Clear();
            KeyStacks.Clear();
            
            CreateRunnerIfNotExist();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnRuntimeInitialized()
        {
            if (Application.isEditor)
            {
                CreateRunnerIfNotExist();
            }
        }

        private static void CreateRunnerIfNotExist()
        {
            if (!Application.isPlaying)
            {
                var runners = Resources.FindObjectsOfTypeAll<Runner>();
                if (runners.Any())
                {
                    foreach (var run in runners)
                    {
                        run.gameObject.SetActive(true);
                    }
                    return;
                }
            }

            /*
             * Unity doesn't run gizmos drawing if the object itself is hidden.
             * So no, no hide flags.
             */
            var helperObject = new GameObject("EditorHelper"); 
            helperObject.AddComponent<Runner>();
            helperObject.hideFlags = HideFlags.DontSave;

            if (Application.isPlaying) Object.DontDestroyOnLoad(helperObject);
        }

        private static void Request(DrawRequest request)
        {
            Requests.Add(request);
        }

        [ExecuteInEditMode]
        private class Runner : MonoBehaviour
        {
            private void Awake()
            {
                if (!Application.isEditor)
                {
                    Destroy(gameObject);
                }
            }

            private void Update()
            {
                using (CollectionPool<HashSet<DrawRequest>, DrawRequest>.Get(out var expiredRequests))
                {
                    foreach (var request in Requests)
                    {
                        if (request.IsExpired) expiredRequests.Add(request);

                        request.Timer += Time.deltaTime;
                    }

                    Requests.ExceptWith(expiredRequests);
                }
            }

            private void OnDrawGizmos()
            {
                foreach (var request in Requests)
                {
                    if (GizmosDrawerPreferences.instance.FilteredCategories.Contains(request.RequestCategory))
                    {
                        continue;
                    }

                    if (!GizmosDrawerPreferences.instance.IsDrawLabel && request is LabelRequest or DynamicLabelRequest)
                    {
                        continue;
                    }
                    
                    request.Draw();
                }
            }
        }
    }
}