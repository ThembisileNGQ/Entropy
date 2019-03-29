using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LaundryBooker.Domain.Repositories;
using LaundryBooker.Infrastructure;
using LaundryBooker.Infrastructure.Repositories.UsersAggregate;
using LaundryBooker.Sts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            if(env.IsEnvironment("Docker"))
            {
                env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "LaundryBooker.Sts/out/wwwroot");
                env.WebRootFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "LaundryBooker.Sts/out/wwwroot"));
            }

            HostingEnvironment = env;

            LoggerFactory = loggerFactory;
            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("LaundryBooker.Sts application is starting.");
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDirectoryBrowser()
                .AddTransient<PostgresOptions>()
                .AddTransient<IUserRepository, UserRepository>();

            services
                .AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients())
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();

            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("LaundryBooker.Sts application has started.");
        }
    }
}