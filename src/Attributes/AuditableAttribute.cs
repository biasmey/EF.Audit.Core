using System;

namespace EF.Audit
{
    [AttributeUsage(AttributeTargets.Class , AllowMultiple = false, Inherited = false)]
    public sealed class AuditableAttribute: Attribute
    {
    }
}