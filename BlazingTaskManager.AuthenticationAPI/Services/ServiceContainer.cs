using BlazingTaskManager.AuthenticationAPI.Data;
using BlazingTaskManager.AuthenticationAPI.Repositories;
using BlazingTaskManager.Shared.DI;
using BlazingTaskManager.Shared.Services.AuthService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlazingTaskManager.AuthenticationAPI.Services
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //  DB connectivity
            //  Auth scheme
            SharedServiceContainer.AddSharedServices<AuthenticationDataContext>(services, config, config["MySerilog:FileName"]!);

            //  Register JWT Service
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Secret"]!))
                };
            });

            // DI
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<IJWTUtilities, JWTUtilities>();


            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Registe middleware
            //  Global Exception: hadles errors
            //  Restrict client access to  API Gateway
            //SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
