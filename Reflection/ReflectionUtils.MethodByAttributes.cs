using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityUtils
{
    public static partial class ReflectionUtils
    {
        public static IEnumerable<(MethodInfo method, TAttribute attribute)> GetMethodsByAttribute<TAttribute>(Type classType, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
        {
            foreach (var (method, attribute) in GetMethodsByAttribute(classType, typeof(TAttribute), bindingFlags)) yield return (method, (TAttribute)attribute);
        }

        public static IEnumerable<(MethodInfo method, Attribute attribute)> GetMethodsByAttribute(Type classType, Type attributeType, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentNullException($"{attributeType.FullName} is not an attribute");

            return MethodByAttributeCache.GetOrFind(classType, attributeType, bindingFlags);
        }

        public static MethodInfo GetMethodByAttribute(Type classType, Type attributeType)
        {
            var methods = GetMethodsByAttribute(classType, attributeType);

            var methodInfos = methods.ToList();
            if (methodInfos.Count > 1)
                throw new ArgumentException(
                    $"There is more than one method with type: {classType.FullName} with the attribute: {attributeType.FullName}"
                );

            if (!methodInfos.Any())
                throw new ArgumentException(
                    $"There is no method with type: {classType.FullName} with the attribute: {attributeType.FullName}"
                );

            return methodInfos.First().method;
        }

        private static class MethodByAttributeCache
        {
            private static readonly Dictionary<Type, ClassCache> Cache = new();

            public static IEnumerable<(MethodInfo method, Attribute attribute)> GetOrFind(Type classType, Type attributeType, BindingFlags bindingFlags)
            {
                if (!Cache.TryGetValue(classType, out var cache)) Cache[classType] = cache = new ClassCache(classType);

                return cache.GetOrFind(attributeType, bindingFlags);
            }

            public static void Clear()
            {
                Cache.Clear();
            }

            private class ClassCache
            {
                private readonly Dictionary<Type?, (MethodInfo method, Attribute attribute)[]> _attributesAndMethod = new();
                private readonly Type _classType;

                private readonly Dictionary<BindingFlags, MethodInfo[]> _methodsByBindingFlags = new();

                public ClassCache(Type classType)
                {
                    _classType = classType;
                }

                public IEnumerable<(MethodInfo method, Attribute attribute)> GetOrFind(Type attributeType, BindingFlags bindingFlags)
                {
                    if (!_attributesAndMethod.TryGetValue(attributeType, out var methods))
                    {
                        var allMethods = GetOrFindWithFlags(bindingFlags);
                        _attributesAndMethod[attributeType] = methods = allMethods.Select(m => (method: m, attribute: m.GetCustomAttribute(attributeType))).Where(t => t.attribute != null).ToArray();
                    }

                    return methods;
                }

                private IEnumerable<MethodInfo> GetOrFindWithFlags(BindingFlags bindingFlags)
                {
                    if (!_methodsByBindingFlags.TryGetValue(bindingFlags, out var methods)) _methodsByBindingFlags[bindingFlags] = methods = _classType.GetMethods(bindingFlags).ToArray();

                    return methods;
                }
            }
        }
    }
}