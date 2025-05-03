using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingTaskManager.Shared.DI
{
    public static class BearerScheme
    {
        public static IServiceCollection AddBearerScheme(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("api", p => {
                    p.RequireAuthenticatedUser();
                    p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
                });

            return services;
        }
    }
}
