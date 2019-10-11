﻿using UnityEngine;

namespace UnityUtils
{
    public abstract class PooledMonoBehaviour : MonoBehaviour
    {
        private ObjectPool _pool;

        protected virtual void Awake()
        {
            if (_pool == null)
            {
                _pool = ObjectPool.GetPoolFor(this);
            }
        }

        public GameObject GetPooledInstance()
        {
            var toReturn = _pool.GetInstance();
            toReturn.GetComponent<PooledMonoBehaviour>()._pool = _pool; //what if the same gameobject have multiple pooledObject

            return toReturn;
        }

        public void ReturnToPool()
        {
            _pool.AddInstance(this);
            gameObject.SetActive(false);
        }
    }
}