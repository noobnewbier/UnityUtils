using UnityEditor;
using UnityEngine;

namespace UnityUtils.Editor
{
    public class NonebHandleUtility
    {
        public static void DrawArrow(int controlID,
                                     Vector3 position,
                                     Quaternion rotation,
                                     float size,
                                     EventType eventType)
        {
            //reference: Handles.ArrowHandleCap, Handles.ConeHandleCap
            //contains various magic number that is used by Unity, no idea how they come up with them

            var pointingDirection = rotation * Vector3.forward;
            var lineThickness = Handles.lineThickness;
            var artificiallyShrunkSize = size * 0.2f;

            Handles.ConeHandleCap(
                controlID,
                position + pointingDirection * size,
                rotation,
                artificiallyShrunkSize,
                eventType
            );

            var lineSize = size * 0.9f;
            Handles.DrawLine(position, position + pointingDirection * lineSize, lineThickness);
        }
    }
}