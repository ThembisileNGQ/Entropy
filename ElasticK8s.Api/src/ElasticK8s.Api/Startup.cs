using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElasticK8s.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            HostingEnvironment = env;

            LoggerFactory = loggerFactory;
            var logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation("ElasticK8s.Api application is starting.");
        } 
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = LoggerFactory.CreateLogger<Startup>();
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            if (HostingEnvironment.IsDevelopment())
            {

                logger.LogInformation("ElasticK8s.Api is in Development.");
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                services.AddSingleton<KubernetesClientConfiguration>(config);
            }
            else
            {
                logger.LogInformation("ElasticK8s.Api is in K8s.");
                var config = KubernetesClientConfiguration.InClusterConfig();
                services.AddSingleton<KubernetesClientConfiguration>(config);
            }
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
        }
    }
}