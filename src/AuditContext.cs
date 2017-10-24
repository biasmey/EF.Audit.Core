using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using EF.Audit.Core.Extensions;
using System;
using System.Threading.Tasks;
using System.Threading;
using EF.Audit.Core.Entities;

namespace EF.Audit.Core
{
    public class AuditContext: DbContext 
    {
        private readonly Func<AuditUser> _getAuditUser;
        public AuditContext(Func<AuditUser> getAuditUser, DbContextOptions options) :base(options)
        {
            _getAuditUser = getAuditUser ?? throw new Exception($"{nameof(Func<AuditUser>)} is null , in your Startup.ConfigureServices add {nameof(IServiceCollectionExtension.AddAudit)}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.EnableAudit();
        }

        public override int SaveChanges()
        {
            return SaveChanges(true);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess).Result;
        }


        public override async  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await base.SaveChangesAsync(true,cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var transsactionId = Guid.NewGuid();
            List<AuditLog> audiEntities = new List<AuditLog>();
            int result = 0;
            var auditUser = _getAuditUser();
            var auditEntries = this.GetAuditEntries().ToList();
            audiEntities.AddRange(this.FindAuditLogs(auditEntries.Where(q => q.State != EntityState.Added), auditUser?.CurrentUser));
            auditEntries.RemoveAll(q => q.State != EntityState.Added);

            result = await base.SaveChangesAsync(acceptAllChangesOnSuccess,cancellationToken);

            if (audiEntities.Any() || auditEntries.Any())
            {
                audiEntities.AddRange(this.FindAuditLogs(auditEntries, auditUser?.CurrentUser));//to get the generate key
                audiEntities.ForEach(q => q.TransssactionId = transsactionId);
                Set<AuditLog>().AddRange(audiEntities);
                await base.SaveChangesAsync(acceptAllChangesOnSuccess,cancellationToken);
            }
            return result;
        }





    }

   
}
