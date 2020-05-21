using UnityEngine;

namespace UnityUtils
{
    public class CubeGizmosDrawer : MonoBehaviour
    {
        [SerializeField] private Color color = Color.cyan;
        [SerializeField] private Vector3 size;

        private void OnEnable()
        {
#if !UNITY_EDITOR
            Destroy(this);
#endif
        }

        private void OnDrawGizmos()
        {
            var originalColor = Gizmos.color;

            Gizmos.color = color;
            Gizmos.DrawCube(transform.position, size);

            Gizmos.color = originalColor;
        }
    }
}