using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF.Audit
{
    public class AuditLog
    {
        public AuditLog()
        {
            PropertiesAudits = new List<PropertyAudit>();
        }

        [Key]
        public long Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string EntityFullName { get; set; }
        public string Table { get; set; }
        public string EntityJson { get; set; }
        public string EntityId { get; set; }
        public string User { get; set; }
     
        public string Operation { get; set; }
        public Guid TransssactionId { get; set; }

        public List<PropertyAudit> PropertiesAudits { get; set; }
    }
}