#if NETSTANDARD1_6
using System;
using System.Reflection;

namespace PactNet.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c);
        }
    }
}
#endif