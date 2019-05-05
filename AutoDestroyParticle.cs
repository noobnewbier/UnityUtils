using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(ParticleSystem))]
    public class AutoDestroyParticle : MonoBehaviour
    {

        ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!_particleSystem.IsAlive()) Destroy(gameObject);
        }
    }
}
