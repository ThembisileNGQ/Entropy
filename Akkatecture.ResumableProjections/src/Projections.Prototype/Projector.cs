using System.Security.Principal;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Projections.Prototype.Repository;

namespace Projections.Prototype
{
    public abstract class PersistentIdStream<TJournal, TIdentity> : ReceiveActor
        where TIdentity : IIdentity
        where TJournal : IPersistenceIdsQuery, ICurrentPersistenceIdsQuery
    {
        
    }
    
    //Copy Generic ExampleProjector
    public abstract class Projector<TJournal,TProjection,TProjectionId,TProjectionContext> : ReceiveActor
        where TProjection : class, IProjection<TProjectionId>
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