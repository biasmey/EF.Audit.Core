# EF.Audit.Core

Easy audit capabilities for EF core projects.

Auditable Entities
===================
First of all, you need to mark all your auditable entities using  `Auditable` and  `NotAuditable` attributes.

**Auditable:** Can be used to mark a class (including all properties).
**NotAuditable:** Marks a property as not auditable.

Enable Audit
=============
Install EF.Audit (version >= 3.0.0) nuget package.
You have two way to use it 
1 - Your dbcontext will inherit the AuditContext and adds services required for using AuditContext.
``` services.AddAudit(); ```
  
```csharp
public class BloggingContext : AuditContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public BloggingContext(Func<AuditUser> fn, DbContextOptions<BloggingContext> options) :base(fn,options)
    {
    }
      
}
```
    
2 - Other way is using extension method `EnableAudit` and `SaveChangesAndAudit` or `SaveChangesAndAuditAsync`
     
```csharp
public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public BloggingContext(DbContextOptions<BloggingContext> options) :base(options)
    {
    }
      
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.EnableAudit(); 
    }
} 
```
    
```csharp
var blog = new Blog { Url = "http://blogs.msdn.com/adonet" };
var count = context.SaveChangesAndAudit(); 
```

Authors and Contributors
========================
Biasmey Morgado Guirola (biasmey), 
José Antonio Plá Rodríguez (jpla2005)

Contributions
=============
Any contribution is welcomed.

