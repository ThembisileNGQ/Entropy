using Akka.Actor;
using Akka.Event;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;

namespace ReadApi
{
    public class CarProjector : ReceiveActor
    {
        private static string _schema = "projection";
        private static string _table = "cars";
        private readonly string _version;
        private readonly ILoggingAdapter _logger;

        public CarProjector(ProjectorSettings settings)
        {
            _logger = Context.GetLogger();
            _version = settings.Version;
            Receive<BeginStreaming>(Handle);
        }

        public bool Handle(BeginStreaming command)
        {
            var mat = ActorMaterializer.Create(Context);

            var readJournal =
                PersistenceQuery.Get(Context.System)
                    .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);

            readJournal
                .EventsByTag("CarAggregate", NoOffset.Instance)
                .RunForeach(e => Handle(Context,e), mat);
            return true;
        }

        public void Handle(
            IUntypedActorContext context,
            EventEnvelope eventEnvelope)
        {
            _logger.Info("Event:" + eventEnvelope.Event);
        }
    }

    public class BeginStreaming
    {
        public static BeginStreaming Instance => new BeginStreaming();
    }
    public class ProjectorSettings
    {
        public string Version { get; }

        public ProjectorSettings(
            string version)
        {
            Version = version;
        }
    }
    
}
