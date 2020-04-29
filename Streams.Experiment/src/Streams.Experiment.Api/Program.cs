using System;
using System.IO;
using Akka.Actor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Streams.Experiment.Domain;
using Streams.Experiment.Domain.Aggregates;
using Streams.Experiment.Domain.Services;

namespace Streams.Experiment.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddEnvironmentVariables();
                })
                .ConfigureLogging((context, logger) =>
                {
                    logger
                        .AddConsole();
                })
                .ConfigureServices((context, collection) =>
                {
                    var config = @"akka.loglevel = INFO
                           akka.actor.debug.receive = on
                           akka.actor.debug.unhandled = on
                           akka.actor.debug.event-stream = on
                           akka.stdout-loglevel = INFO";


                    var actorSystem = ActorSystem.Create("bank-system", config);
                    var aggregateManager = actorSystem.ActorOf(Props.Create(() => new AggregateManager()));

                    var num = 1;
                    
                    foreach (var id in SeedData.UserIds)
                    {
                        var name = $"User{num}";
                        var email = $"user-{num}@foo.com";
                        var registerUser = new RegisterUser(id,name,email,"SE","1234567890");
                        var savings = 2 * num;
                        var credit = num;
                        var openAccount = new OpenBankAccount(id,savings,credit);
                        
                        aggregateManager.Tell(registerUser, ActorRefs.NoSender);
                        aggregateManager.Tell(openAccount, ActorRefs.NoSender);
                        num++;
                    }
                    
                    collection
                        .AddSingleton(actorSystem)
                        .AddSingleton(_ => new AggregateManagerRef(aggregateManager))
                        .AddTransient<UserClubAssignmentService>();

                });
    }

    public static class GlobalRef
    {
        public static IActorRef AggregateManager { get; set; }
    }
}