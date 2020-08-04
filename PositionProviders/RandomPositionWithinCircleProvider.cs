using UnityEngine;

namespace UnityUtils.PositionProviders
{
    public class RandomPositionWithinCircleProvider : PositionProvider
    {
        [SerializeField] private float radius;

        public override Vector3 ProvideLocation()
        {
            var point = Random.insideUnitCircle * radius;
            var position = transform.position;
            return new Vector3(point.x + position.x, position.y, point.y + position.z);
        }

        private void OnDrawGizmosSelected()
        {
            GizmosHelpers.DrawWireDisc(radius, transform.position, Color.red);
        }
    }
}