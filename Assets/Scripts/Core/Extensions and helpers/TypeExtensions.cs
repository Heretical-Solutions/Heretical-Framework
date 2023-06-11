using System;
using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions
{
    public static class TypeExtensions
    {
        public static bool IsTypeEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsTypeGenericEnumerable(this Type type)
        {
            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return true;
            }
            
            return false;
        }

        public static Type GetGenericEnumerableUnderlyingType(this Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static bool IsTypeGenericArray(this Type type)
        {
            return type.IsArray;
        }

        public static Type GetGenericArrayUnderlyingType(this Type type)
        {
            return type.GetElementType();
        }
    }
}