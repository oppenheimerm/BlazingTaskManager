using BlazingTaskManager.Shared.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BlazingTaskManager.Shared.DI
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
            IConfiguration config, string fileName) where TContext : DbContext
        {
            //  Add generic database context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config.GetConnectionString("BlazingTaskManageConnectionString"), sqloptions =>
                sqloptions.EnableRetryOnFailure()
                ));

            //  Configure serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();


            // Tryin this one
            BearerScheme.AddBearerScheme(services, config);


            return services;
        }


        /// <summary>
        /// Setup global <see cref="GlobalException"/> middleware and restrict client access
        /// with <see cref="APIGatewayListener"/>
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use our global exception
            app.UseMiddleware<GlobalException>();

            //  Register our middleware to block direct API service communication
            app.UseMiddleware<APIGatewayListener>();

            return app;
        }
    }
}
