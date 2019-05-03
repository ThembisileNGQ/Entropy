using Akka.Actor;
using Akka.Persistence.Query;
using Akkatecture.Aggregates;

namespace Projections.Prototype
{
    public abstract class Projector<TJournal, TIdentity> : ReceiveActor
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

        public virtual bool ShouldProject(IDomainEvent domainEvent)
        {
            return ProjectionLocator.LocateProjector(domainEvent) != null;
        }

        protected Projector(IProjectionLocator<TIdentity> projectionLocator)
        {
            ProjectionLocator = projectionLocator;
            //this probably needs to be sent the stream reference so that this actor can sink the stream and start processing the messages
            // from the journal, the wierd thing here is the stream can be either from tags or from actual persistence ids (aggregate or saga Ids)
            Receive<BeginProjectorStream>(Handle);
        }

        protected bool Handle(BeginProjectorStream command)
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

    public interface ISetupProjectorSchema
    {
        
    }
}