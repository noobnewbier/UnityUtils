using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace UnityUtils.Pooling
{
    public class BehaviourPool<T> : IObjectPool<T>, IDisposable where T : MonoBehaviour
    {
        private readonly Action<T>? _actionOnDestroy;
        private readonly Action<T>? _actionOnGet;
        private readonly Action<T>? _actionOnRelease;
        private readonly bool _collectionCheck;

        private readonly List<T> _list;
        private readonly int _maxSize;
        private readonly T _objectPrefab;
        private readonly GameObject _pooledObjectRoot;

        public BehaviourPool(
            T objectPrefab,
            Action<T>? actionOnGet = null,
            Action<T>? actionOnRelease = null,
            Action<T>? actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000)
        {
            if (maxSize <= 0)
                throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));

            _objectPrefab = objectPrefab;
            _list = new List<T>(defaultCapacity);
            _maxSize = maxSize;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _collectionCheck = collectionCheck;

            _pooledObjectRoot = new GameObject
            {
                name = $"{_objectPrefab.name}.Pool"
            };
            Object.DontDestroyOnLoad(_pooledObjectRoot);
        }

        public int CountAll { get; private set; }

        public int CountActive => CountAll - CountInactive;

        public void Dispose() => Clear();

        public int CountInactive => _list.Count;

        public T Get()
        {
            T obj;
            if (_list.Count == 0)
            {
                obj = Object.Instantiate(_objectPrefab);
                ++CountAll;
            }
            else
            {
                var index = _list.Count - 1;
                obj = _list[index];
                _list.RemoveAt(index);

                obj.gameObject.SetActive(true);
            }

            _actionOnGet?.Invoke(obj);
            return obj;
        }

        public PooledObject<T> Get(out T v) => new(v = Get(), this);

        public void Release(T element)
        {
            if (_collectionCheck && _list.Count > 0)
                if (_list.Contains(element))
                    throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");

            _actionOnRelease?.Invoke(element);
            if (CountInactive < _maxSize)
            {
                _list.Add(element);
                element.gameObject.SetActive(false);
                element.transform.SetParent(_pooledObjectRoot.transform);
            }
            else
            {
                --CountAll;
                _actionOnDestroy?.Invoke(element);
                Object.Destroy(element.gameObject);
            }
        }

        public void Clear()
        {
            if (_actionOnDestroy != null)
                foreach (var obj in _list)
                    _actionOnDestroy(obj);
            _list.Clear();
            CountAll = 0;
        }
    }
}