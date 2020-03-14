using System;
using System.Collections.Generic;

namespace UnityUtils
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> source,
                                                   T oldValue,
                                                   T newValue,
                                                   IEqualityComparer<T> comparer = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            comparer = comparer ?? EqualityComparer<T>.Default;

            foreach (var item in source)
                yield return
                    comparer.Equals(item, oldValue)
                        ? newValue
                        : item;
        }
    }
}