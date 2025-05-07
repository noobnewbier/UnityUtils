using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        }

        public static IEnumerable<FieldInfo> GetFieldsByAttribute(
            Type type,
            Type attributeType,
            bool searchInheritance = false)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentNullException($"{attributeType.FullName} is not an attribute");

            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fieldInfo => fieldInfo.GetCustomAttribute(attributeType, searchInheritance) != null);
        }

        public static IEnumerable<(Type type, T attachedAttribute)> GetTypesWithAttribute<T>(
            bool searchInheritance = false) where T : Attribute =>
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            let attribute = type.GetCustomAttribute<T>(searchInheritance)
            where attribute != null
            select (type, attribute);

        public static IEnumerable<(Type type, IEnumerable<T> attachedAttributes)> GetTypesWithAttributes<T>(
            bool searchInheritance = false) where T : Attribute =>
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
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

        private static class IsSameOrSubClassCache
        {
            private static readonly Dictionary<Type, SubClassCache> Cache = new();

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
                private readonly HashSet<Type> _knownNonSubTypes = new();
                private readonly HashSet<Type> _knownSubTypes = new();

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