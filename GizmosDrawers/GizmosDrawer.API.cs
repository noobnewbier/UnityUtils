using System.Diagnostics;
using UnityEngine;

namespace UnityUtils
{
    public partial class GizmosDrawer
    {
        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 from, Vector3 to, Color color = new())
        {
            var request = new LineRequest(color, from, to);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawLineList(Vector3[] points, Color color = new())
        {
            for (var i = 0; i < points.Length - 1; i++)
            {
                var from = points[i];
                var to = points[i + 1];
                var request = new LineRequest(color, from, to);

                Request(request);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(Vector3 center, float radius, Color color = new())
        {
            var request = new WireSphereRequest(color, center, radius);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Vector3 center, float radius, Color color = new())
        {
            var request = new SphereRequest(color, center, radius);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireCube(Vector3 center, Vector3 size, Color color = new())
        {
            var request = new WireCubeRequest(color, center, size);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawCube(Vector3 center, Vector3 size, Color color = new())
        {
            var request = new CubeRequest(color, center, size);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 from, Vector3 direction, float length = 2.5f, Color color = new())
        {
            var to = from + direction.normalized * length;
            var request = new LineRequest(color, from, to);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color = new())
        {
            var request = new MeshRequest(color, mesh, position, rotation, scale, false);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Color color = new())
        {
            var request = new MeshRequest(color, mesh, position, Quaternion.identity, Vector3.one, false);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color = new())
        {
            var request = new MeshRequest(color, mesh, position, rotation, scale, true);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Color color = new())
        {
            var request = new MeshRequest(color, mesh, position, Quaternion.identity, Vector3.one, true);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Color color = new())
        {
            var request = new MeshRequest(color, mesh, Vector3.zero, Quaternion.identity, Vector3.one, true);
            Request(request);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Ray r, Color color = new())
        {
            DrawRay(r.origin, r.direction, color: color);
        }
    }
}