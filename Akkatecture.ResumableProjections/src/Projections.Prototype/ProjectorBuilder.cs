using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Streams;
using Projections.Prototype.Repository;

namespace Projections.Prototype
{
    public class ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> : ProjectorBuilder<TJournal>
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
        public IActorRef RepositoryActor { get; }
        public ProjectorMap<TProjection, TProjectionId, TProjectionContext> ProjectorMap { get; set; }
        public EventMapBuilder<TProjection, TProjectionId, TProjectionContext> EventMap { get; set; } 
        
        public ProjectorBuilder(
            TJournal journal,
            ProjectorMap<TProjection,TProjectionId,TProjectionContext> projectorMap,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMap,
            string name,
            ActorSystem actorSystem) 
            : base(journal, name, actorSystem)
        {
            ProjectorMap = projectorMap;
            EventMap = eventMap;
            RepositoryActor =
                actorSystem.ActorOf(Props.Create(() => new RepositoryActor<TProjection, TProjectionId>()),"repository");
            var projectorName = typeof(TProjection).Name;
            //Props.Create(() => new Projector<TJournal,TProjection,TProjectionId,TProjectionContext>());
        }

        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithEventMap(
            EventMapBuilder<TProjection, TProjectionId, TProjectionContext> eventMap)
        {
            
            EventMap = eventMap;
            return this;
        }
        
        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithProjectorMap(
            ProjectorMap<TProjection, TProjectionId, TProjectionContext> projectorMap)
        {
            ProjectorMap = projectorMap;
            return this;
        }

        public void RunAggregateProjection()
        {
            
            var projector = ActorSystem.ActorOf(Props.Create( () => new ProjectorManager<TJournal, TProjectionContext, TProjection, TProjectionId>(RepositoryActor,EventMap,Journal)),"PersistentIdStream");
            projector.Tell(new BeginProjection());
        }
        
        public void RunTaggedProjection()
        {
            
        }
    }
    
    
    
    public class ProjectorBuilder<TJournal> : ProjectorBuilder
        where TJournal : IReadJournal,
        IPersistenceIdsQuery,
        ICurrentPersistenceIdsQuery,
        IEventsByPersistenceIdQuery,
        ICurrentEventsByPersistenceIdQuery,
        IEventsByTagQuery,
        ICurrentEventsByTagQuery
    {
        protected TJournal Journal { get; }
        
        public ProjectorBuilder(
            TJournal journal,
            string name,
            ActorSystem actorSystem) 
            : base(name,actorSystem)
        {
            Journal = journal;
        }
        
        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithEventMap<TProjection, TProjectionId, TProjectionContext>(
            EventMapBuilder<TProjection, TProjectionId, TProjectionContext> eventMap)
            where TProjection : class, IProjection<TProjectionId>, new()
            where TProjectionContext : ProjectionContext, new()
            where TProjectionId : IProjectionId
        {
            
            return new ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext>(
                Journal,
                null,
                eventMap,
                Name,
                ActorSystem);
        }
        
        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithProjectorMap<TProjection, TProjectionId, TProjectionContext>(
            ProjectorMap<TProjection, TProjectionId, TProjectionContext> projectorMap)
            where TProjection : class, IProjection<TProjectionId>, new()
            where TProjectionContext : ProjectionContext, new()
            where TProjectionId : IProjectionId
        {
            return new ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext>(
                Journal,
                projectorMap,
                null,
                Name,
                ActorSystem);
        } 
    }
    public class ProjectorBuilder
    {
        protected ActorSystem ActorSystem { get; set; }
        protected string Name { get; set; }
        internal ProjectorBuilder(
            string name,
            ActorSystem actorSystem)
        {
            ActorSystem = actorSystem;
            Name = name;
        }

        private ProjectorBuilder(
            string name,
            Config config)
        {
            var actorSystem = ActorSystem.Create(name, config);
            ActorSystem = actorSystem;
            Name = name;
        }

        public static ProjectorBuilder Create(string name, Config config)
        {
            return new ProjectorBuilder(name,config);
        }

        public ProjectorBuilder<TJournal> Using<TJournal>(
            string readJournalPluginId = null)
            where TJournal : IReadJournal,
            IPersistenceIdsQuery,
            ICurrentPersistenceIdsQuery,
            IEventsByPersistenceIdQuery,
            ICurrentEventsByPersistenceIdQuery,
            IEventsByTagQuery,
            ICurrentEventsByTagQuery
        {
            var readJournal = PersistenceQuery
                .Get(ActorSystem)
                .ReadJournalFor<TJournal>(readJournalPluginId);
            
            return new ProjectorBuilder<TJournal>(readJournal,Name, ActorSystem);
        }
    }
    
}