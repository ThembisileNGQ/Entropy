﻿using Akka.Actor;
using Domain.Model.Car;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WriteApi
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
            logger.LogInformation("WriteApi application is starting.");
        } 
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var actorSystem = ActorSystem.Create("write-system");
            var aggregateManager = actorSystem.ActorOf(Props.Create(() => new CarAggregateManager()), "car-aggregatemanager");

            services
                .AddAkkatecture(actorSystem)
                .AddActorReference<CarAggregateManager>(aggregateManager);
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