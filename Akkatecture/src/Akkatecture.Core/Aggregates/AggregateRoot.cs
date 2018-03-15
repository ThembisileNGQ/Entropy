using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using Akka.Persistence;
using Akkatecture.Core;
using Akkatecture.Extensions;
using static LanguageExt.Prelude;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateRoot<TAggregate, TIdentity, TState> : ReceivePersistentActor, IAggregateRoot<TIdentity>
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TState>
        where TState : AggregateState<TAggregate,TIdentity, IEventApplier<TAggregate,TIdentity>>
        where TIdentity : IIdentity
    {
        private static readonly IReadOnlyDictionary<Type, Action<TAggregate, IAggregateEvent>> ApplyMethods;
        private static readonly IReadOnlyDictionary<Type, Func<TAggregate, IAggregateEvent, bool>> ApplyMethods3;
        private static readonly IReadOnlyDictionary<Type, Action<IAggregateEvent>> ApplyMethods2;
        private static readonly IAggregateName AggregateName = typeof(TAggregate).GetAggregateName();
        //private readonly List<IUncommittedEvent> _uncommittedEvents = new List<IUncommittedEvent>();
        public TState State { get; protected set; } = null;
        private CircularBuffer<ISourceId> _previousSourceIds = new CircularBuffer<ISourceId>(10);
        private ILoggingAdapter Logger { get; set; }

        public IAggregateName Name => AggregateName;
        public override string PersistenceId => Id.Value;
        public TIdentity Id { get; }
        public int Version { get; protected set; }
        public bool IsNew => Version <= 0;
        //public IEnumerable<IUncommittedEvent> UncommittedEvents => _uncommittedEvents;

        static AggregateRoot()
        {
            ApplyMethods = typeof(TAggregate).GetAggregateEventApplyMethods<TAggregate, TIdentity, TAggregate>();
            ApplyMethods2 = typeof(TAggregate).GetAggregateEventApplyMethods2<TAggregate, TIdentity>();
        }

        protected AggregateRoot(TIdentity id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if ((this as TAggregate) == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate '{GetType().PrettyPrint()}' specifies '{typeof(TAggregate).PrettyPrint()}' as generic argument, it should be its own type");
            }

            if (State == null)
            {
                throw new InvalidOperationException(
                    $"Aggregate '{GetType().PrettyPrint()}' requires '{typeof(TState).PrettyPrint()}' to be initialized");
            }

            Id = id;
            Logger = Context.GetLogger();
        }

        protected void SetSourceIdHistory(int count)
        {
            _previousSourceIds = new CircularBuffer<ISourceId>(count);
        }

        public bool HasSourceId(ISourceId sourceId)
        {
            return !sourceId.IsNone() && _previousSourceIds.Any(s => s.Value == sourceId.Value);
        }

        protected virtual void Emit<TEvent>(TEvent aggregateEvent, IMetadata metadata = null)
            where TEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            if (aggregateEvent == null)
            {
                throw new ArgumentNullException(nameof(aggregateEvent));
            }

            var aggregateSequenceNumber = Version + 1;
            var eventId = EventId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Events,
                $"{Id.Value}-v{aggregateSequenceNumber}");
            var now = DateTimeOffset.Now;
            var eventMetadata = new Metadata
            {
                Timestamp = now,
                AggregateSequenceNumber = aggregateSequenceNumber,
                AggregateName = Name.Value,
                AggregateId = Id.Value,
                EventId = eventId
            };
            eventMetadata.Add(MetadataKeys.TimestampEpoch, now.ToUnixTime().ToString());
            if (metadata != null)
            {
                eventMetadata.AddRange(metadata);
            }

            //var uncommittedEvent = new UncommittedEvent(aggregateEvent, eventMetadata);

            ApplyEvent(aggregateEvent);


            var type = typeof(TEvent);



            var m = ApplyMethods[type];

            var x = m.Bind(this as TAggregate);

            var Method = ApplyMethods2[type];


            Persist(aggregateEvent, x);
            //_uncommittedEvents.Add(uncommittedEvent);
        }

        /*public virtual async Task LoadAsync(
            IEventStore eventStore,
            ISnapshotStore snapshotStore,
            CancellationToken cancellationToken)
        {
            var domainEvents = await eventStore.LoadEventsAsync<TAggregate, TIdentity>(Id, cancellationToken).ConfigureAwait(false);

            ApplyEvents(domainEvents);
        }*/

        /*public virtual async Task<IReadOnlyCollection<IDomainEvent>> CommitAsync(
            IEventStore eventStore,
            ISnapshotStore snapshotStore,
            ISourceId sourceId,
            CancellationToken cancellationToken)
        {
            var domainEvents = await eventStore.StoreAsync<TAggregate, TIdentity>(
                Id,
                _uncommittedEvents,
                sourceId,
                cancellationToken)
                .ConfigureAwait(false);
            _uncommittedEvents.Clear();
            return domainEvents;
        }*/

        public void ApplyEvents(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            if (!domainEvents.Any())
            {
                return;
            }

            ApplyEvents(domainEvents.Select(e => e.GetAggregateEvent()));
            foreach (var domainEvent in domainEvents.Where(e => e.Metadata.ContainsKey(MetadataKeys.SourceId)))
            {
                _previousSourceIds.Put(domainEvent.Metadata.SourceId);
            }
            Version = domainEvents.Max(e => e.AggregateSequenceNumber);
        }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        public void ApplyEvents(IEnumerable<IAggregateEvent> aggregateEvents)
        {
            if (Version > 0)
            {
                throw new InvalidOperationException($"Aggregate '{GetType().PrettyPrint()}' with ID '{Id}' already has events");
            }

            foreach (var aggregateEvent in aggregateEvents)
            {
                var e = aggregateEvent as IAggregateEvent<TAggregate, TIdentity>;
                if (e == null)
                {
                    throw new ArgumentException($"Aggregate event of type '{aggregateEvent.GetType()}' does not belong with aggregate '{this}',");
                }

                ApplyEvent(e);
            }
        }

        protected virtual void ApplyEvent(IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            var eventType = aggregateEvent.GetType();
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType](aggregateEvent);
            }
            else if (_eventAppliers.Any(ea => ea.Apply((TAggregate)this, aggregateEvent)))
            {
                // Already done
            }
            else
            {
                Action<TAggregate, IAggregateEvent> applyMethod;
                if (!ApplyMethods.TryGetValue(eventType, out applyMethod))
                {
                    throw new NotImplementedException(
                        $"Aggregate '{Name}' does have an 'Apply' method that takes aggregate event '{eventType.PrettyPrint()}' as argument");
                }

                applyMethod(this as TAggregate, aggregateEvent);
            }

            Version++;
        }

        private readonly Dictionary<Type, Action<object>> _eventHandlers = new Dictionary<Type, Action<object>>();
        protected void Register<TAggregateEvent>(Action<TAggregateEvent> handler)
            where TAggregateEvent : IAggregateEvent<TAggregate, TIdentity>
        {
            var eventType = typeof(TAggregateEvent);
            if (_eventHandlers.ContainsKey(eventType))
            {
                throw new ArgumentException($"There's already a event handler registered for the aggregate event '{eventType.PrettyPrint()}'");
            }
            _eventHandlers[eventType] = e => handler((TAggregateEvent)e);
        }

        private readonly List<IEventApplier<TAggregate, TIdentity>> _eventAppliers = new List<IEventApplier<TAggregate, TIdentity>>();

        protected void Register(IEventApplier<TAggregate, TIdentity> eventApplier)
        {
            _eventAppliers.Add(eventApplier);
        }

        public override string ToString()
        {
            return $"{GetType().PrettyPrint()} v{Version}";
            //return $"{GetType().PrettyPrint()} v{Version}(-{_uncommittedEvents.Count})";
        }
    }
}
