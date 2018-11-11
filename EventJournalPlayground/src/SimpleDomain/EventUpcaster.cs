using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Akka.Persistence.Journal;
using Akka.Util.Internal;
using Akkatecture.Aggregates;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.Subscribers;
using SimpleDomain.Model.UserAccount;
using SimpleDomain.Model.UserAccount.Events;

namespace SimpleDomain
{

    public class UserAccountAggregateEventUpcaster : AggregateEventUpcaster<UserAccountAggregate, UserAccountId>,
        IUpcast<UserAccountNameChangedEventV2, UserAccountNameChangedEvent>
    {
        public UserAccountNameChangedEventV2 Upcast(UserAccountNameChangedEvent aggregateEvent)
        {
            return new UserAccountNameChangedEventV2(aggregateEvent.Name + "the operation");
        }
    }
    
    public interface IEventUpcaster<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        IAggregateEvent<TAggregate,TIdentity> Upcast(IAggregateEvent<TAggregate, TIdentity> aggregateEvent);
    }
    
    public interface IUpcast<out TNewerAggregateEvent, in TOlderAggregateEvent>
    where TOlderAggregateEvent : IAggregateEvent
    where TNewerAggregateEvent : IAggregateEvent
    {
        TNewerAggregateEvent Upcast(TOlderAggregateEvent aggregateEvent);
    }
    
    
    public abstract class AggregateEventUpcaster<TAggregate, TIdentity> : AggregateEventUpcaster<TAggregate, TIdentity,
        IEventUpcaster<TAggregate, TIdentity>>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        
    }
    
    public abstract class AggregateEventUpcaster<TAggregate,TIdentity, TEventUpcaster> : IReadEventAdapter, IEventUpcaster<TAggregate, TIdentity>
        where TEventUpcaster : class, IEventUpcaster<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        private static ConcurrentDictionary<Type, bool> DecisionCache = new ConcurrentDictionary<Type, bool>();

        public static readonly IReadOnlyDictionary<Type, Func<TEventUpcaster, IAggregateEvent, IAggregateEvent>> UpcastFunctions;

        static AggregateEventUpcaster()
        {
            UpcastFunctions  = typeof(TEventUpcaster).GetAggregateEventUpcastMethods<TAggregate, TIdentity, TEventUpcaster>();
        }
        
        public AggregateEventUpcaster()
        {
            var upcastables = GetAggregateEventUpcastTypes(GetType());
            var dictionary = upcastables.ToDictionary(x => x, x=> true);
            DecisionCache = new ConcurrentDictionary<Type, bool>(dictionary);
            
            var me = this as TEventUpcaster;
            if (me == null)
            {
                throw new InvalidOperationException(
                    $"Event applier of type '{GetType().PrettyPrint()}' has a wrong generic argument '{typeof(TEventUpcaster).PrettyPrint()}'");
            }
            
            var oldevt = new UserAccountNameChangedEvent("test");
          
        }
        
        public bool ShouldUpcast(object potentialUpcast)
        {
            var type = potentialUpcast.GetType();
            
            if (potentialUpcast is ICommittedEvent<TAggregate,TIdentity> comittedEvent)
            {
                var eventType = type.GetType().GenericTypeArguments[2];

                if (DecisionCache.ContainsKey(eventType))
                {
                    
                    return true;
                }
                else
                {
                    DecisionCache.AddOrSet(eventType, false);
                    return false;
                }
                
            }

            DecisionCache.AddOrSet(type, false);
            return false;
        }
        
        //move to new ting man
        internal static IReadOnlyList<Type> GetAggregateEventUpcastTypes(Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            
            var upcastableEventTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUpcast<,>))
                .Select(i =>   i.GetGenericArguments()[1])
                .ToList();

            return upcastableEventTypes;
        }

        public IEventSequence FromJournal(object evt, string manifest)
        {
            if (ShouldUpcast(evt))
            {
                //dynamic dispach here any better options
                var comittedEvent = evt as dynamic;
                    
                var upcastedEvent = Upcast(comittedEvent.AggregateEvent);
            }

            return EventSequence.Single(evt);
        }
        
        
        
        public IAggregateEvent<TAggregate, TIdentity> Upcast(
            IAggregateEvent<TAggregate, TIdentity> aggregateEvent)
        {
            var aggregateEventType = aggregateEvent.GetType();
            Func<TEventUpcaster,IAggregateEvent, IAggregateEvent> upcaster;

            if (!UpcastFunctions.TryGetValue(aggregateEventType, out upcaster))
            {
                throw new ArgumentException();
            }

            var evt = upcaster((TEventUpcaster)(object)this, aggregateEvent) as IAggregateEvent<TAggregate, TIdentity>;

            return evt;
        }
    }


    public static class HelperExttensions
    {
        internal static IReadOnlyDictionary<Type, Func<T,IAggregateEvent, IAggregateEvent>> GetAggregateEventUpcastMethods<TAggregate, TIdentity, T>(this Type type)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateEventType = typeof(IAggregateEvent<TAggregate, TIdentity>);
            //throw new NotImplementedException();
            foreach (var thing in type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if(mi.Name != "Upcast") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);  
                }))
            {
                Console.WriteLine(thing);
            }
            
            return type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Upcast") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
                })
                .ToDictionary(
                    //problem might be here
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => ReflectionHelper.CompileMethodInvocation<Func<T,IAggregateEvent, IAggregateEvent>>(type, "Upcast", mi.GetParameters()[0].ParameterType));
                    
        }
    }
    
    
}