using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EF.Audit.Core.Extensions
{
    public static class DbContextExtension
    {


        /// <summary>
        /// Saves DbContext changes taking into account Audit
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        /// Indicates whether Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges
        //  is called after the changes have been sent successfully to the database.
        /// </param>
        /// <param name="currentUser">Current logged user</param>
        /// <returns></returns>
        public static int SaveChangesAndAudit(this DbContext context, bool acceptAllChangesOnSuccess, string currentUser = null)
        {
            return SaveChangesAndAuditAsync(context,acceptAllChangesOnSuccess, currentUser).Result;
        }

        /// <summary>
        /// Saves DbContext changes taking into account Audit
        /// </summary>
        /// </param>
        /// <param name="currentUser">Current logged user</param>
        /// <returns></returns>
        public static int SaveChangesAndAudit(this DbContext context, string currentUser = null)
        {
            return SaveChangesAndAuditAsync(context, true, currentUser).Result;
        }

        /// <summary>
        /// Saves DbContext async changes taking into account Audit
        /// </summary>
        /// <param name="currentUser">Current logged user</param>
        /// <returns></returns>
        public static async Task<int> SaveChangesAndAuditAsync(this DbContext context, string currentUser = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAndAuditAsync(context, true, currentUser, cancellationToken);
        }

        /// <summary>
        /// Saves DbContext async changes taking into account Audit
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        /// Indicates whether Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges
        ///  is called after the changes have been sent successfully to the database.
        /// </param>
        /// <param name="currentUser">Current logged user</param>
        /// <returns></returns>
        public static async Task<int> SaveChangesAndAuditAsync(this DbContext context, bool acceptAllChangesOnSuccess, string currentUser = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<AuditLog> audiEntities = new List<AuditLog>();
            int result = 0;
            var transsactionId = Guid.NewGuid();

            var auditEntries = context.GetAuditEntries();

            audiEntities.AddRange(context.FindAuditLogs(auditEntries.Where(q => q.State != EntityState.Added), currentUser));
            auditEntries.RemoveAll(q => q.State != EntityState.Added);

            result = await context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (audiEntities.Any() || auditEntries.Any())
            {
                audiEntities.AddRange(context.FindAuditLogs(auditEntries, currentUser));//to get the generate key
                audiEntities.ForEach(q => q.TransssactionId = transsactionId);
                await context.Set<AuditLog>().AddRangeAsync(audiEntities);
                await context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            return result;
        }

        internal static List<EntityEntry> GetAuditEntries(this DbContext context)
        {
            return context.ChangeTracker.Entries().Where(e => (e.State == EntityState.Added ||
                                                              e.State == EntityState.Deleted ||
                                                              e.State == EntityState.Modified) && e.IsAttr<AuditableAttribute>()).ToList();
        }

        internal static List<AuditLog> FindAuditLogs(this DbContext context, IEnumerable<EntityEntry> addedEntries, string currentUser)
        {
            var result = new List<AuditLog>();

            foreach (var entry in addedEntries)
            {
                result.Add(MadeAuditLog(context, entry, currentUser));
            }
            return result;

        }

   
        private static AuditLog MadeAuditLog(DbContext context, EntityEntry entry, string currentUser)
        {
            var includedProperties = new List<string>();
            var entityKey = entry.GetPrimaryKey();
            var entityType = entry.Entity.GetType();

            var auditLog = new AuditLog
            {
                Created = DateTimeOffset.Now,
                EntityFullName = entityType.FullName,
                Table = entry.Metadata.Relational().TableName,
                EntityJson = entry.Serialize(),
                EntityId = entityKey,
                Operation = entry.State == EntityState.Unchanged ? EntityState.Added.ToString()
                                                                 : entry.State.ToString(),
                User = currentUser
            };

            if (entry.State == EntityState.Modified)
            {
                var props = entityType.GetProperties().Where(pi => !pi.IsAttr<NotAuditableAttribute>());
                includedProperties.AddRange(props.Select(pi => pi.Name));
                var propertiesChanged = (from prop in entry.Properties
                                         where (!Equals(prop.CurrentValue, prop.OriginalValue) && includedProperties.Contains(prop.Metadata.Name))
                                         select new PropertyAudit
                                         {
                                             PropertyName = prop.Metadata.Name,
                                             NewValue = Convert.ToString(prop.CurrentValue),
                                             OldValue = Convert.ToString(prop.OriginalValue)
                                         }).ToArray();

                auditLog.PropertiesAudits.AddRange(propertiesChanged);

            }
            return auditLog;
        }

        private static string GetPrimaryKey(this EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties)
            {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null)
                {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }


    }



}