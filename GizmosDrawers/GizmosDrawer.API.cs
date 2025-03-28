using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace UnityUtils
{
    public partial class GizmosDrawer
    {
        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 from, Vector3 to, float duration = 0f, string label = "")
        {
            DrawLine(from, to, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 from, Vector3 to, Color color, float duration = 0f, string label = "")
        {
            var request = new LineRequest(color, duration, from, to);
            Request(request);
            DrawLabel(from, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3[] points, float duration = 0f, string label = "")
        {
            DrawLine(points, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3[] points, Color color, float duration = 0f, string label = "")
        {
            if (!points.Any()) return;

            for (var i = 0; i < points.Length - 1; i++)
            {
                var from = points[i];
                var to = points[i + 1];
                var request = new LineRequest(color, duration, from, to);

                Request(request);
            }

            DrawLabel(points.First(), label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(Vector3 center, float radius, float duration = 0f, string label = "")
        {
            DrawWireSphere(center, radius, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration = 0f, string label = "")
        {
            var request = new WireSphereRequest(color, duration, center, radius);
            Request(request);
            DrawLabel(center, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Vector3 center, float radius, float duration = 0f, string label = "")
        {
            DrawSphere(center, radius, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawSphere(Vector3 center, float radius, Color color, float duration = 0f, string label = "")
        {
            var request = new SphereRequest(color, duration, center, radius);
            Request(request);
            DrawLabel(center, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireCube(Vector3 center, Vector3 size, float duration = 0f, string label = "")
        {
            DrawWireCube(center, size, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireCube(Vector3 center, Vector3 size, Color color, float duration = 0f, string label = "")
        {
            var request = new WireCubeRequest(color, duration, center, size);
            Request(request);
            DrawLabel(center, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawCube(Vector3 center, Vector3 size, float duration = 0f, string label = "")
        {
            DrawCube(center, size, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawCube(Vector3 center, Vector3 size, Color color, float duration = 0f, string label = "")
        {
            var request = new CubeRequest(color, duration, center, size);
            Request(request);
            DrawLabel(center, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 from, Vector3 direction, float length = 2.5f, float duration = 0f, string label = "")
        {
            DrawRay(from, direction, Color.red, length);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 from, Vector3 direction, Color color, float length = 2.5f, float duration = 0f, string label = "")
        {
            var to = from + direction.normalized * length;
            var request = new LineRequest(color, duration, from, to);
            Request(request);
            DrawLabel(from, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, float duration = 0f, string label = "")
        {
            DrawMesh(mesh, position, rotation, scale, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0f, string label = "")
        {
            var request = new MeshRequest(color, duration, mesh, position, rotation, scale, false);
            Request(request);
            DrawLabel(position, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, float duration = 0f, string label = "")
        {
            DrawMesh(mesh, position, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawMesh(Mesh mesh, Vector3 position, Color color, float duration = 0f, string label = "")
        {
            var request = new MeshRequest(color, duration, mesh, position, Quaternion.identity, Vector3.one, false);
            Request(request);
            DrawLabel(position, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, float duration = 0f, string label = "")
        {
            DrawWireMesh(mesh, position, rotation, scale, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0f, string label = "")
        {
            var request = new MeshRequest(color, duration, mesh, position, rotation, scale, true);
            Request(request);
            DrawLabel(position, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, float duration = 0f, string label = "")
        {
            DrawWireMesh(mesh, position, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Color color, float duration = 0f, string label = "")
        {
            var request = new MeshRequest(color, duration, mesh, position, Quaternion.identity, Vector3.one, true);
            Request(request);
            DrawLabel(position, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, float duration = 0f, string label = "")
        {
            DrawWireMesh(mesh, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireMesh(Mesh mesh, Color color, float duration = 0f, string label = "")
        {
            var request = new MeshRequest(color, duration, mesh, Vector3.zero, Quaternion.identity, Vector3.one, true);
            Request(request);
            DrawLabel(Vector3.zero, label, color, duration);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Ray r, float duration = 0f, string label = "")
        {
            DrawRay(r, Color.red, duration, label);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawRay(Ray r, Color color, float duration = 0f, string label = "")
        {
            DrawRay(r.origin, r.direction, color);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireDisc(float rad, Vector3 position, Color color, float duration = 0f, string label = "")
        {
            var request = new WireDiscRequest(color, duration, rad, position);
            Request(request);
            DrawLabel(position, label, color, duration);
        }

        private static void DrawLabel(Vector3 position, string label, Color color, float duration = 0f)
        {
            var request = new LabelRequest(color, duration, label, position);
            Request(request);
        }
    }
}