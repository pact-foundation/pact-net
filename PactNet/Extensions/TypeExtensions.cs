#if USE_TYPE_EXTENSIONS
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