using System.Reflection.Metadata;
using Akka.Actor;
using Akka.Persistence.Query;

namespace Projections.Prototype
{
    public class ProjectorManager<TJournal, TProjectionContext, TProjection, TProjectionId> : ReceiveActor
        where TProjection : class, IProjection<TProjectionId>, new()
        where TProjectionContext : ProjectionContext, new ()
        where TProjectionId : IProjectionId
        where TJournal : IReadJournal,
        IPersistenceIdsQuery,
        ICurrentPersistenceIdsQuery,
        IEventsByPersistenceIdQuery,
        ICurrentEventsByPersistenceIdQuery,
        IEventsByTagQuery,
        ICurrentEventsByTagQuery
    {
        private TJournal _journal { get; }
        private IActorRef _repository { get; }

        private EventMapBuilder<TProjection,TProjectionId,TProjectionContext> _eventMap { get; }
        public ProjectorManager(
            IActorRef repository,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMap,
            TJournal journal)
        {
            _journal = journal;
            _repository = repository;
            _eventMap = eventMap;
            
            Receive<BeginProjection>(Handle);
            Receive<ReportAggregateId>(Handle);
        }

        public bool Handle(BeginProjection command)
        {
            var projector = Context.ActorOf(Props.Create(() => new AggregateIdStream<TJournal, TProjectionId>(_journal)),"aggregate-id-stream");
            projector.Tell(new BeginAggregateIdStream{ Manager = Self});
            return true;
        }
        
        public bool Handle(ReportAggregateId command)
        {
            var projector = Context.ActorOf(Props.Create(() => new AggregateEventStream<TJournal,TProjection, TProjectionId, TProjectionContext>(_repository,_eventMap, _journal)),$"aggregate-event-stream-{command.Id}");
            projector.Tell(new BeginAggregateEventStream{ Manager = Self, AggregateId = command.Id, FromSequenceNumber = 0, ToSequenceNumber = long.MaxValue});
            return true;
        }
    }
}