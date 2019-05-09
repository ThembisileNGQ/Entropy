using System;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Projections.Prototype.Repository;

namespace Projections.Prototype
{
    public class AggregateEventStream<TJournal,TProjection, TProjectionId, TProjectionContext> : ReceiveActor
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
        
        public AggregateEventStream(
            IActorRef repository,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMap,
            TJournal journal)
        {
            _journal = journal;
            _eventMap = eventMap;
            _repository = repository;
            
            Receive<BeginAggregateEventStream>(Handle);
        }
        
        protected bool Handle(BeginAggregateEventStream command)
        {
            var materializer = ActorMaterializer.Create(Context);
            var actorContext = Context;
            var manager = command.Manager;
            var projectionContext = new TProjectionContext();
            
            projectionContext.StreamId = command.AggregateId;
            projectionContext.StartTimestamp = DateTimeOffset.UtcNow;
            
            _journal
                .CurrentEventsByPersistenceId(command.AggregateId, command.FromSequenceNumber, command.ToSequenceNumber)
                .Buffer(1024, OverflowStrategy.Backpressure)
                .RunForeach(eventEnvelope => Handle(actorContext, _repository, projectionContext, manager, eventEnvelope), materializer)
                .ContinueWith(x =>
                {
                    actorContext.GetLogger().Info("done");
                });
            
            return true;
        }
        
        public void Handle(
            IUntypedActorContext actorContext,
            IActorRef repository,
            TProjectionContext projectionContext,
            IActorRef manager,
            EventEnvelope eventEnvelope)
        {
            var projector = FindOrCreate(eventEnvelope.PersistenceId, actorContext);
            projector.Tell(new ProjectEvent<TProjectionContext>{Context = projectionContext, Event = eventEnvelope});
            actorContext.GetLogger().Info( "{0} - {1} - {2}", eventEnvelope.Event.GetType(),eventEnvelope.PersistenceId, eventEnvelope.SequenceNr);
        }
        
        protected virtual IActorRef FindOrCreate(string aggregateId, IUntypedActorContext context)
        {
            var aggregate = Context.Child(aggregateId);

            if(Equals(aggregate, ActorRefs.Nobody))
            {
                aggregate = context.ActorOf(Props.Create(() =>
                    new Projector<TJournal, TProjection, TProjectionId, TProjectionContext>(aggregateId,_repository,_eventMap)));
            }

            return aggregate;
        }
    }
    public class AggregateIdStream<TJournal, TProjectionId> : ReceiveActor
        where TProjectionId : IProjectionId
        where TJournal : IPersistenceIdsQuery, ICurrentPersistenceIdsQuery
    {
        private TJournal _journal { get; }
        public AggregateIdStream(
            TJournal journal)
        {
            _journal = journal;
            
            Receive<BeginAggregateIdStream>(Handle);
        }

        protected bool Handle(BeginAggregateIdStream command)
        {
            var materializer = ActorMaterializer.Create(Context);
            var context = Context;
            var manager = command.Manager;
            //GetPersistentIds
            _journal
                .CurrentPersistenceIds()
                .Buffer(1024, OverflowStrategy.Backpressure)
                .RunForeach(x => Handle(context, manager, x), materializer)
                .ContinueWith(x =>
                {
                    context.GetLogger().Info("done");
                });
            
            return true;
        }
        
        public void Handle(
            IUntypedActorContext context,
            IActorRef manager,
            string aggregateId)
        {
            context.GetLogger().Info(aggregateId);
            manager.Tell(new ReportAggregateId(aggregateId));
        }
        
        
    }
    
    //Copy Generic ExampleProjector
    public class Projector<TJournal, TProjection, TProjectionId, TProjectionContext> : ReceiveActor
        where TProjection : class, IProjection<TProjectionId>, new()
        where TProjectionContext : ProjectionContext, new()
        where TProjectionId : IProjectionId
        where TJournal : IReadJournal
    {
        protected string Id { get; set; }
        private IActorRef RepositoryRef { get; set; }
        protected ProjectorMap<TProjection, TProjectionId, TProjectionContext> ProjectorMap { get; set; }
        protected EventMapBuilder<TProjection, TProjectionId, TProjectionContext> EventMap { get; set; }
        protected IEventMap<TProjectionContext> EventMapHandler { get; set; }
        //protected Source<EventEnvelope, NotUsed> EventStream { get; set; }

        public Projector(
            string id,
            IActorRef repositoryRef,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMap)
        {
            Id = id;
            EventMap = eventMap;
            RepositoryRef = repositoryRef;
            //this probably needs to be sent the stream reference so that this actor can sink the stream and start processing the messages
            // from the journal, the wierd thing here is the stream can be either from tags or from actual persistence ids (aggregate or saga Ids)
            Receive<CreateProjectorSchema>(Handle);
            Receive<BeginProjectorStream>(Handle);
            Receive<ClearProjectorSchema>(Handle);
            Receive<DropProjectorSchema>(Handle);
            ReceiveAsync<ProjectEvent<TProjectionContext>>(Handle);
            
            ProjectorMap = new ProjectorMap<TProjection, TProjectionId, TProjectionContext>
            {
                Create = async (key, context, projector, shouldOverride) =>
                {
                    var projection = new TProjection()
                    {
                        Id = key
                    };

                    await projector(projection);

                    RepositoryRef.Tell(new Create<TProjection, TProjectionId>{ Projection = projection});
                },
                Update = async (key, context, projector, createIfMissing) =>
                {
                    var query = new Read<TProjectionId> {Key = key};
                    var projection =  await RepositoryRef.Ask<TProjection>(query);
                    
                    await projector(projection);

                    RepositoryRef.Tell(new Update<TProjection, TProjectionId>{ Projection = projection});
                },
                Delete = (key, context) =>
                {
                    var command = new Delete<TProjectionId> {Key = key};
                    RepositoryRef.Tell(command);

                    return Task.FromResult(true);
                },
                Custom = (context, projector) => projector()
            };

            EventMapHandler = EventMap.Build(ProjectorMap);
        }

        protected async Task Handle(ProjectEvent<TProjectionContext> command)
        {
            var a = await EventMapHandler.Handle(command.Event.Event, command.Context);
            if(a)
                Context.GetLogger().Info("handled");
            else
                Context.GetLogger().Warning("NOT handled");
        }
        
        protected bool Handle(CreateProjectorSchema command)
        {
            
            return true;
        }
        
        

        protected bool Handle(BeginProjectorStream command)
        {
            
            return true;
        }
        
        

        protected bool Handle(ClearProjectorSchema command)
        {
            
            return true;
        }

        protected bool Handle(DropProjectorSchema command)
        {
            
            return true;
        }
    }

    
    
    public class CreateProjectorSchema : ISetupProjectorSchema
    {
        
    }
    
    public class BeginProjectorStream : ISetupProjectorSchema
    {
        
    }
    
    public class BeginAggregateIdStream : ISetupProjectorSchema
    {
        public IActorRef Manager { get; set; }
    }
    
    public class BeginAggregateEventStream : ISetupProjectorSchema
    {
        public string AggregateId { get; set; }
        public long FromSequenceNumber { get; set; }
        public long ToSequenceNumber { get; set; }
        public IActorRef Manager { get; set; }
    }

    public class ReportAggregateId 
    {
        public string Id { get; }

        public ReportAggregateId(string id)
        {
            Id = id;
        }
    }
    
    public class BeginProjection : ISetupProjectorSchema
    {
        
    }
    
    public class ClearProjectorSchema : ISetupProjectorSchema
    {
        
    }
    
    public class DropProjectorSchema : ISetupProjectorSchema
    {
        
    }
    
    
    public class ProjectEvent<TProjectionContext> : ISetupProjectorSchema
        where TProjectionContext : ProjectionContext, new()
    {
        public TProjectionContext Context { get; set; }
        public EventEnvelope Event { get; set; }
    }

    public interface ISetupProjectorSchema
    {
        
    }
}