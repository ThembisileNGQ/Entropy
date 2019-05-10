using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace Projections.Prototype
{
    public class EventDispatcher<TProjectionContext> : ReceiveActor
        where TProjectionContext : ProjectionContext, new()
    {
        private IEventMap<TProjectionContext> _eventMap { get; }
        private Source<EventEnvelope, NotUsed> _eventStream { get; }
        public EventDispatcher(
            IEventMap<TProjectionContext> eventMap,
            Source<EventEnvelope, NotUsed> eventStream)
        {
            Receive<BeginInnerProjection>(Handle);
        }

        public bool Handle(BeginInnerProjection command)
        {
            var mat = ActorMaterializer.Create(Context);
            var context = Context;

            _eventStream
                .RunForeach(x => Handle(context, x), mat);
            
            return true;
        }

        public void Handle(
            IUntypedActorContext context,
            EventEnvelope persistedEvent)
        {
            context.GetLogger().Info(persistedEvent.Offset.ToString());
        }
        
    }


    public class BeginInnerProjection
    {
        
    }
    
    
}