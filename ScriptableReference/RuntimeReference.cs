using System;
using UnityEngine;

namespace UnityUtils.ScriptableReference
{
    public class RuntimeReference<T> : ScriptableObject
    {
        [SerializeField] private T initialReference;
        [SerializeField] private T currentReference;
        [SerializeField] private bool canBeNullWhenAccessed;

        public T CurrentReference
        {
            get
            {
                if (currentReference == null && !canBeNullWhenAccessed)
                    throw new InvalidOperationException($"{name} current reference cannot be null when accessed, does it requires instantiation?");

                return currentReference;
            }
            set
            {
                if ((value == null) & !canBeNullWhenAccessed) throw new InvalidOperationException($"{name} current reference cannot be set to null");

                currentReference = value;
            }
        }

        private void OnEnable()
        {
            currentReference = initialReference;
        }
    }
}