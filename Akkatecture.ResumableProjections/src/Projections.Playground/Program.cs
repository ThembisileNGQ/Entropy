using Domain.Model.GiftCard.Events;
using Projections.Prototype;
using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akkatecture.Aggregates;
using Domain.Model.GiftCard;
using Projections.Prototype.Extensions;
using Projections.Prototype.Repository;

namespace Projections.Playground
{
    public class Program
    {
        public static async Task Rubbish(Akka.Configuration.Config config)
        {
            var actorSystem = ActorSystem.Create("tessttt", config);
            
            var repositoryActor =
                actorSystem.ActorOf(Props.Create(() => new RepositoryActor<GiftCardProjection, GiftCardProjectionId>()),"repository-actor");
           
            var projectorMap = new ProjectorMap<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext>
            {
                Create = async (key, context, projector, shouldOverride) =>
                {
                    var projection = new GiftCardProjection()
                    {
                        Id = key
                    };

                    await projector(projection);

                    repositoryActor.Tell(new Create<GiftCardProjection, GiftCardProjectionId>{ Projection = projection});
                },
                Update = async (key, context, projector, createIfMissing) =>
                {
                    var query = new Read<GiftCardProjectionId> {Key = key};
                    var projection =  await repositoryActor.Ask<GiftCardProjection>(query);
                    
                    await projector(projection);

                    repositoryActor.Tell(new Update<GiftCardProjection, GiftCardProjectionId>{ Projection = projection});
                },
                Delete = (key, context) =>
                {
                    var command = new Delete<GiftCardProjectionId> {Key = key};
                    repositoryActor.Tell(command);

                    return Task.FromResult(true);
                },
                Custom = (context, projector) => projector()
            };

            var eventMap = new EventMapBuilder<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext>();

            eventMap.Map<DomainEvent<GiftCard, GiftCardId, IssuedEvent>>()
                .AsCreateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits = evt.AggregateEvent.Credits;
                    projection.IsCancelled = false;
                    projection.Issued = evt.Timestamp.UtcDateTime;
                });
            
            eventMap.Map<DomainEvent<GiftCard, GiftCardId, RedeemedEvent>>()
                .AsUpdateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits -= evt.AggregateEvent.Credits;
                });
            
            
            eventMap.Map<DomainEvent<GiftCard, GiftCardId, CancelledEvent>>()
                .AsUpdateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.IsCancelled = false;
                });

           var handler = eventMap.Build(projectorMap);
            var context2 = new GiftCardProjectionContext();

            var mat = ActorMaterializer.Create(actorSystem);

            await PersistenceQuery
                .Get(actorSystem)
                .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier)
                .CurrentEventsByPersistenceId("giftcard-a8fd515e-d4fb-4bf6-9d7f-67abdd0fdeef", 0, 10)
                .RunForeach(async x => await handler.Handle(x.Event, context2), mat);
        }
        
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

            
            var eventMapBuilder = new EventMapBuilder<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext>();

            eventMapBuilder.Map<DomainEvent<GiftCard, GiftCardId, IssuedEvent>>()
                .AsCreateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits = evt.AggregateEvent.Credits;
                    projection.IsCancelled = false;
                    projection.Issued = evt.Timestamp.UtcDateTime;
                });
            
            eventMapBuilder.Map<DomainEvent<GiftCard, GiftCardId, RedeemedEvent>>()
                .AsUpdateOf(x =>
                {
                    return IProjectionIdExtensions.From(x.AggregateIdentity);
                })
                .Using((projection, evt) =>
                {
                    projection.Credits -= evt.AggregateEvent.Credits;
                });
            
            
            eventMapBuilder.Map<DomainEvent<GiftCard, GiftCardId, CancelledEvent>>()
                .AsUpdateOf(x => IProjectionIdExtensions.From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.IsCancelled = false;
                });
            
            
            
            var builder = ProjectorBuilder
                .Create("giftcard-projection-manager", config);

            var repositoryActor =
                builder.ActorSystem.ActorOf(Props.Create(() => new RepositoryActor<GiftCardProjection, GiftCardProjectionId>()),"repository-actor-monkas");
           
            var projectorMap = new ProjectorMap<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext>
            {
                Create = async (key, context, projector, shouldOverride) =>
                {
                    var projection = new GiftCardProjection()
                    {
                        Id = key
                    };

                    await projector(projection);

                    repositoryActor.Tell(new Create<GiftCardProjection, GiftCardProjectionId>{ Projection = projection});
                },
                Update = async (key, context, projector, createIfMissing) =>
                {
                    var query = new Read<GiftCardProjectionId> {Key = key};
                    var projection =  await repositoryActor.Ask<GiftCardProjection>(query);
                    
                    await projector(projection);

                    repositoryActor.Tell(new Update<GiftCardProjection, GiftCardProjectionId>{ Projection = projection});
                },
                Delete = (key, context) =>
                {
                    var command = new Delete<GiftCardProjectionId> {Key = key};
                    repositoryActor.Tell(command);

                    return Task.FromResult(true);
                },
                Custom = (context, projector) => projector()
            };
                
                builder
                .Using<SqlReadJournal>(SqlReadJournal.Identifier)
                .WithEventMapBuilder(eventMapBuilder)
                .WithProjectorBuilder(projectorMap)
                .RunAggregateProjection(repositoryActor);
            
            
            
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
            Console.WriteLine(" here ");
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
