using System.Security.Principal;
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
    public class PersistentIdStream<TJournal, TProjectionId> : ReceiveActor
        where TProjectionId : IProjectionId
        where TJournal : IPersistenceIdsQuery, ICurrentPersistenceIdsQuery
    {
        private TJournal _journal { get; }
        public PersistentIdStream(
            TJournal journal)
        {
            _journal = journal;
            
            Receive<BeginPersistentIdStream>(Handle);
        }

        protected bool Handle(BeginPersistentIdStream command)
        {
            var materializer = ActorMaterializer.Create(Context);
            var context = Context;
            //GetPersistentIds
            _journal
                .CurrentPersistenceIds()
                .Buffer(200, OverflowStrategy.Backpressure)
                .RunForeach(x => Handle(context, x), materializer)
                .ContinueWith(x =>
                {
                    context.GetLogger().Info("done");
                });
            
            return true;
        }
        
        public void Handle(
            IUntypedActorContext context,
            string aggregateId)
        {
            context.GetLogger().Info(aggregateId);
        }
        
        
    }
    
    //Copy Generic ExampleProjector
    public class Projector<TJournal,TProjection,TProjectionId,TProjectionContext> : ReceiveActor
        where TProjection : class, IProjection<TProjectionId>, new()
        where TProjectionContext : ProjectionContext
        where TProjectionId : IProjectionId
        where TJournal : IReadJournal,
        IPersistenceIdsQuery,
        ICurrentPersistenceIdsQuery,
        IEventsByPersistenceIdQuery,
        ICurrentEventsByPersistenceIdQuery,
        IEventsByTagQuery,
        ICurrentEventsByTagQuery
    {
        protected string Id { get; set; }
        private ProjectionRepository<TProjection,TProjectionId> Repository { get; set; }
        protected ProjectorMap<TProjection, TProjectionId, TProjectionContext> ProjectorMap { get; set; }
        protected EventMapBuilder<TProjection, TProjectionId, TProjectionContext> EventMap { get; set; }
        protected Source<EventEnvelope, NotUsed> EventStream { get; set; }

        protected Projector(
            string id,
            ProjectionRepository<TProjection,TProjectionId> repository,
            ProjectorMap<TProjection,TProjectionId,TProjectionContext> projectorMap,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMap,
            Source<EventEnvelope, NotUsed> eventStream)
        {
            Id = id;
            ProjectorMap = projectorMap;
            EventStream = eventStream;
            EventMap = eventMap;
            Repository = repository;
            //this probably needs to be sent the stream reference so that this actor can sink the stream and start processing the messages
            // from the journal, the wierd thing here is the stream can be either from tags or from actual persistence ids (aggregate or saga Ids)
            Receive<CreateProjectorSchema>(Handle);
            Receive<BeginProjectorStream>(Handle);
            Receive<ClearProjectorSchema>(Handle);
            Receive<DropProjectorSchema>(Handle);
            
            ProjectorMap = new ProjectorMap<TProjection, TProjectionId, TProjectionContext>
            {
                Create = async (key, context, projector, shouldOverride) =>
                {
                    var projection = new TProjection()
                    {
                        Id = key
                    };

                    await projector(projection);

                    repository.Add(projection);
                },
                Update = async (key, context, projector, createIfMissing) =>
                {
                    var projection = repository.Find(key);
                    
                    await projector(projection);

                    repository.Add(projection);
                },
                Delete = (key, context) =>
                {
                    repository.RemoveByKey(key);

                    return Task.FromResult(true);
                },
                Custom = (context, projector) => projector()
            };
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
    
    public class BeginPersistentIdStream : ISetupProjectorSchema
    {
        
    }
    
    public class ClearProjectorSchema : ISetupProjectorSchema
    {
        
    }
    
    public class DropProjectorSchema : ISetupProjectorSchema
    {
        
    }

    public interface ISetupProjectorSchema
    {
        
    }
}