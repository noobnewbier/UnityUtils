using System;
using UnityEngine;

namespace UnityUtils
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Perform a deep copy of the object via serialization.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>A deep copy of the object.</returns>
        public static T DeepCopy<T>(this T source)
        {
            if (!typeof(T).IsSerializable) throw new ArgumentException("The type must be serializable.", nameof(source));

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default;

            var serialized = JsonUtility.ToJson(source);
            return JsonUtility.FromJson<T>(serialized);
        }
    }
}