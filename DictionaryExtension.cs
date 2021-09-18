using System;
using System.Collections.Generic;

namespace UnityUtils
{
    public static class DictionaryExtension
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                             TKey key,
                                                             TValue defaultValue) =>
            dictionary.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                             TKey key,
                                                             Func<TValue> defaultValueProvider) =>
            dictionary.TryGetValue(key, out var value) ? value : defaultValueProvider();
    }
}