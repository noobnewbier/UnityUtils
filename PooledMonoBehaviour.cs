using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PooledMonoBehaviour : MonoBehaviour
{

    ObjectPool _pool;

    protected virtual void Awake()
    {
        if (_pool == null)
        {
            _pool = ObjectPool.GetPoolFor(this);
        }
    }

    public GameObject GetPooledInstance()
    {
        GameObject toReturn = _pool.GetInstance();
        toReturn.GetComponent<PooledMonoBehaviour>()._pool = _pool; //what if the same gameobject have multiple pooledObject

        return toReturn;
    }

    public void ReturnToPool()
    {
        _pool.AddInstance(this);
        gameObject.SetActive(false);
    }

}
