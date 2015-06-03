using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Distributr.Core.Utility
{
    public class IgnoreInCsvAttribute : Attribute
    {
    }
    public static class ExtensionsOfPropertyInfo
    {
        public static IEnumerable<T> GetAttributes<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetCustomAttributes(typeof(T), true).Cast<T>();
        }

        public static bool IsMarkedWith<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetAttributes<T>().Any();
        }
    }
}