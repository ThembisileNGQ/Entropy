using Akka.Actor;
using Akka.Configuration;
using Domain.Model.GiftCard;
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
            var logger = LoggerFactory.CreateLogger<Startup>();
            logger.LogInformation("WriteApi application is starting.");
        } 
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var config = ConfigurationFactory.ParseString(Config.Postgres);
            var actorSystem = ActorSystem.Create("write-system", config);
            var aggregateManager = actorSystem.ActorOf(Props.Create(() => new GiftCardManager()), "car-aggregatemanager");

            services
                .AddAkkatecture(actorSystem)
                .AddActorReference<GiftCardManager>(aggregateManager);
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
            logger.LogInformation("WriteApi application is starting.");
        }
    }
}