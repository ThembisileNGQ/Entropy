using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Domain.Model.GiftCard;
using Domain.Model.GiftCard.Commands;

namespace Application
{
    public class Program
    {
        public  static async Task Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("giftcard-system",Config.Postgres);
            var aggregateManager = actorSystem.ActorOf(Props.Create(() => new GiftCardManager()),"gift-card-manager");
            var hydrators = new List<IActorRef>();
            for (var i = 0; i < 100; i++)
            {
                var id = GiftCardId.New;
                
                var hydrator = actorSystem.ActorOf(Props.Create(() => new Hydrator(aggregateManager, id)),$"hydrator-{id}");;
                hydrators.Add(hydrator);
            }

            await Task.Delay(TimeSpan.FromMinutes(4));
            Console.ReadLine();
        }
    }

    public class Hydrator : ReceiveActor
    {
        private IActorRef _aggregateManager;
        private int _credits;
        private int _redemptions;
        private GiftCardId _id;
        private Random _rng;
        public Hydrator(IActorRef aggregateManager, GiftCardId id)
        {
            _aggregateManager = aggregateManager;
            _rng = new Random();
            var time = TimeSpan.FromSeconds(_rng.NextDouble() * 60);
            _credits = _rng.Next(50, 500);
            _redemptions = _rng.Next(2, 6);
            _id = id;
            _aggregateManager.Tell(new IssueCommand(_id,_credits));
            
            Context.GetLogger().Info($"[{_id}] started with [{_credits}]credits, sending Use in [{time.Seconds}s]");
            Context.System.Scheduler.ScheduleTellOnce(time,Self, new Use(), Self);
            Receive<Use>(Handle);
        }

        private bool Handle(Use command)
        {
            if (_redemptions <= 0) 
                return true;
            
            var use = _credits < 100 ? _credits : _rng.Next(_credits);
            var dice = _rng.Next(100);
            if (dice < 95)
            {
                _aggregateManager.Tell(new RedeemCommand(_id,use));
                _credits = _credits - use;
                if (_credits > 0)
                {
                    var time = TimeSpan.FromSeconds(_rng.NextDouble() * 60);
                    Context.GetLogger().Info($"[{_id}] redeemed with [{use}]credits, sending another Use in [{time.Seconds}s]");
                    Context.System.Scheduler.ScheduleTellOnce(time,Self,new Use(),Self);
                }

                _redemptions--;
            }
            else
            {
                _aggregateManager.Tell(new CancelCommand(_id));
                Context.GetLogger().Warning($"[{_id}] cancelled with [{_credits}]credits left.");
                _redemptions = 0;
            }
            
            
            return true;
        }

        private class Use{}
        
    }
    
    
    
    
}