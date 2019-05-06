using Akka;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams.Dsl;
using Projections.Prototype;

namespace Projections.Playground
{
    public class GiftCardProjector : Projector<SqlReadJournal,GiftCardProjection,GiftCardProjectionId,TestContext>
    {
        public GiftCardProjector(
            ProjectorMap<GiftCardProjection, GiftCardProjectionId, TestContext> projectorMap,
            EventMapBuilder<GiftCardProjection, GiftCardProjectionId, TestContext> eventMap,
            Source<EventEnvelope, NotUsed> eventStream) 
            : base(projectorMap, eventMap, eventStream)
        {
            
        }
    }
}