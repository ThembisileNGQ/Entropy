using Domain.Model.GiftCard.Events;
using Projections.Prototype;
using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akkatecture.Aggregates;
using Domain.Model.GiftCard;
using Projections.Prototype.Extensions;

namespace Projections.Playground
{
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            /*
             * API NOTES
             *
             * For tagged events we will probably use IProjectionLocator to map tagged events to their appropriate keys.
             *
             * ProjectionManager will be an actor that has been propsed up for sending Source<EventEnvelope, NotUsed> to child projector
             * child projector will have an Id and this Id will either be a consequence of it being a persistentId or it will
             * be a consequence of it being a Tag + ProjectionLocator
             */
            var config = ConfigurationFactory.ParseString(Config.Postgres);

            var eventMap = new EventMapBuilder<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext>();

            eventMap.Map<IDomainEvent<GiftCard, GiftCardId, IssuedEvent>>()
                .AsCreateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits = evt.AggregateEvent.Credits;
                    projection.IsCancelled = false;
                    projection.Issued = evt.Timestamp.UtcDateTime;
                });
            
            eventMap.Map<IDomainEvent<GiftCard, GiftCardId, RedeemedEvent>>()
                .AsUpdateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits -= evt.AggregateEvent.Credits;
                });
            
            
            eventMap.Map<IDomainEvent<GiftCard, GiftCardId, CancelledEvent>>()
                .AsUpdateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.IsCancelled = false;
                });

            
            ProjectorBuilder
                .Create("giftcard-projection-manager", config)
                .Using<SqlReadJournal>(SqlReadJournal.Identifier)
                .WithEventMap(eventMap)
                .RunAggregateProjection();
            
            /*
             *
             * ProjectorBuilder
                .Create("GiftCardProjectionManager", config)
                .Using<SqlReadJournal>(SqlReadJournal.Identifier)
                .WithEventMap(GiftCardProjector.GetEventMap())
                .RunAggregateProjection();
                
               ProjectorBuilder
                .Create("IssuedGiftCardsProjecection", config)
                .Using<SqlReadJournal>(SqlReadJournal.Identifier)
                .WithEventMap(GiftCardProjector.GetEventMapForIssued())
                .Using<ProjectorLocator>(locator)
                .RunTaggedEventProjection();
             *
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
