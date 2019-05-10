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
        public ProjectorMap<TProjection, TProjectionId, TProjectionContext> ProjectorMap { get; set; }
        public EventMapBuilder<TProjection, TProjectionId, TProjectionContext> EventMapBuilder { get; set; } 
        public IEventMap<TProjectionContext> EventMap { get; set; }
        
        public ProjectorBuilder(
            TJournal journal,
            ProjectorMap<TProjection,TProjectionId,TProjectionContext> projectorMap,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMapBuilder,
            string name,
            ActorSystem actorSystem) 
            : base(journal, name, actorSystem)
        {
            ProjectorMap = projectorMap;
            EventMapBuilder = eventMapBuilder;
        }

        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithEventMapBuilder(
            EventMapBuilder<TProjection, TProjectionId, TProjectionContext> eventMap)
        {
            
            EventMapBuilder = eventMap;
            return this;
        }
        
        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithProjectorBuilder(
            ProjectorMap<TProjection, TProjectionId, TProjectionContext> projectorMap)
        {
            ProjectorMap = projectorMap;
            return this;
        }

        public void RunAggregateProjection(IActorRef repository)
        {
            EventMap = EventMapBuilder.Build(ProjectorMap);
            var projector = ActorSystem.ActorOf(Props.Create( () => new ProjectorManager<TJournal, TProjectionContext, TProjection, TProjectionId>(repository,EventMap,Journal)),"PersistentIdStream");
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
        
        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithEventMapBuilder<TProjection, TProjectionId, TProjectionContext>(
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
        
        public ProjectorBuilder<TJournal, TProjection, TProjectionId, TProjectionContext> WithProjectorBuilder<TProjection, TProjectionId, TProjectionContext>(
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
        public ActorSystem ActorSystem { get; set; }
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