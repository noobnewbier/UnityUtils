using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils.Pooling;
using Object = UnityEngine.Object;

namespace UnityUtils
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private PooledMonoBehaviour pooledMono;
        [Range(0, 10000)] [SerializeField] private int poolSize;
        private readonly Stack<PooledMonoBehaviour> _pool = new Stack<PooledMonoBehaviour>();

        public T GetInstance<T>() where T : PooledMonoBehaviour => GetInstance().GetComponent<T>();

        public GameObject GetInstance()
        {
            GameObject toReturn;
            if (!_pool.Any())
            {
                toReturn = Instantiate(pooledMono.gameObject);
            }
            else
            {
                toReturn = _pool.Pop().gameObject;
                var toReturnTransform = toReturn.transform;

                toReturnTransform.parent = null;
                var pooledMonoTransform = pooledMono.transform;
                toReturnTransform.rotation = pooledMonoTransform.rotation;
                toReturnTransform.localScale = pooledMonoTransform.localScale;
                toReturnTransform.position = pooledMonoTransform.position;
            }

            toReturn.SetActive(true);

            return toReturn;
        }

        public void AddInstance(PooledMonoBehaviour instance)
        {
            if (_pool.Count < poolSize)
            {
                _pool.Push(instance);
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(transform, false);
            }
            else
            {
                //just discard it - we have too much of it
                Destroy(instance.gameObject);
            }
        }

        public static ObjectPool GetPoolFor(PooledMonoBehaviour toPool, int poolSize)
        {
            var existingPool = GameObject.Find(toPool.name + "Pool");
            if (existingPool)
            {
                var currentPool = existingPool.GetComponent<ObjectPool>();

                if (currentPool.pooledMono == toPool) return currentPool;
                throw new InvalidOperationException(
                    $"Two pool should not have a same name, something has gone wrong for {toPool.name}'s pooling"
                );
            }

            var pool = new GameObject().AddComponent<ObjectPool>();
            pool.InitPool(toPool, poolSize);

            return pool;
        }

        private void InitPool(PooledMonoBehaviour pooledMonoBehaviour, int newPoolSize)
        {
            name = GetPoolName(pooledMonoBehaviour);
            poolSize = newPoolSize;
            pooledMono = pooledMonoBehaviour;
        }

        private static string GetPoolName(Object pooledObject) => pooledObject.name + "Pool";
    }
}