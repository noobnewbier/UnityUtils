using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnityUtils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class GizmosHelpers
    {
        public static void DrawWireDisc(float rad, Vector3 position, Color color)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = color;

            DrawWireDisc(rad, position);
            
            Gizmos.color = oldColor;
        }
        
        public static void DrawWireDisc(float rad, Vector3 position)
        {
            //lower value leads to smoother circle, but slower drawing
            const float thetaDelta = 0.05f;
            
            var theta = 0f;
            var x = rad * Mathf.Cos(theta);
            var y = rad * Mathf.Sin(theta);
            var pos = position + new Vector3(x, 0, y);
            var lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += thetaDelta)
            {
                x = rad * Mathf.Cos(theta);
                y = rad * Mathf.Sin(theta);
                var newPos = position + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }

            Gizmos.DrawLine(pos, lastPos);
        }
    }
}