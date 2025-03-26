using System.Collections.Generic;
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
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnRuntimeInitialized()
        {
            var runner = new GameObject("GizmosDrawerRunner").AddComponent<Runner>();
            Object.DontDestroyOnLoad(runner.gameObject);
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
                        request.Timer += Time.deltaTime;

                        if (request.IsExpired) expiredRequests.Add(request);
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