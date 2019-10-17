using UnityEngine;

namespace UnityUtils
{
    public abstract class PooledMonoBehaviour : MonoBehaviour
    {
        private ObjectPool _pool;

        public GameObject GetPooledInstance()
        {
            // Couldn't do it on awake - what if when we need an instance when there is no instance out there? 
            if (_pool == null)
            {
                _pool = ObjectPool.GetPoolFor(this);
            }

            var toReturn = _pool.GetInstance();
            toReturn.GetComponent<PooledMonoBehaviour>()._pool = _pool; //what if the same gameobject have multiple pooledObject

            return toReturn;
        }

        public void ReturnToPool()
        {
            _pool.AddInstance(this);
        }
    }
}