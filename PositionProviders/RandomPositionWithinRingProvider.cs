using UnityEngine;

namespace UnityUtils.PositionProviders
{
    public class RandomPositionWithinRingProvider : PositionProvider
    {
        [SerializeField] private float innerRadius;
        [SerializeField] private float outerRadius;

        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            GizmosHelpers.DrawWireDisc(innerRadius, position, Color.red);
            GizmosHelpers.DrawWireDisc(outerRadius, position, Color.red);
        }

        public override Vector3 ProvideLocation()
        {
            var point = RandomUtils.RandomPointWithinRing(innerRadius, outerRadius);

            var position = transform.position;
            return new Vector3(point.x + position.x, position.y, point.y + position.z);
        }
    }
}