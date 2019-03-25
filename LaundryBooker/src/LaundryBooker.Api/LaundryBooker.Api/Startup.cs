using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LaundryBooker.Api
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
            logger.LogInformation("LaundryBooker.Api application is starting.");
        } 
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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