using System;
using System.Linq;
using UnityEngine;

namespace UnityUtils.Pooling
{
    public abstract class PooledMonoBehaviour : MonoBehaviour, IPoolable<GameObject>
    {
        [Range(0, 10000)] [SerializeField] private int poolSize = 50;

        private ObjectPool _pool;

        public GameObject GetPooledInstance()
        {
            // Couldn't do it on awake - what if when we need an instance when there is no instance out there? 
            if (_pool == null)
            {
                _pool = ObjectPool.GetPoolFor(this, poolSize);
            }

            var toReturn = _pool.GetInstance();
            toReturn.GetComponent<PooledMonoBehaviour>()._pool = _pool;

            return toReturn;
        }

        public void ReturnToPool()
        {
            var allChildren = GetComponentsInChildren<PooledMonoBehaviour>();

            foreach (var child in allChildren.Where(c => c != this)) child.ReturnToPool();

            _pool.AddInstance(this);
        }

        private void OnValidate()
        {
            if (GetComponents<PooledMonoBehaviour>().Length > 1)
            {
                throw new InvalidOperationException($"{gameObject.name} has more than 1 {nameof(PooledMonoBehaviour)}");
            }
        }
    }
}