using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace EF.Audit.Core.Extensions
{
    internal static class EntityEntryExtension
    {
        public static string Serialize(this EntityEntry entry)
        {
            return JsonConvert.SerializeObject(entry.Entity, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    ContractResolver = new EntityContractResolver(entry),
                                    Formatting = Formatting.Indented,
                                    NullValueHandling = NullValueHandling.Ignore,
                                });

        }

        public static bool IsAttr<T>(this EntityEntry entry) where T : Attribute
        {
            return entry.Entity.GetType().CustomAttributes.Any(q => q.AttributeType == typeof(T));
        }
    }
}
