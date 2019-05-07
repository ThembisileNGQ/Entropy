using System;
using Akka;
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
    public class GiftCardProjector : Projector<SqlReadJournal,GiftCardProjection,GiftCardProjectionId,TestContext>
    {
        public GiftCardProjector(
            string projectorId,
            ProjectionRepository<GiftCardProjection,GiftCardProjectionId> repository,
            ProjectorMap<GiftCardProjection, GiftCardProjectionId, TestContext> projectorMap,
            EventMapBuilder<GiftCardProjection, GiftCardProjectionId, TestContext> eventMap,
            Source<EventEnvelope, NotUsed> eventStream) 
            : base(projectorId,repository,projectorMap, eventMap, eventStream)
        {
            EventMap = new EventMapBuilder<GiftCardProjection, GiftCardProjectionId, TestContext>();

            EventMap.Map<IDomainEvent<GiftCard, GiftCardId, IssuedEvent>>()
                .AsCreateOf(x => From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits = evt.AggregateEvent.Credits;
                    projection.IsCancelled = false;
                    projection.Issued = evt.Timestamp.UtcDateTime;
                });
            
            EventMap.Map<IDomainEvent<GiftCard, GiftCardId, RedeemedEvent>>()
                .AsUpdateOf(x => From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.Credits -= evt.AggregateEvent.Credits;
                });
            
            
            EventMap.Map<IDomainEvent<GiftCard, GiftCardId, CancelledEvent>>()
                .AsUpdateOf(x => From(x.AggregateIdentity))
                .Using((projection, evt) =>
                {
                    projection.IsCancelled = false;
                });

            var map = eventMap.Build(ProjectorMap);
        }
    }
}