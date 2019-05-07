using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Akka.Persistence.Query;

namespace Projections.Prototype
{
    public class InnerProjector
    {
        private readonly IEventMap<ProjectionContext> _map;
        private readonly IReadOnlyList<InnerProjector> _children;
        private ShouldRetry _shouldRetry = (exception, count) => TaskConstants.FalseTask;

        public InnerProjector(IEventMapBuilder<ProjectionContext> eventMapBuilder, IEnumerable<InnerProjector> children = null)
            : this(BuildMap(eventMapBuilder), children)
        {
            
        }

        private static IEventMap<ProjectionContext> BuildMap(IEventMapBuilder<ProjectionContext> eventMapBuilder)
        {
            if (eventMapBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventMapBuilder));
            }

            return eventMapBuilder.Build(new ProjectorMap<ProjectionContext>
            {
                Custom = (context, projector) => projector()
            });
        }

        public InnerProjector(IEventMap<ProjectionContext> map, IEnumerable<InnerProjector> children = null)
        {
            _map = map;

            _children = children?.ToList() ?? new List<InnerProjector>();
            if (_children.Contains(null))
            {
                throw new ArgumentException("There is null child projector.", nameof(children));
            }
        }

        public ShouldRetry ShouldRetry
        {
            get { return _shouldRetry; }
            set
            {
                _shouldRetry = value ?? throw new ArgumentNullException(nameof(value), "Retry policy is missing.");
            }
        }

        public async Task Handle(IReadOnlyList<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                await ExecuteWithRetry(() => ProjectTransaction(transaction)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteWithRetry(Func<Task> action)
        {
            for (var attempt = 1;;attempt++)
            {
                try
                {
                    await action();
                    break;
                }
                catch (ProjectionException exception)
                {
                    if (!await ShouldRetry(exception, attempt))
                    {
                        throw;
                    }
                }
            }
        }

        private async Task ProjectTransaction(Transaction transaction)
        {
            foreach (EventEnvelope eventEnvelope in transaction.Events)
            {
                try
                {
                    await ProjectEvent(
                        eventEnvelope.Event,
                        new ProjectionContext
                        {
                            TransactionId = transaction.Id,
                            StreamId = transaction.StreamId,
                            TimeStamp = transaction.Timestamp,
                            Checkpoint = transaction.Checkpoint,
                            TransactionHeaders = transaction.Headers
                        });
                }
                catch (ProjectionException projectionException)
                {
                    projectionException.CurrentEvent = eventEnvelope;
                    projectionException.TransactionId = transaction.Id;
                    projectionException.SetTransactionBatch(new[] { transaction });
                    throw;
                }
                catch (Exception exception)
                {
                    var projectionException = new ProjectionException("Projector failed to project an event.", exception)
                    {
                        CurrentEvent = eventEnvelope,
                        TransactionId = transaction.Id
                    };

                    projectionException.SetTransactionBatch(new[] { transaction });
                    throw projectionException;
                }
            }
        }

        private async Task ProjectEvent(object anEvent, ProjectionContext context)
        {
            foreach (var child in _children)
            {
                await child.ProjectEvent(anEvent, context);
            }

            // There is no way to identify the child projector when an exception happens so we don't handle exceptions here.
            await _map.Handle(anEvent, context);
        }
    }
    
    public delegate Task<bool> ShouldRetry(ProjectionException exception, int attempts);
}