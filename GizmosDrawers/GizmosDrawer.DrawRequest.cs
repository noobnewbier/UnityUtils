using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace UnityUtils
{
    public partial class GizmosDrawer
    {
        private abstract class DrawRequest
        {
            private readonly Color _color;

            protected DrawRequest(Color color, float duration)
            {
                _color = color;
                Duration = duration;
            }

            public float Duration { get; set; }
            public float Timer { get; set; }
            public bool IsExpired => Timer > Duration;

            public void Draw()
            {
                using (new NonebEditorGUI.GizmosColorScope(_color))
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

            public LineRequest(Color color, float duration, Vector3 from, Vector3 to) : base(color, duration)
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

            public WireSphereRequest(Color color, float duration, Vector3 center, float radius) : base(color, duration)
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

            public SphereRequest(Color color, float duration, Vector3 center, float radius) : base(color, duration)
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

            public WireCubeRequest(Color color, float duration, Vector3 center, Vector3 size) : base(color, duration)
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

            public CubeRequest(Color color, float duration, Vector3 center, Vector3 size) : base(color, duration)
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

            public MeshRequest(Color color, float duration, Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, bool isWired) : base(color, duration)
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

        private class WireDiscRequest : DrawRequest
        {
            private readonly Vector3 _position;
            private readonly float _rad;

            public WireDiscRequest(Color color, float duration, float rad, Vector3 position) : base(color, duration)
            {
                _rad = rad;
                _position = position;
            }

            protected override void OnDraw()
            {
                //lower value leads to smoother circle, but slower drawing
                const float thetaDelta = 0.05f;

                var theta = 0f;
                var x = _rad * Mathf.Cos(theta);
                var y = _rad * Mathf.Sin(theta);
                var pos = _position + new Vector3(x, 0, y);
                var lastPos = pos;
                for (theta = 0.1f; theta < Mathf.PI * 2; theta += thetaDelta)
                {
                    x = _rad * Mathf.Cos(theta);
                    y = _rad * Mathf.Sin(theta);
                    var newPos = _position + new Vector3(x, 0, y);
                    Gizmos.DrawLine(pos, newPos);
                    pos = newPos;
                }

                Gizmos.DrawLine(pos, lastPos);
            }
        }

        private class LabelRequest : DrawRequest
        {
            private readonly Vector3 _position;
            private readonly string _text;

            public LabelRequest(Color color, float duration, string text, Vector3 position) : base(color, duration)
            {
                _text = text;
                _position = position;
            }

            protected override void OnDraw()
            {
                Handles.Label(_position, _text);
            }
        }
    }
}