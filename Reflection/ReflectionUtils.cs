using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [InitializeOnLoad]
    public static partial class ReflectionUtils
    {
        static ReflectionUtils()
        {
            ClearCache();
        }

        [MenuItem("NonebNi/Editor/Clear Reflection Cache")]
        public static void ClearCache()
        {
            IsSameOrSubClassCache.Clear();
            MethodByAttributeCache.Clear();
            MemberBindingFlagsCache.Clear();
            SameOrSubclassCache.Clear();
            TypeHierarchyCache.Clear();
            TypeCache.Clear();
        }

        /// <summary>
        /// A type is concrete if I can create an instance of it. The check is probably incomplete, use at your own risk
        /// https://stackoverflow.com/questions/31772922/difference-between-isgenerictype-and-isgenerictypedefinition
        /// </summary>
        public static bool IsConcrete(this Type type) => type is { IsAbstract: false, IsInterface: false, IsGenericTypeParameter: false, IsGenericTypeDefinition: false };

        public static (bool success, object? instance) TryCreateInstance(this Type type)
        {
            try
            {
                return (true, Activator.CreateInstance(type));
            }
            catch (MissingMethodException)
            {
                return (false, null);
            }
        }

        public static object CreateObjectWithLeastPossibleDependencies(this Type type)
        {
            if (type.IsValueType)
                //value type -> no constructor
                return Activator.CreateInstance(type);

            if (type == typeof(string)) return string.Empty;

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (type.GetConstructor(bindingFlags, null, Type.EmptyTypes, null) != null)
                //default param-less constructor - just use this
                return Activator.CreateInstance(type);

            var constructors = type.GetConstructors(bindingFlags);
            var leastArgConstructors = constructors.Aggregate((current, next) =>
                {
                    if (current == null) return next;

                    var currParams = current.GetParameters();
                    var nextParams = next.GetParameters();

                    // Recursive construction, we need to just fuck off.
                    if (currParams.Any(p => p.ParameterType == type)) return next;

                    if (nextParams.Any(p => p.ParameterType == type)) return current;

                    var currRequiredArgCount = currParams.Count(p => !p.HasDefaultValue);
                    var nextRequiredArgCount = nextParams.Count(p => !p.HasDefaultValue);

                    return currRequiredArgCount <= nextRequiredArgCount ?
                        current :
                        next;
                }
            );

            if (leastArgConstructors == null)
                /*
                 * Break glass in case of emergency - there isn't a constructor we can use,
                 * so we just bypass it completely and create an object without using a constructor.
                 */
                return FormatterServices.GetUninitializedObject(type);

            var args = leastArgConstructors
                .GetParameters()
                .Select(p =>
                    {
                        return p switch
                        {
                            _ when p.HasDefaultValue => p.DefaultValue ?? null,
                            _ when p.ParameterType.IsValueType => Activator.CreateInstance(p.ParameterType),
                            _ => null
                        };
                    }
                )
                .ToArray();

            return leastArgConstructors.Invoke(args);
        }

        public static TypeHierarchy GetTypeHierarchy(this Type type) => TypeHierarchyCache.GetOrFind(type);

        public static IEnumerable<Type> GetSubclasses(this Type type) => SameOrSubclassCache.Get(type);

        public static IEnumerable<(FieldInfo fieldInfo, Attribute attribute)> GetFieldsByAttribute(
            Type type,
            Type attributeType,
            bool searchInheritance = false,
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentNullException($"{attributeType.FullName} is not an attribute");

            return type.GetFields(bindingFlags)
                .Select(f => (fieldInfo: f, attribute: f.GetCustomAttribute(attributeType, searchInheritance)))
                .Where(t => t.attribute != null);
        }

        public static IEnumerable<(Type type, T attachedAttribute)> GetTypesWithAttribute<T>(
            bool searchInheritance = false) where T : Attribute =>
            from type in GetAllTypes()
            let attribute = type.GetCustomAttribute<T>(searchInheritance)
            where attribute != null
            select (type, attribute);

        public static IEnumerable<(Type type, IEnumerable<T> attachedAttributes)> GetTypesWithAttributes<T>(
            bool searchInheritance = false) where T : Attribute =>
            from type in GetAllTypes()
            let attributes = type.GetCustomAttributes<T>(searchInheritance)
            where attributes.Any()
            select (type, attributes);

        public static T[] GetAttributes<T>(this ICustomAttributeProvider target, bool inherit)
            where T : Attribute =>
            (T[])target.GetCustomAttributes(typeof(T), inherit);

        public static T? GetAttribute<T>(this ICustomAttributeProvider target, bool inherit)
            where T : Attribute
        {
            var attributes = target.GetAttributes<T>(inherit);

            if (attributes.Length > 1) throw new InvalidOperationException($"More than one matching attributes {target}");

            return attributes.FirstOrDefault();
        }

        /// <summary>
        ///     Getting the element type within an array or a list. This assumes the given type <paramref name="t" /> is an array
        /// or a list.
        /// </summary>
        public static Type? GetTypeWithinCollection(this Type t)
        {
            if (t.IsArray) return t.GetElementType();

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) return t.GetGenericArguments()[0];

            Debug.LogError($"Type({t}) is not a List<> or array - couldn't retrieve the type within it!");
            return null;
        }

        public static bool IsUnitySupportedCollection(this Type t)
        {
            if (t.IsArray) return true;

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) return true;

            return false;
        }

        /// <summary>
        ///     <see cref="Type.GetField(string)" /> but also looking from its base type recursively as well.
        /// <see cref="BindingFlags" /> includes non public field by default, as that's usually what I expect when working with
        /// type hierarchy.
        /// </summary>
        public static FieldInfo? GetFieldIncludingParents(
            this Type type,
            string fieldName,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var fi = type.GetField(fieldName, bindingFlags);
            if (fi != null) return fi;

            var baseType = type.BaseType;
            return baseType?.GetField(fieldName, bindingFlags);
        }

        public static IEnumerable<FieldInfo> GetFieldsOfType<T>(
            this Type type,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var fields = type.GetFields(bindingFlags);
            foreach (var fi in fields)
                if (fi.FieldType == typeof(T))
                    yield return fi;
        }

        public static bool IsSameOrSubclass(this Type potentialDescendant, Type? potentialBase) => IsSameOrSubClassCache.Check(potentialDescendant, potentialBase);

        /// <summary>
        ///     Checking if a type is a subclass of a generic type.
        /// </summary>
        public static bool IsSubclassOfGeneric(this Type? t, Type genericType)
        {
            if (t == genericType)
                //Only check for subclass!
                return false;

            if (genericType is { IsGenericTypeDefinition: false, IsGenericType: false }) return false;

            //Ref: https://stackoverflow.com/a/457708
            while (t != null && t != typeof(object))
            {
                var cur = t.IsGenericType ?
                    t.GetGenericTypeDefinition() :
                    t;
                if (genericType == cur) return true;
                t = t.BaseType;
            }

            return false;
        }

        private static IEnumerable<Type> GetAllTypes() => TypeCache.GetOrFind();

        private static class TypeCache
        {
            private static Type[]? _cache;

            public static Type[] GetOrFind()
            {
                return _cache ??= AppDomain.CurrentDomain.GetAssemblies()
                    /*
                     * todo:
                     * I think I will regret at some point, but this might be good enough now
                     * in a nutshell we are trying to cut down the number of types we are chowing through for performance
                     * It might be better if we can have a way to walk "upwards" in the type hierarchy when doing the type analysis...?
                     */
                    .Where(a => a.FullName.Contains("Unity") || a.FullName.Contains("Noneb"))
                    .SelectMany(assembly => assembly.GetTypes())
                    .ToArray();
            }

            public static void Clear()
            {
                _cache = null;
            }
        }

        private static class SameOrSubclassCache
        {
            private static readonly Dictionary<Type, Type[]> Cache = new ();

            public static IEnumerable<Type> Get(Type baseType)
            {
                if (!Cache.TryGetValue(baseType, out var cache))
                {
                    var enumerable = GetAllTypes().Where(type => IsSameOrSubclass(type, baseType));
                    Cache[baseType] = cache =
                        enumerable
                            .ToArray();
                }

                return cache;
            }

            public static void Clear()
            {
                Cache.Clear();
            }
        }

        private static class TypeHierarchyCache
        {
            private static readonly Dictionary<Type, TypeHierarchy> Cache = new ();

            public static TypeHierarchy GetOrFind(Type type)
            {
                if (!Cache.TryGetValue(type, out var cache)) Cache[type] = cache = CreateHierarchy(type);

                return cache;
            }

            private static TypeHierarchy CreateHierarchy(Type type)
            {
                var subclasses = type.GetSubclasses().Where(t => t != type);
                var children = new List<TypeHierarchy>();
                foreach (var subclass in subclasses)
                {
                    var child = CreateHierarchy(subclass);
                    children.Add(child);
                }

                return new TypeHierarchy(type, children);
            }

            public static void Clear()
            {
                Cache.Clear();
            }
        }


        private static class IsSameOrSubClassCache
        {
            private static readonly Dictionary<Type, SubClassCache> Cache = new ();

            public static bool Check(Type potentialDescendant, Type? potentialBase)
            {
                if (potentialBase == null) return false;

                if (!Cache.TryGetValue(potentialBase, out var subClassCache)) Cache[potentialBase] = subClassCache = new SubClassCache();

                if (!subClassCache.TryGet(potentialDescendant, out var result))
                {
                    result = potentialDescendant == potentialBase
                             || potentialDescendant.IsSubclassOf(potentialBase)
                             || potentialDescendant.IsSubclassOfGeneric(potentialBase);

                    subClassCache.SetCache(potentialDescendant, result);
                }

                return result;
            }

            public static void Clear()
            {
                Cache.Clear();
            }

            //TODO: this seems to slow things down -> we might need to give objects a bigger capacity initially so it doesn't keep doubling
            private class SubClassCache
            {
                private readonly HashSet<Type> _knownNonSubTypes = new ();
                private readonly HashSet<Type> _knownSubTypes = new ();

                public void SetCache(Type? type, bool isSubType)
                {
                    if (type == null) return;

                    if (isSubType)
                        _knownSubTypes.Add(type);
                    else
                        _knownNonSubTypes.Add(type);
                }

                public bool TryGet(Type? potentialDescendant, out bool result)
                {
                    if (potentialDescendant == null)
                    {
                        result = false;
                        return true;
                    }

                    if (_knownSubTypes.Contains(potentialDescendant))
                    {
                        result = true;
                        return true;
                    }

                    if (_knownNonSubTypes.Contains(potentialDescendant))
                    {
                        result = false;
                        return true;
                    }

                    result = false;
                    return false;
                }
            }
        }
    }
}