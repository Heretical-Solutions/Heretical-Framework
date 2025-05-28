using System;
using System.Collections.Generic;
using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeHelpers
    {
        public static void GetTypesWithAttributeInAllAssemblies<TAttribute>(
            out Type[] result)
            where TAttribute : System.Attribute
        {
            List<Type> resultList = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                //ReflectionTypeLoadException: Exception of type 'System.Reflection.ReflectionTypeLoadException' was thrown.
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttribute<TAttribute>(false) != null)
                        {
                            resultList.Add(type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    foreach (Exception ex in e.LoaderExceptions)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    continue;
                }
            }

            result = resultList.ToArray();
        }
        
        public static void GetTypesWithAttributeInAllAssemblies<TAttribute>(
            out Type[] result,

            RepositoryFactory repositoryFactory,
            
            out IReadOnlyRepository<int, Type> hashToType,
            out IReadOnlyRepository<Type, int> typeToHash)
            where TAttribute : System.Attribute
        {
            hashToType = repositoryFactory.BuildDictionaryRepository<int, Type>();

            typeToHash = repositoryFactory.BuildDictionaryRepository<Type, int>();

            List<Type> resultList = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<TAttribute>(false) != null)
                    {
                        resultList.Add(type);
                    }
                }
            }

            foreach (Type type in resultList)
            {
                string typeFullString = type.ToString();

                int typeHash = typeFullString.GetHashCode();
                
                ((IRepository<int, Type>)hashToType).AddOrUpdate(typeHash, type);
                
                ((IRepository<Type, int>)typeToHash).AddOrUpdate(type, typeHash);
            }

            result = resultList.ToArray();
        }

        public static bool IsSameOrInheritor(
            this Type currentType,
            Type targetType)
        {
            return
                (currentType == targetType)
                || targetType.IsAssignableFrom(currentType)
                || (currentType.IsGenericType
                    && currentType.GetGenericTypeDefinition() == targetType); //ISomething<,> without specifying the generic types
        }
    }
}