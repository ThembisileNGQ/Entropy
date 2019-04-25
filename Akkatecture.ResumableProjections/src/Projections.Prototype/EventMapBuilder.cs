using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projections.Prototype
{
    public class EventMapBuilder<TContext> : IEventMapBuilder<TContext>
    {
        private readonly EventMap<TContext> _eventMap = new EventMap<TContext>();
        private ProjectorMap<TContext> _projector;

        public EventMapBuilder<TContext> Where(Func<object, TContext, Task<bool>> filter)
        {
            _eventMap.AddFilter(filter);
            return this;
        }

        public IAction<TEvent, TContext> Map<TEvent>()
        {
            AssertNotBuilt();

            return new Action<TEvent>(this, () => _projector);
        }

        public IEventMap<TContext> Build(ProjectorMap<TContext> projector)
        {
            AssertNotBuilt();

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            if (projector.Custom == null)
            {
                throw new ArgumentException(
                    $"Expected the Custom property to point to a valid instance of {nameof(CustomHandler<TContext>)}", nameof(projector));
            }

            this._projector = projector;

            return _eventMap;
        }

        private void AssertNotBuilt()
        {
            if (_projector != null)
            {
                throw new InvalidOperationException("The event map has already been built.");
            }
        }

        private sealed class Action<TEvent> : IAction<TEvent, TContext>
        {
            private readonly EventMapBuilder<TContext> _parent;
            private readonly Func<ProjectorMap<TContext>> _getProjector;

            private readonly List<Func<TEvent, TContext, Task<bool>>> predicates =
                new List<Func<TEvent, TContext, Task<bool>>>();

            public Action(EventMapBuilder<TContext> parent, Func<ProjectorMap<TContext>> getProjector)
            {
                _parent = parent;
                _getProjector = getProjector;
            }

            public IAction<TEvent, TContext> When(Func<TEvent, TContext, Task<bool>> predicate)
            {
                if (predicate == null)
                {
                    throw new ArgumentNullException(nameof(predicate));
                }

                predicates.Add(predicate);
                return this;
            }

            public void As(Func<TEvent, TContext, Task> action)
            {
                if (action == null)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                Add((anEvent, context) => _getProjector().Custom(context, async () => await action(anEvent, context)));
            }

            private void Add(Func<TEvent, TContext, Task> action)
            {
                _parent._eventMap.Add<TEvent>(async (anEvent, context) =>
                {
                    foreach (Func<TEvent, TContext, Task<bool>> predicate in predicates)
                    {
                        if (!await predicate(anEvent, context))
                        {
                            return;
                        }
                    }

                    await action(anEvent, context);
                });
            }
        }
    }
}
