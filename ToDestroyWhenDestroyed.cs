using UnityEngine;

namespace UnityUtils
{
    public class ToDestroyWhenDestroyed : UnityEngine.MonoBehaviour
    {
        [SerializeField] private GameObject[] gameObjects;

        private void OnDestroy()
        {
            foreach (var go in gameObjects)
            {
                Destroy(go);
            }
        }
    }
}