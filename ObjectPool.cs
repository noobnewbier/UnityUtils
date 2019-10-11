﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityUtils
{
    public class ObjectPool : MonoBehaviour
    {
        private const int PoolSize = 50;
        private readonly Stack<PooledMonoBehaviour> _pool = new Stack<PooledMonoBehaviour>();

        private PooledMonoBehaviour _pooledMono;

        public PooledMonoBehaviour PooledGameObject
        {
            set => _pooledMono = value;
        }


        public T GetInstance<T>() where T : PooledMonoBehaviour
        {
            return GetInstance().GetComponent<T>();
        }

        public GameObject GetInstance()
        {
            GameObject toReturn;
            if (!_pool.Any())
            {
                toReturn = Instantiate(_pooledMono.gameObject);
            }
            else
            {
                toReturn = _pool.Pop().gameObject;
            }

            toReturn.SetActive(true);
            toReturn.transform.SetParent(transform);

            return toReturn;
        }

        public void AddInstance(PooledMonoBehaviour instance)
        {
            if (_pool.Count < PoolSize)
            {
                _pool.Push(instance);
                instance.gameObject.SetActive(false);
            }
            else
            {
                //just discard it - we have too much of it
                Destroy(instance.gameObject);
            }
        }

        public static ObjectPool GetPoolFor(PooledMonoBehaviour toPool)
        {
            if (Application.isEditor) //if there are existing pools
            {
                var obj = GameObject.Find(toPool.name + "Pool");
                if (obj)
                {
                    return obj.GetComponent<ObjectPool>();
                }
            }

            var newPool = new GameObject
            {
                name = toPool.name + "Pool"
            };
            var pool = newPool.AddComponent<ObjectPool>();
            pool.PooledGameObject = toPool;

            return pool;
        }
    }
}