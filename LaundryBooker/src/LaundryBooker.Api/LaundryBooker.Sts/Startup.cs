using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryBooker.Domain.Repositories;
using LaundryBooker.Infrastructure;
using LaundryBooker.Infrastructure.Repositories.UsersAggregate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LaundryBooker.Sts
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public ILoggerFactory LoggerFactory { get;  }
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            HostingEnvironment = env;

            LoggerFactory = loggerFactory;
            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("LaundryBooker.Sts application is starting.");
        } 
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<PostgresOptions>();

            services
                .AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients())
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddDeveloperSigningCredential();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("LaundryBooker.Sts application has started.");
        }
    }
}