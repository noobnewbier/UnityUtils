using System;
using System.Collections.Generic;

namespace UnityUtils
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum
        {
            foreach (Enum value in Enum.GetValues(typeof(T)))
                if (input.HasFlag(value))
                    yield return (T)value;
        }

        public static int GetEnumIndex<T>(this T? enumValue) where T : Enum
        {
            var allValues = Enum.GetValues(typeof(T));
            var index = Array.IndexOf(allValues, enumValue);

            return index;
        }
    }
}