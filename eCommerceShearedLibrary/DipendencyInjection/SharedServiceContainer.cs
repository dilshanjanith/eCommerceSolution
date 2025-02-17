
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Microsoft.AspNetCore.Builder;
using eCommerceShearedLibrary.Middleware;

namespace eCommerceShearedLibrary.DipendencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedService<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName ) where TContext : DbContext
        {
            //Add Genaric database context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"), sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
                ));

            //Configure Seril Loging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Add JWTAuthentication Scheme
            JWTAuthenticationServiceExtensions.AddJWTAuthenticationScheme(services, config);

            return services;
        }

        public static IApplicationBuilder UseShearedPolicies(this IApplicationBuilder app)
        {
            //Use Global Exception
            app.UseMiddleware<GlobalExceptions>();

            //Register Middleware to block All outside API calls
            app.UseMiddleware<ListenToOnlyApiGeteway>();

            return app;
        }
    }
}
