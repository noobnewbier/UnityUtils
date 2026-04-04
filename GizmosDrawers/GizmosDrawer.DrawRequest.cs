using System;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace UnityUtils
{
    public partial class GizmosDrawer
    {
        internal abstract class DrawRequest
        {
            /// <summary>
            /// There's an issue where when we are trying to draw gizmos with label, there's no fucking way for me to control the draw order such that label comes first.
            /// Or to be precise, nothing seems to affect the draw order between gizmos and handles, handles seems to always comes last if they are triggered in on draw gizmos.
            /// There's a Handle.DrawGizmos which might do the job but the way the function is named makes me feel like it triggers a redraw which is perf issue when we have enough gizmos.
            ///
            /// So as a workaround, we just tone down the alpha so shit is still readable even if they are partially covered by the gizmos. 
            /// Works nice enough so I don't waste any more time on this.
            /// </summary>
            private const float AlphaMultiplier = 0.5f;
            
            public readonly WeakReference? DurationKey;
            protected readonly Color Color;

            protected DrawRequest(Color color, float duration, WeakReference? key = null, string requestCategory = DefaultCategory)
            {
                /*
                 * Note:
                 * Yes, it's more elegant if we do a factory pattern like a good old OOP sanitarians.
                 * But we are not, and I am lazy when there's not style cop looking over me,
                 * so here we go.
                 */
                // Automatically assigning a key if not otherwise specified.
                if (key == null)
                {
                    KeyStacks.TryPeek(out key);
                    key ??= null;
                }

                if (requestCategory == DefaultCategory)
                {
                    CategoryStacks.TryPeek(out requestCategory);
                    requestCategory ??= DefaultCategory;
                }

                Color = color;
                Color.a *= AlphaMultiplier;
                
                Duration = duration;
                RequestCategory = requestCategory;
                DurationKey = key;
            }

            public float Duration { get; }
            public string RequestCategory { get; }
            public float Timer { get; set; }

            public bool IsExpired
            {
                get
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (DurationKey != null)
                    {
                        if (DurationKey is not { IsAlive: true })
                        {
                            return true;
                        }

                        if (DurationKey.Target is UnityEngine.Object uObj)
                        {
                            // in case of unity's object, it's also expired if it's destroyed.
                            return uObj == null;
                        }

                        // special case, duration is -1 means we are relying on key for their life time.
                        return false;
                    }

                    return Timer > Duration;
                }
            }

            public void Draw()
            {
                using (new NonebEditorGUI.GizmosColorScope(Color))
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

            public LineRequest(Color color, float duration, Vector3 from, Vector3 to, WeakReference? key = null) : base(color, duration, key)
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

            public WireSphereRequest(Color color, float duration, Vector3 center, float radius, WeakReference? key = null) : base(color, duration, key)
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

            public SphereRequest(Color color, float duration, Vector3 center, float radius, WeakReference? key = null) : base(color, duration, key)
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

            public WireCubeRequest(Color color, float duration, Vector3 center, Vector3 size, WeakReference? key = null) : base(color, duration, key)
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

            public CubeRequest(Color color, float duration, Vector3 center, Vector3 size, WeakReference? key = null) : base(color, duration, key)
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

            public MeshRequest(Color color, float duration, Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, bool isWired, WeakReference? key = null) : base(color, duration, key)
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

            public WireDiscRequest(Color color, float duration, float rad, Vector3 position, WeakReference? key = null) : base(color, duration, key)
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
            private readonly GUIContent _labelContent;
            private readonly Vector3 _position;

            public LabelRequest(Color color, float duration, string text, Vector3 position, WeakReference? key = null) : base(color, duration, key)
            {
                _labelContent = new (text);
                _position = position;
            }

            protected override void OnDraw()
            {
                Handles.Label(_position, _labelContent);
            }
        }

        /// <summary>
        /// Draw label with dynamically, so they become smaller and eventually fade away when you can't see them as your camera zooms out.
        /// </summary>
        private class DynamicLabelRequest : DrawRequest
        {
            private static readonly GUIStyle LabelStyle = new ()
            {
                alignment = TextAnchor.MiddleCenter
            };
            private readonly GUIContent _labelContent;
            private readonly float _maxOffsetFromCenter;
            private readonly Vector3 _position;

            public DynamicLabelRequest(Color color, float duration, string text, Vector3 position, float maxOffsetFromCenter = 1, WeakReference? key = null) : base(color, duration, key)
            {
                _position = position;
                _maxOffsetFromCenter = maxOffsetFromCenter;
                _labelContent = new (text);
            }

            protected override void OnDraw()
            {
                DrawCenteredLabel();
            }

            /// <summary>
            /// <see cref="Handles.Label(UnityEngine.Vector3,string)" /> can't center the label properly.
            /// Root of the problem is that <see cref="HandleUtility.WorldPointToSizedRect" /> isn't taking rects maxX/Y into account,
            /// This leads to a problem where the rect is essentially stretched to the left a bit, and as a result when trying to center the item,
            /// the text will go a bit "righter" than it should be.
            /// Until Unity(current ver: 2020.2.7f1) fix this, this should do the trick
            /// </summary>
            private void DrawCenteredLabel()
            {
                //behind the camera
                if (HandleUtility.WorldToGUIPointWithDepth(_position).z < 0.0)
                    return;

                var guiPoint = HandleUtility.WorldToGUIPoint(_position);

                //stop drawing the label if the text will be so small that it's invisible
                //define font size in a way it is more less around the same portion of the bounding rect
                const float fontSizeToBoundingRectRatio = 0.125f;
                var viewCameraTransform = SceneView.currentDrawingSceneView.camera.transform;
                var cameraForward = viewCameraTransform.forward;
                var viewForward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
                //the magnitude is 0 when the camera is "upright", e.g when you click on the y-axis on the scene view
                var viewRotation = viewForward.magnitude != 0f ?
                    Quaternion.LookRotation(viewForward, Vector3.up) :
                    Quaternion.identity;

                var v1 = HandleUtility.WorldToGUIPoint(_position + viewRotation * Vector3.left * _maxOffsetFromCenter);
                var v2 = HandleUtility.WorldToGUIPoint(_position + viewRotation * Vector3.right * _maxOffsetFromCenter);
                var v3 = HandleUtility.WorldToGUIPoint(_position + viewRotation * Vector3.forward * _maxOffsetFromCenter);
                var v4 = HandleUtility.WorldToGUIPoint(_position + viewRotation * Vector3.back * _maxOffsetFromCenter);
                //Which vertices is which corner of the rect depends on the orientation of the camera
                var minX = Mathf.Min
                (
                    v1.x,
                    v2.x,
                    v3.x,
                    v4.x
                );
                var minY = Mathf.Min
                (
                    v1.y,
                    v2.y,
                    v3.y,
                    v4.y
                );
                var maxX = Mathf.Max
                (
                    v1.x,
                    v2.x,
                    v3.x,
                    v4.x
                );
                var maxY = Mathf.Max
                (
                    v1.y,
                    v2.y,
                    v3.y,
                    v4.y
                );
                var boundingRect = new Rect
                (
                    minX,
                    minY,
                    maxX - minX,
                    maxY - minY
                );


                const int maxFontSize = 14;
                var fontSizeInFloat = boundingRect.height * fontSizeToBoundingRectRatio;
                LabelStyle.fontSize = Mathf.Min(Mathf.RoundToInt(fontSizeInFloat), maxFontSize);
                //These are just a magic number that feels right to me
                const int minReadableFontSize = 6;
                const float minVisibleAlpha = 0.5f;
                //decreasing alpha when the user is further away from the text while avoiding drawing text that are practically not readable
                LabelStyle.normal.textColor = new
                (
                    Color.r,
                    Color.g,
                    Color.b,
                    fontSizeInFloat / minReadableFontSize
                );

                if (LabelStyle.normal.textColor.a > minVisibleAlpha)
                {
                    var size = LabelStyle.CalcSize(_labelContent);
                    var rect = new Rect(guiPoint, size);
                    rect.xMin -= size.x / 2;
                    rect.xMax -= size.x / 2;
                    rect.yMin -= size.y / 2;
                    rect.yMax -= size.y / 2;

                    Handles.BeginGUI();

                    GUI.Label(LabelStyle.padding.Add(rect), _labelContent, LabelStyle);

                    Handles.EndGUI();
                }
            }
        }
    }
}