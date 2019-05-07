using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Streams;

namespace Projections.Prototype
{
    public class ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext> : ProjectorManager<TJournal>
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
        public Props ProjectorProps { get; }
        public ProjectorMap<TProjection, TProjectionId, TProjectionContext> ProjectorMap { get; set; }
        public EventMapBuilder<TProjection, TProjectionId, TProjectionContext> EventMap { get; set; } 
        
        public ProjectorManager(
            TJournal journal,
            ProjectorMap<TProjection,TProjectionId,TProjectionContext> projectorMap,
            EventMapBuilder<TProjection,TProjectionId,TProjectionContext> eventMap,
            ActorSystem actorSystem) 
            : base(journal, actorSystem)
        {
            ProjectorMap = projectorMap;
            EventMap = eventMap;

            var projectorName = typeof(TProjection).Name;
            //Props.Create(() => new Projector<TJournal,TProjection,TProjectionId,TProjectionContext>());
        }

        public ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext> WithEventMap(
            EventMapBuilder<TProjection, TProjectionId, TProjectionContext> eventMap)
        {
            EventMap = eventMap;
            return this;
        }
        
        public ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext> WithProjectorMap(
            ProjectorMap<TProjection, TProjectionId, TProjectionContext> projectorMap)
        {
            ProjectorMap = projectorMap;
            return this;
        }

        public void RunAggregateProjection()
        {
            var projector = _actorSystem.ActorOf(Props.Create<PersistentIdStream<TJournal, TProjectionId>>(_journal),"PersistentIdStream");
            projector.Tell(new BeginPersistentIdStream());
        }
        
        public void RunTaggedProjection()
        {
            
        }
    }
    
    
    
    public class ProjectorManager<TJournal> : ProjectorManager
        where TJournal : IReadJournal,
        IPersistenceIdsQuery,
        ICurrentPersistenceIdsQuery,
        IEventsByPersistenceIdQuery,
        ICurrentEventsByPersistenceIdQuery,
        IEventsByTagQuery,
        ICurrentEventsByTagQuery
    {
        protected TJournal _journal { get; set; }
        
        public ProjectorManager(
            TJournal journal,
            ActorSystem actorSystem) 
            : base(actorSystem)
        {
            _journal = journal;
        }
        
        public ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext> WithEventMap<TProjection, TProjectionId, TProjectionContext>(
            EventMapBuilder<TProjection, TProjectionId, TProjectionContext> eventMap)
            where TProjection : class, IProjection<TProjectionId>, new()
            where TProjectionContext : ProjectionContext
            where TProjectionId : IProjectionId
        {
            
            return new ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext>(
                _journal,
                null,
                eventMap,
                _actorSystem);
        }
        
        public ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext> WithProjectorMap<TProjection, TProjectionId, TProjectionContext>(
            ProjectorMap<TProjection, TProjectionId, TProjectionContext> projectorMap)
            where TProjection : class, IProjection<TProjectionId>, new()
            where TProjectionContext : ProjectionContext
            where TProjectionId : IProjectionId
        {
            return new ProjectorManager<TJournal, TProjection, TProjectionId, TProjectionContext>(
                _journal,
                projectorMap,
                null,
                _actorSystem);
        } 
    }
    public class ProjectorManager
    {
        protected ActorSystem _actorSystem { get; set; }
        internal ProjectorManager(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        private ProjectorManager(
            string name,
            Config config)
        {
            var actorSystem = ActorSystem.Create(name, config);
            _actorSystem = actorSystem;
        }

        public static ProjectorManager Create(string name, Config config)
        {
            return new ProjectorManager(name,config);
        }

        public ProjectorManager<TJournal> Using<TJournal>(
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
                .Get(_actorSystem)
                .ReadJournalFor<TJournal>(readJournalPluginId);
            
            return new ProjectorManager<TJournal>(readJournal, _actorSystem);
        }
        /*ProjectorManager
            * .Create(config)
            * .Using<SqlReadJournal>()
            * .WithProjection<TProjectionContext, TProjection, TProjectionIdentity>()
            * .WithProjectionMap<TProjectionContext, TProjection, TProjectionIdentity()
            * .WithEventMap<TProjectionContext>()
            * .Project()*/
        
    }
    
}