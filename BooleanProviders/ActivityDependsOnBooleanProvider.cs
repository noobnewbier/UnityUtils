using UnityEngine;

namespace UnityUtils.BooleanProviders
{
    public class ActivityDependsOnBooleanProvider : MonoBehaviour
    {
        [SerializeField] private GameObject[] gameObjects;
        [SerializeField] private BooleanProvider provider;

        private void Update()
        {
            foreach (var go in gameObjects) go.SetActive(provider.ProvideBoolean());
        }
    }
}