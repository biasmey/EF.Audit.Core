using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Reflection;

namespace EF.Audit.Core.Extensions
{
    internal static class ReflectionExtension
    {
        public static bool IsAttr<T>(this PropertyInfo entry) where T : Attribute
        {
            return entry.CustomAttributes.Any(q => q.AttributeType == typeof(T));
        }

       
    }
}
