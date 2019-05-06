using System.Reflection.Metadata;
using System.Security.Principal;
using Akka.Actor;
using Akka.Persistence.Query;
using Akkatecture.Aggregates;

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
        protected IProjectionLocator<TIdentity> ProjectionLocator { get; }
        protected ProjectorMap<TProjection, TIdentity, TProjectionContext> ProjectorMap { get; }

        public virtual bool ShouldProject(IDomainEvent domainEvent)
        {
            return ProjectionLocator.LocateProjector(domainEvent) != null;
        }

        protected Projector(
            IProjectionLocator<TIdentity> projectionLocator, 
            ProjectorMap<TProjection,TIdentity,TProjectionContext> projectorMap)
        {
            ProjectionLocator = projectionLocator;
            ProjectorMap = projectorMap;
            //this probably needs to be sent the stream reference so that this actor can sink the stream and start processing the messages
            //from the journal, the weird thing here is the stream can be either from tags or from actual persistence ids (aggregate or saga Ids)
            Receive<BeginProjectorStream>(Handle);
        }

        protected bool Handle(BeginProjectorStream command)
        {
            
            return true;
        }

        protected virtual void Handle(EventEnvelope envelope)
        {
            //ProjectorMap.
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

    public interface ISetupProjectorSchema
    {
        
    }
}