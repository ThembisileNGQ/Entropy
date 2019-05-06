using System.Security.Principal;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;

namespace Projections.Prototype
{
    public abstract class PersistentIdStream<TJournal, TIdentity> : ReceiveActor
        where TIdentity : IIdentity
        where TJournal : IPersistenceIdsQuery, ICurrentPersistenceIdsQuery
    {
        
    }
    
    //Copy Generic ExampleProjector
    public abstract class Projector<TJournal,TProjection,TIdentity,TProjectionContext> : ReceiveActor
        where TProjectionContext : ProjectionContext
        where TIdentity : IProjectionId
        where TJournal : IReadJournal,
        IPersistenceIdsQuery,
        ICurrentPersistenceIdsQuery,
        IEventsByPersistenceIdQuery,
        ICurrentEventsByPersistenceIdQuery,
        IEventsByTagQuery,
        ICurrentEventsByTagQuery
    {
        protected ProjectorMap<TProjection, TIdentity, TProjectionContext> ProjectorMap { get; }
        protected EventMapBuilder<TProjection, TIdentity, TProjectionContext> EventMap { get; }
        protected Source<EventEnvelope, NotUsed> EventStream { get; }

        protected Projector(
            ProjectorMap<TProjection,TIdentity,TProjectionContext> projectorMap,
            EventMapBuilder<TProjection,TIdentity,TProjectionContext> eventMap,
            Source<EventEnvelope, NotUsed> eventStream)
        {
            ProjectorMap = projectorMap;
            EventStream = eventStream;
            EventMap = eventMap;
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