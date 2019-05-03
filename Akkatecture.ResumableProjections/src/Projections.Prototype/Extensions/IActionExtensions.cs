using System;
using System.Threading.Tasks;
using Projections.Prototype.Transactions;

namespace Projections.Prototype.Extensions
{
    public static class IActionExtensions
    {
        public static void As<TEvent, TContext>(
            this IAction<TEvent, TContext> actionBuilder,
            Action<TEvent, TContext> action)
        {
            if (actionBuilder == null)
            {
                throw new ArgumentNullException(nameof(actionBuilder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            actionBuilder.As((anEvent, context) =>
            {
                action(anEvent, context);
                return TaskConstants.ZeroTask;
            });
        }

        public static void As<TEvent, TContext>(
            this IAction<TEvent, TContext> actionBuilder,
            Action<TEvent> action)
        {
            if (actionBuilder == null)
            {
                throw new ArgumentNullException(nameof(actionBuilder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            actionBuilder.As((anEvent, context) =>
            {
                action(anEvent);
                return TaskConstants.ZeroTask;
            });
        }

        public static void As<TEvent, TContext>(
            this IAction<TEvent, TContext> actionBuilder,
            Func<TEvent, Task> action)
        {
            if (actionBuilder == null)
            {
                throw new ArgumentNullException(nameof(actionBuilder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            actionBuilder.As((anEvent, context) => action(anEvent));
        }
        
        public static IAction<TEvent, TContext> When<TEvent, TContext>(
            this IAction<TEvent, TContext> actionBuilder,
            Func<TEvent, TContext, bool> predicate)
        {
            if (actionBuilder == null)
            {
                throw new ArgumentNullException(nameof(actionBuilder));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return actionBuilder.When((anEvent, context) => Task.FromResult(predicate(anEvent, context)));
        }

        public static IAction<TEvent, TContext> When<TEvent, TContext>(
            this IAction<TEvent, TContext> actionBuilder,
            Func<TEvent, bool> predicate)
        {
            if (actionBuilder == null)
            {
                throw new ArgumentNullException(nameof(actionBuilder));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return actionBuilder.When((anEvent, context) => Task.FromResult(predicate(anEvent)));
        }

        public static IAction<TEvent, TContext> When<TEvent, TContext>(
            this IAction<TEvent, TContext> actionBuilder,
            Func<TEvent, Task<bool>> predicate)
        {
            if (actionBuilder == null)
            {
                throw new ArgumentNullException(nameof(actionBuilder));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return actionBuilder.When((anEvent, context) => predicate(anEvent));
        }

        public static ICrudAction<TEvent, TProjection, TKey, TContext> When<TEvent, TProjection, TKey, TContext>(
            this ICrudAction<TEvent, TProjection, TKey, TContext> crudAction,
            Func<TEvent, TContext, bool> predicate)
        {
            if (crudAction == null)
            {
                throw new ArgumentNullException(nameof(crudAction));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return crudAction.When((anEvent, context) => Task.FromResult(predicate(anEvent, context)));
        }

        public static ICrudAction<TEvent, TProjection, TKey, TContext> When<TEvent, TProjection, TKey, TContext>(
            this ICrudAction<TEvent, TProjection, TKey, TContext> crudAction,
            Func<TEvent, bool> predicate)
        {
            if (crudAction == null)
            {
                throw new ArgumentNullException(nameof(crudAction));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return crudAction.When((anEvent, context) => Task.FromResult(predicate(anEvent)));
        }

        public static ICrudAction<TEvent, TProjection, TKey, TContext> When<TEvent, TProjection, TKey, TContext>(
            this ICrudAction<TEvent, TProjection, TKey, TContext> crudAction,
            Func<TEvent, Task<bool>> predicate)
        {
            if (crudAction == null)
            {
                throw new ArgumentNullException(nameof(crudAction));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return crudAction.When((anEvent, context) => predicate(anEvent));
        }
        
        public static void Using<TEvent, TProjection, TContext>(
            this ICreateAction<TEvent, TProjection, TContext> action,
            Action<TProjection, TEvent, TContext> projector)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            action.Using((projection, anEvent, context) =>
            {
                projector(projection, anEvent, context);
                return TaskConstants.FalseTask;
            });
        }
        
        public static void Using<TEvent, TProjection, TContext>(
            this ICreateAction<TEvent, TProjection, TContext> action,
            Action<TProjection, TEvent> projector)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            action.Using((projection, anEvent, context) =>
            {
                projector(projection, anEvent);
                return TaskConstants.FalseTask;
            });
        }
        
        public static void Using<TEvent, TProjection, TContext>(
            this ICreateAction<TEvent, TProjection, TContext> action,
            Func<TProjection, TEvent, Task> projector)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            action.Using((projection, anEvent, context) => projector(projection, anEvent));
        }

        public static void Using<TEvent, TProjection, TContext>(
            this IUpsertActionBuilder<TEvent, TProjection, TContext> eventActionBuilder,
            Action<TProjection, TEvent> projector)
        {
            if (eventActionBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventActionBuilder));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            eventActionBuilder.Using((projection, anEvent, context) =>
            {
                projector(projection, anEvent);
                return TaskConstants.FalseTask;
            });
        }

        
        public static void Using<TEvent, TProjection, TContext>(
            this IUpsertActionBuilder<TEvent, TProjection, TContext> eventActionBuilder,
            Func<TProjection, TEvent, Task> projector)
        {
            if (eventActionBuilder == null)
            {
                throw new ArgumentNullException(nameof(eventActionBuilder));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            eventActionBuilder.Using((projection, anEvent, context) => projector(projection, anEvent));
        }
        
        public static void Using<TEvent, TKey, TProjection, TContext>(
            this IUpdateAction<TEvent, TKey, TProjection, TContext> action,
            Action<TProjection, TEvent, TContext> projector)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            action.Using((projection, anEvent, context) =>
            {
                projector(projection, anEvent, context);
                return TaskConstants.FalseTask;
            });
        }
        
        public static void Using<TEvent, TKey, TProjection, TContext>(
            this IUpdateAction<TEvent, TKey, TProjection, TContext> action,
            Action<TProjection, TEvent> projector)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            action.Using((projection, anEvent, context) =>
            {
                projector(projection, anEvent);
                return TaskConstants.FalseTask;
            });
        }
        
        public static void Using<TEvent, TKey, TProjection, TContext>(
            this IUpdateAction<TEvent, TKey, TProjection, TContext> action,
            Func<TProjection, TEvent, Task> projector)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (projector == null)
            {
                throw new ArgumentNullException(nameof(projector));
            }

            action.Using((projection, anEvent, context) => projector(projection, anEvent));
        }
    }
}