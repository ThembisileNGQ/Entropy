using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projections.Prototype
{
    public class EventMap<TContext> : IEventMap<TContext>
    {
        private readonly Dictionary<Type, List<Handler>> _mappings = new Dictionary<Type, List<Handler>>();
        private readonly List<Func<object, TContext, Task<bool>>> _filters = new List<Func<object, TContext, Task<bool>>>();

        internal void Add<TEvent>(Func<TEvent, TContext, Task> action)
        {
            if (!_mappings.ContainsKey(typeof(TEvent)))
            {
                _mappings[typeof(TEvent)] = new List<Handler>();
            }

            _mappings[typeof(TEvent)].Add((@event, context) => action((TEvent)@event, context));
        }

        internal void AddFilter(Func<object, TContext, Task<bool>> filter)
        {
            _filters.Add(filter);
        }

        public async Task<bool> Handle(object anEvent, TContext context)
        {
            if (anEvent == null)
            {
                throw new ArgumentNullException(nameof(anEvent));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (await PassesFilter(anEvent, context))
            {
                var eventType = anEvent.GetType();

                var handlers = GetHandlersForType(eventType);

                if (handlers.Any())
                {
                    foreach (var handler in handlers)
                    {
                        await handler(anEvent, context);
                    }

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> PassesFilter(object anEvent, TContext context)
        {
            if (_filters.Count > 0)
            {
                var results = await Task.WhenAll(_filters.Select(filter => filter(anEvent, context)));

                return results.All(x => x);
            }
            else
            {
                return true;
            }
        }

        private List<Handler> GetHandlersForType(Type eventType)
        {
            var handlers = new List<Handler>();
            var baseType = _mappings.Keys.FirstOrDefault(key => eventType.GetTypeInfo().IsSubclassOf(key));
            if (baseType != null)
            {
                handlers.AddRange(_mappings[baseType]);
            }

            if (_mappings.TryGetValue(eventType, out var concreteTypeHandlers))
            {
                handlers.AddRange(concreteTypeHandlers);
            }

            return handlers;
        }

        private delegate Task Handler(object @event, TContext context);
    }
}
