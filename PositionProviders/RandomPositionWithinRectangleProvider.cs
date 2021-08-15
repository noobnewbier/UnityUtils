using UnityEngine;

namespace UnityUtils.PositionProviders
{
    public class RandomPositionWithinRectangleProvider : PositionProvider
    {
        [SerializeField] private float halfLength;
        [SerializeField] private float halfWidth;

        private void OnDrawGizmosSelected()
        {
            var color = Gizmos.color;
            Gizmos.color = Color.red;

            var localPosition = transform.localPosition;
            var leftX = localPosition.x - halfWidth;
            var rightX = localPosition.x + halfWidth;
            var botZ = localPosition.z - halfLength;
            var topZ = localPosition.z + halfLength;

            var topLeft = transform.TransformPoint(new Vector3(leftX, localPosition.y, topZ));
            var botLeft = transform.TransformPoint(new Vector3(leftX, localPosition.y, botZ));
            var topRight = transform.TransformPoint(new Vector3(rightX, localPosition.y, topZ));
            var botRight = transform.TransformPoint(new Vector3(rightX, localPosition.y, botZ));

            Gizmos.DrawLine(topLeft, botLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(botLeft, botRight);
            Gizmos.DrawLine(botRight, topRight);

            Gizmos.color = color;
        }

        public override Vector3 ProvideLocation()
        {
            var localPosition = transform.localPosition;
            var x = localPosition.x + Random.Range(-halfWidth, halfWidth);
            var z = localPosition.z + Random.Range(-halfLength, halfLength);
            var targetPosition = new Vector3(x, localPosition.y, z);
            targetPosition = transform.TransformPoint(targetPosition);
            return targetPosition;
        }
    }
}