using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    const int POOL_SIZE = 50;

    public PooledMonoBehaviour PooledGameObject { set { _pooledMono = value; } }

    PooledMonoBehaviour _pooledMono;
    Stack<PooledMonoBehaviour> _pool = new Stack<PooledMonoBehaviour>();


    public T GetInstance<T>() where T : PooledMonoBehaviour
    { 
        return GetInstance().GetComponent<T>();
    }

    public GameObject GetInstance()
    {
        GameObject toReturn;
        if (!_pool.Any())
            toReturn = Instantiate(_pooledMono.gameObject);
        else
            toReturn = _pool.Pop().gameObject;

        toReturn.SetActive(true);
        toReturn.transform.SetParent(transform);

        return toReturn;
    }

    public void AddInstance(PooledMonoBehaviour instance)
    {
        if (_pool.Count < POOL_SIZE)
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
        if (Application.isEditor)//if there are existing pools
        {
            GameObject obj = GameObject.Find(toPool.name + "Pool");
            if (obj)
            {
                return obj.GetComponent<ObjectPool>();
            }
        }

        GameObject newPool = new GameObject
        {
            name = toPool.name + "Pool"
        };
        ObjectPool pool = newPool.AddComponent<ObjectPool>();
        pool.PooledGameObject = toPool;

        return pool;
    }

}
