using UnityEngine;

namespace UnityUtils
{
    [RequireComponent(typeof(ParticleSystem))]
    public class AutoDestroyParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

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