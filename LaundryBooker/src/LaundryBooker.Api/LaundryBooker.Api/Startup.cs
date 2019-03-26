using IdentityModel;
using LaundryBooker.Domain.Repositories;
using LaundryBooker.Infrastructure;
using LaundryBooker.Infrastructure.Repositories.BookingMonthAggregate;
using LaundryBooker.Infrastructure.Repositories.UsersAggregate;
using LaundryBooker.Sts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using LaundryBooker.Api.Models;

namespace LaundryBooker.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get;  }
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            HostingEnvironment = env;

            LoggerFactory = loggerFactory;
            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("LaundryBooker.Api application is starting.");
        } 
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder =>
                {
                    corsBuilder.AllowAnyHeader();
                    corsBuilder.AllowAnyMethod();
                    corsBuilder.AllowAnyOrigin();
                    corsBuilder.AllowCredentials();
                });
            });

            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["AUTHORIZATIONAUTHORITY_URL"];
                    options.Audience = Configuration["AUTHORIZATION_AUDIENCE"];
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                });

            services.AddAuthorization(options =>
            {
                options
                    .AddScopePolicy("bookings.read")
                    .AddScopePolicy("bookings.write")
                    .AddScopePolicy("bookings.delete");
            });

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services
                .AddTransient<PostgresOptions>()
                .AddTransient<BookingResponseModelFactory>()
                .AddTransient<IBookingMonthRepository, BookingMonthRepository>()
                .AddTransient<IUserRepository, UserRepository>();
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseAuthentication();

            app.Map("/api", api =>
            {
                api.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}");
                });
            });

            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("LaundryBooker.Api application has started.");
        }

    }
}