using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Projections.Prototype.Transactions;

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
    
    
    public sealed class EventMapBuilder<TProjection, TKey, TContext> : IEventMapBuilder<TProjection, TKey, TContext>
    {
        private readonly EventMapBuilder<TContext> _innerBuilder = new EventMapBuilder<TContext>();
        private ProjectorMap<TProjection, TKey, TContext> _projector;

        public EventMapBuilder<TProjection, TKey, TContext> Where(Func<object, TContext, Task<bool>> predicate)
        {
            _innerBuilder.Where(predicate);
            return this;
        }
        public ICrudAction<TEvent, TProjection, TKey, TContext> Map<TEvent>()
        {
            return new CrudAction<TEvent>(this);
        }
        public IEventMap<TContext> Build(ProjectorMap<TProjection, TKey, TContext> projector)
        {
            _projector = projector;
            
            return _innerBuilder.Build(new ProjectorMap<TContext>
            {
                Custom = (context, projectEvent) => projectEvent()
            });
        }

        private sealed class CrudAction<TEvent> : ICrudAction<TEvent, TProjection, TKey, TContext>
        {
            private readonly IAction<TEvent, TContext> _actionBuilder;
            private readonly Func<ProjectorMap<TProjection, TKey, TContext> > _getProjector;

            public CrudAction(EventMapBuilder<TProjection, TKey, TContext> parent)
            {
                _actionBuilder = parent._innerBuilder.Map<TEvent>();
                _getProjector = () => parent._projector;
            }

            public ICreateAction<TEvent, TProjection, TContext> AsCreateOf(Func<TEvent, TKey> getKey)
            {
                if (getKey == null)
                {
                    throw new ArgumentNullException(nameof(getKey));
                }

                return new CreateAction(_actionBuilder, _getProjector, getKey);
            }

            public ICreateAction<TEvent, TProjection, TContext> AsCreateIfDoesNotExistOf(
                Func<TEvent, TKey> getKey)
            {
                return AsCreateOf(getKey).IgnoringDuplicates();
            }

            public ICreateAction<TEvent, TProjection, TContext> AsCreateOrUpdateOf(Func<TEvent, TKey> getKey)
            {
                return AsCreateOf(getKey).OverwritingDuplicates();
            }

            public IDeleteAction<TEvent, TKey, TContext> AsDeleteOf(Func<TEvent, TKey> getKey)
            {
                if (getKey == null)
                {
                    throw new ArgumentNullException(nameof(getKey));
                }

                return new DeleteAction(_actionBuilder, _getProjector, getKey);
            }

            public IDeleteAction<TEvent, TKey, TContext> AsDeleteIfExistsOf(Func<TEvent, TKey> getKey)
            {
                return AsDeleteOf(getKey).IgnoringMisses();
            }

            public IUpdateAction<TEvent, TKey, TProjection, TContext> AsUpdateOf(Func<TEvent, TKey> getKey)
            {
                if (getKey == null)
                {
                    throw new ArgumentNullException(nameof(getKey));
                }

                return new UpdateAction(_actionBuilder, _getProjector, getKey);
            }

            public IUpdateAction<TEvent, TKey, TProjection, TContext> AsUpdateIfExistsOf(Func<TEvent, TKey> getKey)
            {
                return AsUpdateOf(getKey).IgnoringMisses();
            }

            public void As(Func<TEvent, TContext, Task> action)
            {
                _actionBuilder.As((anEvent, context) => _getProjector().Custom(context, () => action(anEvent, context)));
            }

            Transactions.IAction<TEvent, TContext> Transactions.IAction<TEvent, TContext>.When(Func<TEvent, TContext, Task<bool>> predicate)
            {
                return When(predicate);
            }

            public ICrudAction<TEvent, TProjection, TKey, TContext> When(
                Func<TEvent, TContext, Task<bool>> predicate)
            {
                if (predicate == null)
                {
                    throw new ArgumentNullException(nameof(predicate));
                }

                _actionBuilder.When(predicate);
                return this;
            }

            private sealed class CreateAction : ICreateAction<TEvent, TProjection, TContext>
            {
                private Func<TProjection, TEvent, TContext, bool> _shouldOverwrite;

                private readonly IAction<TEvent, TContext> _actionBuilder;
                private readonly Func<ProjectorMap<TProjection, TKey, TContext>> _projector;
                private readonly Func<TEvent, TKey> _getKey;

                public CreateAction(IAction<TEvent, TContext> actionBuilder,
                    Func<ProjectorMap<TProjection, TKey, TContext>> projector, Func<TEvent, TKey> getKey)
                {
                    _actionBuilder = actionBuilder;
                    _projector = projector;
                    _getKey = getKey;

                    _shouldOverwrite = (existingProjection, @event, context) =>
                        throw new InvalidOperationException(
                            $"Projection {typeof(TProjection)} with key {getKey(@event)}already exists.");
                }

                public ICreateAction<TEvent, TProjection, TContext> Using(Func<TProjection, TEvent, TContext, Task> projector)
                {
                    if (projector == null)
                    {
                        throw new ArgumentNullException(nameof(projector));
                    }

                    _actionBuilder.As((anEvent, context) => this._projector().Create(
                            _getKey(anEvent),
                            context,
                            projection => projector(projection, anEvent, context),
                            existingProjection => _shouldOverwrite(existingProjection, anEvent, context)));

                    return this;
                }

                public ICreateAction<TEvent, TProjection, TContext>  IgnoringDuplicates()
                {
                    _shouldOverwrite = (duplicate, @event,context) => false;
                    return this;
                }

                public ICreateAction<TEvent, TProjection, TContext> OverwritingDuplicates()
                {
                    _shouldOverwrite = (duplicate, @event,context) => true;
                    return this;
                }

                public ICreateAction<TEvent, TProjection, TContext> HandlingDuplicatesUsing(Func<TProjection, TEvent, TContext, bool> shouldOverwrite)
                {
                    _shouldOverwrite = shouldOverwrite;
                    return this;
                }
            }

            private sealed class UpdateAction : IUpdateAction<TEvent, TKey, TProjection, TContext>
            {
                private readonly IAction<TEvent, TContext> _actionBuilder;
                private readonly Func<ProjectorMap<TProjection, TKey, TContext>> _projector;
                private readonly Func<TEvent, TKey> _getKey;
                private Func<TKey, TContext, bool> _handleMissesUsing;

                public UpdateAction(
                    IAction<TEvent, TContext> actionBuilder,
                    Func<ProjectorMap<TProjection, TKey, TContext>> projector,
                    Func<TEvent, TKey> getKey)
                {
                    _projector = projector;
                    _actionBuilder = actionBuilder;
                    _getKey = getKey;

                    ThrowingIfMissing();
                }

                public IUpdateAction<TEvent, TKey, TProjection, TContext> Using(Func<TProjection, TEvent, TContext, Task> updateAction)
                {
                    if (updateAction == null)
                    {
                        throw new ArgumentNullException(nameof(updateAction));
                    }

                    _actionBuilder.As((anEvent, context) => OnUpdate(updateAction, anEvent, context));

                    return this;
                }

                private async Task OnUpdate(Func<TProjection, TEvent, TContext, Task> projector, TEvent anEvent, TContext context)
                {
                    var key = _getKey(anEvent);
                    
                    await this._projector().Update(
                        key,
                        context,
                        projection => projector(projection, anEvent, context),
                        () => _handleMissesUsing(key, context));
                }

                public IUpdateAction<TEvent, TKey, TProjection, TContext> ThrowingIfMissing()
                {
                    _handleMissesUsing = (key, ctx) => throw new InvalidOperationException($"Failed to find {typeof(TProjection).Name} with key {key}");
                    return this;
                }

                public IUpdateAction<TEvent, TKey, TProjection, TContext> IgnoringMisses()
                {
                    _handleMissesUsing = (_, __) => false;
                    return this;
                }

                public IUpdateAction<TEvent, TKey, TProjection, TContext> CreatingIfMissing()
                {
                    _handleMissesUsing = (_, __) => true;
                    return this;
                }

                public IUpdateAction<TEvent, TKey, TProjection, TContext> HandlingMissesUsing(Func<TKey, TContext, bool> action)
                {
                    _handleMissesUsing = action;
                    return this;
                }
            }

            private class DeleteAction : IDeleteAction<TEvent, TKey, TContext>
            {
                private Action<TKey, TContext> _handleMissing;

                public DeleteAction(IAction<TEvent, TContext> actionBuilder,
                    Func<ProjectorMap<TProjection, TKey, TContext>> projector, Func<TEvent, TKey> getKey)
                {
                    actionBuilder.As((anEvent, context) => OnDelete(projector(), getKey, anEvent, context));

                    ThrowingIfMissing();
                }

                private async Task OnDelete(ProjectorMap<TProjection, TKey, TContext> projector, Func<TEvent, TKey> getKey, TEvent anEvent, TContext context)
                {
                    var key = getKey(anEvent);
                    var deleted = await projector.Delete(key, context);
                    
                    if (!deleted)
                    {
                        _handleMissing(key, context);
                    }
                }

                public IDeleteAction<TEvent, TKey, TContext> ThrowingIfMissing()
                {
                    _handleMissing = (key, ctx) => throw new InvalidOperationException($"Could not delete {typeof(TProjection).Name} with key {key} because it does not exist");;
                    return this;
                }

                public IDeleteAction<TEvent, TKey, TContext> IgnoringMisses()
                {
                    _handleMissing = (_, __) => {};
                    return this;
                }

                public IDeleteAction<TEvent, TKey, TContext> HandlingMissesUsing(Action<TKey, TContext> action)
                {
                    _handleMissing = action;
                    return this;
                }
            }
        }
    }
}
