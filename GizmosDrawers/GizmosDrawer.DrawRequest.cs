using UnityEngine;
using UnityUtils.Editor;

namespace UnityUtils
{
    public partial class GizmosDrawer
    {
        private abstract class DrawRequest
        {
            private readonly Color _color;

            protected DrawRequest(Color color)
            {
                _color = color;
            }

            public float Duration { get; set; }
            public float Timer { get; set; }
            public bool IsExpired => Timer >= Duration;

            public void Draw()
            {
                using (new NonebEditorUtils.GizmosColorScope(_color))
                {
                    OnDraw();
                }
            }

            protected abstract void OnDraw();
        }

        private class LineRequest : DrawRequest
        {
            private readonly Vector3 _from;
            private readonly Vector3 _to;

            public LineRequest(Color color, Vector3 from, Vector3 to) : base(color)
            {
                _from = from;
                _to = to;
            }

            protected override void OnDraw()
            {
                Gizmos.DrawLine(_from, _to);
            }
        }

        private class WireSphereRequest : DrawRequest
        {
            private readonly Vector3 _center;
            private readonly float _radius;

            public WireSphereRequest(Color color, Vector3 center, float radius) : base(color)
            {
                _center = center;
                _radius = radius;
            }

            protected override void OnDraw()
            {
                Gizmos.DrawWireSphere(_center, _radius);
            }
        }

        private class SphereRequest : DrawRequest
        {
            private readonly Vector3 _center;
            private readonly float _radius;

            public SphereRequest(Color color, Vector3 center, float radius) : base(color)
            {
                _center = center;
                _radius = radius;
            }

            protected override void OnDraw()
            {
                Gizmos.DrawSphere(_center, _radius);
            }
        }

        private class WireCubeRequest : DrawRequest
        {
            private readonly Vector3 _center;
            private readonly Vector3 _size;

            public WireCubeRequest(Color color, Vector3 center, Vector3 size) : base(color)
            {
                _center = center;
                _size = size;
            }

            protected override void OnDraw()
            {
                Gizmos.DrawWireCube(_center, _size);
            }
        }

        private class CubeRequest : DrawRequest
        {
            private readonly Vector3 _center;
            private readonly Vector3 _size;

            public CubeRequest(Color color, Vector3 center, Vector3 size) : base(color)
            {
                _center = center;
                _size = size;
            }

            protected override void OnDraw()
            {
                Gizmos.DrawWireCube(_center, _size);
            }
        }

        private class MeshRequest : DrawRequest
        {
            private readonly bool _isWired;
            private readonly Mesh _mesh;
            private readonly Vector3 _position;
            private readonly Quaternion _rotation;
            private readonly Vector3 _scale;

            public MeshRequest(Color color, Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, bool isWired) : base(color)
            {
                _mesh = mesh;
                _position = position;
                _rotation = rotation;
                _scale = scale;
                _isWired = isWired;
            }

            protected override void OnDraw()
            {
                if (_isWired)
                    Gizmos.DrawWireMesh(_mesh, _position, _rotation, _scale);
                else
                    Gizmos.DrawMesh(_mesh, _position, _rotation, _scale);
            }
        }
    }
}