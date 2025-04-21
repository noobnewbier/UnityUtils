using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    public static class CollectionExtensions
    {
        public static void Fill<T>(this T[] originalArray, T with)
        {
            for (var i = 0; i < originalArray.Length; i++) originalArray[i] = with;
        }

        public static IEnumerable<T[]> FindAllCombinations<T>(this IReadOnlyCollection<IReadOnlyCollection<T>> collections)
        {
            // simplest case -> there's nothing we can do
            if (!collections.Any()) yield break;

            // only one row -> just return the element
            if (collections.Count == 1)
            {
                var onlyRow = collections.First();
                foreach (var element in onlyRow) yield return new[] { element };

                yield break;
            }

            // base case -> combination of two rows
            if (collections.Count == 2)
            {
                var firstRow = collections.First();
                var secondRow = collections.Last();

                foreach (var i in firstRow)
                foreach (var j in secondRow)
                    yield return new[] { i, j };

                yield break;
            }

            // recursion -> first row element prepend to the combination of the rest
            var prefix = collections.First();
            var combinationOfRest = collections.Skip(1).ToArray().FindAllCombinations().ToArray();
            foreach (var i in prefix)
            foreach (var combination in combinationOfRest)
                yield return combination.Prepend(i).ToArray();
        }
    }
}