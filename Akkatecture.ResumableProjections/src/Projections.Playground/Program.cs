using Domain.Model.GiftCard.Events;
using Projections.Prototype;
using System;
using System.Threading.Tasks;
using Akka.Configuration;
using Akka.Persistence.Query.Sql;
using Domain.Model.GiftCard;

namespace Projections.Playground
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(Config.Postgres);

            ProjectorManager
                .Create("GiftCardProjectionManager", config)
                .Using<SqlReadJournal>(SqlReadJournal.Identifier)
                .WithEventMap(GiftCardProjector.GetEventMap())
                .RunAggregateProjection();
            //var projectionManager = new ProjectorManager<SqlReadJournal,GiftCardProjection,GiftCardProjectionId,GiftCardProjectionContext>
            /*
             *
             * ProjectorManager
             * .Using<SqlReadJournal>()
             * .WithProjection<TProjectionContext, TProjection, TProjectionIdentity>()
             * .WithProjectionMap<TProjectionContext, TProjection, TProjectionIdentity()
             * .WithEventMap<TProjectionContext>()
             * .Project()
             * 
             */    
            Console.ReadLine();
            
        }

        public static async Task PlayThing()
        {
            var mapBuilder = new EventMapBuilder<GiftCardProjectionContext>();

            mapBuilder
                .Map<RedeemedEvent>()
                .When((evt, context) =>
                {
                    Console.WriteLine("in conditional");
                    return Task.FromResult(evt.Credits > 0);
                })
                .As((evt, context) => 
                {
                    Console.WriteLine("in as");
                    return Task.CompletedTask;
                });

            var projectorMap = new ProjectorMap<GiftCardProjectionContext>
            {
                Custom = (context, projector) =>
                {
                    Console.WriteLine("in projector");

                    return projector();
                }
            };

            var map = mapBuilder.Build(projectorMap);

            var a = await map.Handle(new RedeemedEvent(5), new GiftCardProjectionContext());

            Console.ReadLine();
        }
    }

    public class GiftCardProjectionContext : ProjectionContext
    {

    }

    public class GiftCardProjectionId : IProjectionId
    {
        public string Value { get; set; }
    }

    
    
    
    /*public class IssuedGiftCardsProjection : IProjection
    {
        public IProjection Id { get; set; }
        public int Count { get; set; }
    }
    
    
    public class DepletedGiftCardsProjection : IProjection
    {
        public IProjection Id { get; set; }
        public int Count { get; set; }
    }*/

    public static class IProjectionIdExtensions
    {
        public static GiftCardProjectionId From(GiftCardId id)
        {
            return new GiftCardProjectionId(){Value = id.Value};
        }
    }
}
