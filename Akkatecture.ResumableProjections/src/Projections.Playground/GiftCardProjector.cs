using System;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams.Dsl;
using Akkatecture.Aggregates;
using Domain.Model.GiftCard;
using Domain.Model.GiftCard.Events;
using Projections.Prototype;
using static Projections.Playground.IProjectionIdExtensions;
using Projections.Prototype.Extensions;
using Projections.Prototype.Repository;

namespace Projections.Playground
{
    public class GiftCardProjection : IProjection<GiftCardProjectionId>
    {
        public GiftCardProjectionId Id { get; set; }
        public DateTime Issued { get; set; }
        public int Credits { get; set; }
        public bool IsCancelled { get; set; }
    }
    /*public class GiftCardProjector : Projector<SqlReadJournal,GiftCardProjection,GiftCardProjectionId,GiftCardProjectionContext>
    {
        public GiftCardProjector(
            string projectorId,
            IActorRef repository,
            ProjectorMap<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext> projectorMap,
            EventMapBuilder<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext> eventMap,
            Source<EventEnvelope, NotUsed> eventStream) 
            : base(projectorId,repository, eventMap)
        {
            
        }

        public static EventMapBuilder<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext> GetEventMap()
        {
            var eventMap = new EventMapBuilder<GiftCardProjection, GiftCardProjectionId, GiftCardProjectionContext>();

            eventMap.Map<DomainEvent<GiftCard, GiftCardId, IssuedEvent>>()
                .AsCreateOf(x => From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits = evt.AggregateEvent.Credits;
                    projection.IsCancelled = false;
                    projection.Issued = evt.Timestamp.UtcDateTime;
                });
            
            eventMap.Map<DomainEvent<GiftCard, GiftCardId, RedeemedEvent>>()
                .AsUpdateOf(x => From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits -= evt.AggregateEvent.Credits;
                });
            
            
            eventMap.Map<DomainEvent<GiftCard, GiftCardId, CancelledEvent>>()
                .AsUpdateOf(x => From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.IsCancelled = false;
                });

            return eventMap;
        }
    }*/
}