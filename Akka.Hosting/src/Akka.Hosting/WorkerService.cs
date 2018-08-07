using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Hosting.Actors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Akka.Hosting
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private ActorSystem ActorSystem { get; set; }
        private IActorRef GreetingActor { get; set; }
        public WorkerService(
            ILogger<WorkerService> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            ActorSystem = ActorSystem.Create("hosted-actorsystem");
            GreetingActor = ActorSystem.ActorOf(Props.Create(() => new GreetingActor()));

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting background job.");
            

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Saying Hello");

                var message = new Hello();
                GreetingActor.Tell(message);

                await Task.Delay(2000, stoppingToken);

            }

            _logger.LogInformation("Shutting down");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saying Goodbye");

            var message = new Goodbye();
            GreetingActor.Tell(message);

            await ActorSystem.Terminate();
            
        }
    }
}
