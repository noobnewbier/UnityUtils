using UnityEngine;

namespace UnityUtils
{
    public class ToDestroyWhenDestroyed : MonoBehaviour
    {
        [SerializeField] private GameObject[] gameObjects;

        private void OnDestroy()
        {
            foreach (var go in gameObjects) Destroy(go);
        }
    }
}