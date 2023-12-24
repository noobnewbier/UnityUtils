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
    }
}