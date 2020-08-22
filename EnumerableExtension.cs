using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> ReplaceAll<T>(this IEnumerable<T> source,
                                                   T oldValue,
                                                   T newValue,
                                                   IEqualityComparer<T> comparer = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            comparer = comparer ?? EqualityComparer<T>.Default;

            foreach (var item in source)
                yield return
                    comparer.Equals(item, oldValue) ? newValue : item;
        }

        //reference : https://stackoverflow.com/questions/3188693/how-can-i-get-linq-to-return-the-object-which-has-the-max-value-for-a-given-prop
        public static T MaxBy<T, TU>(this IEnumerable<T> data, Func<T, TU> f) where TU : IComparable
        {
            return data.Aggregate((i1, i2) => f(i1).CompareTo(f(i2)) > 0 ? i1 : i2);
        }
        
        //reference : https://stackoverflow.com/questions/3188693/how-can-i-get-linq-to-return-the-object-which-has-the-max-value-for-a-given-prop
        public static T MinBy<T, TU>(this IEnumerable<T> data, Func<T, TU> f) where TU : IComparable
        {
            return data.Aggregate((i1, i2) => f(i1).CompareTo(f(i2)) < 0 ? i1 : i2);
        }
    }
}