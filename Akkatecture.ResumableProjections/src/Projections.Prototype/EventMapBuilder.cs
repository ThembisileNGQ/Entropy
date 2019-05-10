using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Projections.Prototype.Transactions;

namespace Projections.Prototype
{
    public class EventMapBuilder<TProjectionContext> : IEventMapBuilder<TProjectionContext>
        where TProjectionContext : ProjectionContext, new()
    {
        private readonly EventMap<TProjectionContext> _eventMap = new EventMap<TProjectionContext>();
        private ProjectorMap<TProjectionContext> _projector;

        public EventMapBuilder<TProjectionContext> Where(Func<object, TProjectionContext, Task<bool>> filter)
        {
            _eventMap.AddFilter(filter);
            return this;
        }

        public IAction<TEvent, TProjectionContext> Map<TEvent>()
        {
            AssertNotBuilt();

            return new Action<TEvent>(this, () => _projector);
        }

        public IEventMap<TProjectionContext> Build(ProjectorMap<TProjectionContext> projector)
        {
            AssertNotBuilt();

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            if (projector.Custom == null)
            {
                throw new ArgumentException(
                    $"Expected the Custom property to point to a valid instance of {nameof(CustomHandler<TProjectionContext>)}", nameof(projector));
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

        private sealed class Action<TEvent> : IAction<TEvent, TProjectionContext>
        {
            private readonly EventMapBuilder<TProjectionContext> _parent;
            private readonly Func<ProjectorMap<TProjectionContext>> _getProjector;

            private readonly List<Func<TEvent, TProjectionContext, Task<bool>>> predicates =
                new List<Func<TEvent, TProjectionContext, Task<bool>>>();

            public Action(EventMapBuilder<TProjectionContext> parent, Func<ProjectorMap<TProjectionContext>> getProjector)
            {
                _parent = parent;
                _getProjector = getProjector;
            }

            public IAction<TEvent, TProjectionContext> When(Func<TEvent, TProjectionContext, Task<bool>> predicate)
            {
                if (predicate == null)
                {
                    throw new ArgumentNullException(nameof(predicate));
                }

                predicates.Add(predicate);
                return this;
            }

            public void As(Func<TEvent, TProjectionContext, Task> action)
            {
                if (action == null)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                Add((anEvent, context) => _getProjector().Custom(context, async () => await action(anEvent, context)));
            }

            private void Add(Func<TEvent, TProjectionContext, Task> action)
            {
                _parent._eventMap.Add<TEvent>(async (anEvent, context) =>
                {
                    foreach (Func<TEvent, TProjectionContext, Task<bool>> predicate in predicates)
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
    
    
    public sealed class EventMapBuilder<TProjection, TProjectionId, TProjectionContext> : IEventMapBuilder<TProjection, TProjectionId, TProjectionContext>
        where TProjection : class, IProjection<TProjectionId>, new()
        where TProjectionContext : ProjectionContext, new()
        where TProjectionId : IProjectionId
    {
        private readonly EventMapBuilder<TProjectionContext> _innerBuilder = new EventMapBuilder<TProjectionContext>();
        private ProjectorMap<TProjection, TProjectionId, TProjectionContext> _projector;

        public EventMapBuilder<TProjection, TProjectionId, TProjectionContext> Where(Func<object, TProjectionContext, Task<bool>> predicate)
        {
            _innerBuilder.Where(predicate);
            return this;
        }
        public ICrudAction<TEvent, TProjection, TProjectionId, TProjectionContext> Map<TEvent>()
        {
            return new CrudAction<TEvent>(this);
        }
        public IEventMap<TProjectionContext> Build(ProjectorMap<TProjection, TProjectionId, TProjectionContext> projector)
        {
            _projector = projector;
            
            return _innerBuilder.Build(new ProjectorMap<TProjectionContext>
            {
                Custom = (context, projectEvent) => projectEvent()
            });
        }

        private sealed class CrudAction<TEvent> : ICrudAction<TEvent, TProjection, TProjectionId, TProjectionContext>
        {
            private readonly IAction<TEvent, TProjectionContext> _actionBuilder;
            private readonly Func<ProjectorMap<TProjection, TProjectionId, TProjectionContext> > _getProjector;

            public CrudAction(EventMapBuilder<TProjection, TProjectionId, TProjectionContext> parent)
            {
                _actionBuilder = parent._innerBuilder.Map<TEvent>();
                _getProjector = () => parent._projector;
            }

            public ICreateAction<TEvent, TProjection, TProjectionContext> AsCreateOf(Func<TEvent, TProjectionId> getKey)
            {
                if (getKey == null)
                {
                    throw new ArgumentNullException(nameof(getKey));
                }

                return new CreateAction(_actionBuilder, _getProjector, getKey);
            }

            public ICreateAction<TEvent, TProjection, TProjectionContext> AsCreateIfDoesNotExistOf(
                Func<TEvent, TProjectionId> getKey)
            {
                return AsCreateOf(getKey).IgnoringDuplicates();
            }

            public ICreateAction<TEvent, TProjection, TProjectionContext> AsCreateOrUpdateOf(Func<TEvent, TProjectionId> getKey)
            {
                return AsCreateOf(getKey).OverwritingDuplicates();
            }

            public IDeleteAction<TEvent, TProjectionId, TProjectionContext> AsDeleteOf(Func<TEvent, TProjectionId> getKey)
            {
                if (getKey == null)
                {
                    throw new ArgumentNullException(nameof(getKey));
                }

                return new DeleteAction(_actionBuilder, _getProjector, getKey);
            }

            public IDeleteAction<TEvent, TProjectionId, TProjectionContext> AsDeleteIfExistsOf(Func<TEvent, TProjectionId> getKey)
            {
                return AsDeleteOf(getKey).IgnoringMisses();
            }

            public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> AsUpdateOf(Func<TEvent, TProjectionId> getKey)
            {
                if (getKey == null)
                {
                    throw new ArgumentNullException(nameof(getKey));
                }

                return new UpdateAction(_actionBuilder, _getProjector, getKey);
            }

            public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> AsUpdateIfExistsOf(Func<TEvent, TProjectionId> getKey)
            {
                return AsUpdateOf(getKey).IgnoringMisses();
            }

            public void As(Func<TEvent, TProjectionContext, Task> action)
            {
                _actionBuilder.As((anEvent, context) => _getProjector().Custom(context, () => action(anEvent, context)));
            }

            Transactions.IAction<TEvent, TProjectionContext> Transactions.IAction<TEvent, TProjectionContext>.When(Func<TEvent, TProjectionContext, Task<bool>> predicate)
            {
                return When(predicate);
            }

            public ICrudAction<TEvent, TProjection, TProjectionId, TProjectionContext> When(
                Func<TEvent, TProjectionContext, Task<bool>> predicate)
            {
                if (predicate == null)
                {
                    throw new ArgumentNullException(nameof(predicate));
                }

                _actionBuilder.When(predicate);
                return this;
            }

            private sealed class CreateAction : ICreateAction<TEvent, TProjection, TProjectionContext>
            {
                private Func<TProjection, TEvent, TProjectionContext, bool> _shouldOverwrite;

                private readonly IAction<TEvent, TProjectionContext> _actionBuilder;
                private readonly Func<ProjectorMap<TProjection, TProjectionId, TProjectionContext>> _projector;
                private readonly Func<TEvent, TProjectionId> _getKey;

                public CreateAction(
                    IAction<TEvent, TProjectionContext> actionBuilder,
                    Func<ProjectorMap<TProjection, TProjectionId, TProjectionContext>> projector, 
                    Func<TEvent, TProjectionId> getKey)
                {
                    _actionBuilder = actionBuilder;
                    _projector = projector;
                    _getKey = getKey;

                    _shouldOverwrite = (existingProjection, @event, context) =>
                        throw new InvalidOperationException(
                            $"Projection {typeof(TProjection)} with key {getKey(@event)}already exists.");
                }

                public ICreateAction<TEvent, TProjection, TProjectionContext> Using(Func<TProjection, TEvent, TProjectionContext, Task> projector)
                {
                    if (projector == null)
                    {
                        throw new ArgumentNullException(nameof(projector));
                    }

                    _actionBuilder.As((anEvent, context) => _projector().Create(
                            _getKey(anEvent),
                            context,
                            projection => projector(projection, anEvent, context),
                            existingProjection => _shouldOverwrite(existingProjection, anEvent, context)));

                    return this;
                }

                public ICreateAction<TEvent, TProjection, TProjectionContext>  IgnoringDuplicates()
                {
                    _shouldOverwrite = (duplicate, @event,context) => false;
                    return this;
                }

                public ICreateAction<TEvent, TProjection, TProjectionContext> OverwritingDuplicates()
                {
                    _shouldOverwrite = (duplicate, @event,context) => true;
                    return this;
                }

                public ICreateAction<TEvent, TProjection, TProjectionContext> HandlingDuplicatesUsing(Func<TProjection, TEvent, TProjectionContext, bool> shouldOverwrite)
                {
                    _shouldOverwrite = shouldOverwrite;
                    return this;
                }
            }

            private sealed class UpdateAction : IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext>
            {
                private readonly IAction<TEvent, TProjectionContext> _actionBuilder;
                private readonly Func<ProjectorMap<TProjection, TProjectionId, TProjectionContext>> _projector;
                private readonly Func<TEvent, TProjectionId> _getKey;
                private Func<TProjectionId, TProjectionContext, bool> _handleMissesUsing;

                public UpdateAction(
                    IAction<TEvent, TProjectionContext> actionBuilder,
                    Func<ProjectorMap<TProjection, TProjectionId, TProjectionContext>> projector,
                    Func<TEvent, TProjectionId> getKey)
                {
                    _projector = projector;
                    _actionBuilder = actionBuilder;
                    _getKey = getKey;

                    ThrowingIfMissing();
                }

                public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> Using(Func<TProjection, TEvent, TProjectionContext, Task> updateAction)
                {
                    if (updateAction == null)
                    {
                        throw new ArgumentNullException(nameof(updateAction));
                    }

                    
                    _actionBuilder.As((anEvent, context) => OnUpdate(updateAction, anEvent, context));

                    return this;
                }

                private async Task OnUpdate(Func<TProjection, TEvent, TProjectionContext, Task> projector, TEvent anEvent, TProjectionContext context)
                {
                    var key = _getKey(anEvent);
                    
                    await this._projector().Update(
                        key,
                        context,
                        async projection => await projector(projection, anEvent, context),
                        () => _handleMissesUsing(key, context));
                }

                public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> ThrowingIfMissing()
                {
                    _handleMissesUsing = (key, ctx) => throw new InvalidOperationException($"Failed to find {typeof(TProjection).Name} with key {key}");
                    return this;
                }

                public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> IgnoringMisses()
                {
                    _handleMissesUsing = (_, __) => false;
                    return this;
                }

                public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> CreatingIfMissing()
                {
                    _handleMissesUsing = (_, __) => true;
                    return this;
                }

                public IUpdateAction<TEvent, TProjectionId, TProjection, TProjectionContext> HandlingMissesUsing(Func<TProjectionId, TProjectionContext, bool> action)
                {
                    _handleMissesUsing = action;
                    return this;
                }
            }

            private class DeleteAction : IDeleteAction<TEvent, TProjectionId, TProjectionContext>
            {
                private Action<TProjectionId, TProjectionContext> _handleMissing;

                public DeleteAction(IAction<TEvent, TProjectionContext> actionBuilder,
                    Func<ProjectorMap<TProjection, TProjectionId, TProjectionContext>> projector, Func<TEvent, TProjectionId> getKey)
                {
                    actionBuilder.As((anEvent, context) => OnDelete(projector(), getKey, anEvent, context));

                    ThrowingIfMissing();
                }

                private async Task OnDelete(ProjectorMap<TProjection, TProjectionId, TProjectionContext> projector, Func<TEvent, TProjectionId> getKey, TEvent anEvent, TProjectionContext context)
                {
                    var key = getKey(anEvent);
                    var deleted = await projector.Delete(key, context);
                    
                    if (!deleted)
                    {
                        _handleMissing(key, context);
                    }
                }

                public IDeleteAction<TEvent, TProjectionId, TProjectionContext> ThrowingIfMissing()
                {
                    _handleMissing = (key, ctx) => throw new InvalidOperationException($"Could not delete {typeof(TProjection).Name} with key {key} because it does not exist");;
                    return this;
                }

                public IDeleteAction<TEvent, TProjectionId, TProjectionContext> IgnoringMisses()
                {
                    _handleMissing = (_, __) => {};
                    return this;
                }

                public IDeleteAction<TEvent, TProjectionId, TProjectionContext> HandlingMissesUsing(Action<TProjectionId, TProjectionContext> action)
                {
                    _handleMissing = action;
                    return this;
                }
            }
        }
    }
}
