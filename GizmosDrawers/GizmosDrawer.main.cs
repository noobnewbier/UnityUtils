using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityUtils
{
    [InitializeOnLoad]
    public partial class GizmosDrawer
    {
        private static readonly HashSet<DrawRequest> Requests = new();

        static GizmosDrawer()
        {
            Requests.Clear();
            CreateRunnerIfNotExist();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnRuntimeInitialized()
        {
            CreateRunnerIfNotExist();
        }

        private static void CreateRunnerIfNotExist()
        {
            if (!Application.isPlaying)
                if (Resources.FindObjectsOfTypeAll<Runner>().Any())
                    return;

            /*
             * Unity doesn't run gizmos drawing if the object itself is hidden.
             * So no, no hide flags.
             */
            var runner = new GameObject("AutoDeletedEditorHelper").AddComponent<Runner>();


            if (Application.isPlaying) Object.DontDestroyOnLoad(runner.gameObject);
        }

        private static void Request(DrawRequest request)
        {
            Requests.Add(request);
        }

        private class Runner : MonoBehaviour
        {
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
                foreach (var request in Requests) request.Draw();
            }
        }
    }
}