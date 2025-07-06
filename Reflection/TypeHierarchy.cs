using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    //todo: check interface
    /// <summary>
    /// Basically just a typical Node data structure, used to denote type hierarchy for convenience
    /// </summary>
    public record TypeHierarchy(Type Value, List<TypeHierarchy> Children)
    {
        public IEnumerable<Type> Flatten()
        {
            return Children.Select(c => c.Flatten())
                .SelectMany(t => t)
                .Append(Value);
        }
    }
}