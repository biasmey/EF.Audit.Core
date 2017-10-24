using EF.Audit.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EF.Audit.Core.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddAudit(this IServiceCollection services)
        {
            Func<IServiceProvider, AuditUser> getAuditUser = (sp) => new AuditUser { CurrentUser = sp.GetService<IHttpContextAccessor>().HttpContext.User?.Identity?.Name };

            services.AddScoped(typeof(Func<AuditUser>), sp => {
                Func<AuditUser> func = () => {
                    return getAuditUser(sp);
                };

                return func;
            });
            return services;
        }
    }
}
