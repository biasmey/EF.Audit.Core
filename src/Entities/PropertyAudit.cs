using System;
using System.ComponentModel.DataAnnotations;

namespace EF.Audit
{
    public class PropertyAudit
    {
       
        [Key]
        public long Id { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string PropertyName { get; set; }
        public long AuditLogId { get; set; }
        public AuditLog AuditLog { get; set; }
    }
}