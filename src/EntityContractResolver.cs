using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EF.Audit.Core
{
    internal class EntityContractResolver : DefaultContractResolver
    {
        private readonly EntityEntry _entry;

        public EntityContractResolver(EntityEntry entry)
        {
            _entry = entry;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);

            // Get the navigations
            var navigations = _entry.Metadata.GetNavigations().Select(n => n.Name);

            // Exclude the navigation properties
            return list.Where(p => !navigations.Contains(p.PropertyName)).ToArray();
        }
    }
}