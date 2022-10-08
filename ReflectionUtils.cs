using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityUtils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<MethodInfo> GetMethodsByAttribute(Type type, Type attributeType)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentNullException($"{attributeType.FullName} is not an attribute");

            return type.GetMethods().Where(m => m.GetCustomAttribute(attributeType, false) != null);
        }

        public static IEnumerable<FieldInfo> GetFieldsByAttribute(Type type,
            Type attributeType,
            bool searchInheritance = false)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentNullException($"{attributeType.FullName} is not an attribute");

            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fieldInfo => fieldInfo.GetCustomAttribute(attributeType, searchInheritance) != null);
        }

        public static IEnumerable<(Type type, T attachedAttribute)> GetTypesWithAttribute<T>(
            bool searchInheritance = false) where T : Attribute
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                let attribute = type.GetCustomAttribute<T>(searchInheritance)
                where attribute != null
                select (type, attribute);
        }

        public static IEnumerable<(Type type, IEnumerable<T> attachedAttributes)> GetTypesWithAttributes<T>(
            bool searchInheritance = false) where T : Attribute
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                let attributes = type.GetCustomAttributes<T>(searchInheritance)
                where attributes.Any()
                select (type, attributes);
        }

        public static MethodInfo GetMethodByAttribute(Type type, Type attributeType)
        {
            var methods = GetMethodsByAttribute(type, attributeType);

            var methodInfos = methods.ToList();
            if (methodInfos.Count > 1)
                throw new ArgumentException(
                    $"There is more than one method with type: {type.FullName} with the attribute: {attributeType.FullName}"
                );

            if (!methodInfos.Any())
                throw new ArgumentException(
                    $"There is no method with type: {type.FullName} with the attribute: {attributeType.FullName}"
                );

            return methodInfos.First();
        }

        public static T[] GetAttributes<T>(this ICustomAttributeProvider target, bool inherit)
            where T : Attribute
        {
            return (T[])target.GetCustomAttributes(typeof(T), inherit);
        }
        
        public static T? GetAttribute<T>(this ICustomAttributeProvider target, bool inherit)
            where T : Attribute
        {
            var attributes = target.GetAttributes<T>(inherit);

            if (attributes.Length > 1)
            {
                throw new InvalidOperationException($"More than one matching attributes {target}");
            }

            return attributes.FirstOrDefault();
        }
    }
}