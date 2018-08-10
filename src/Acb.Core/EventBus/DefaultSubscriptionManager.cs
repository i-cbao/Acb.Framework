using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acb.Core.EventBus
{
    public class DefaultSubscriptionManager : ISubscriptionManager
    {
        private readonly ConcurrentDictionary<string, List<Delegate>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public DefaultSubscriptionManager()
        {
            _handlers = new ConcurrentDictionary<string, List<Delegate>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>(Func<TH> handler)
            where TH : IEventHandler<T>
        {
            var key = GetEventKey<T>();
            if (!HasSubscriptionsForEvent<T>())
            {
                _handlers.TryAdd(key, new List<Delegate>());
            }
            _handlers[key].Add(handler);
            if (!_eventTypes.Contains(typeof(T)))
                _eventTypes.Add(typeof(T));
        }

        public void RemoveSubscription<T, TH>()
            where TH : IEventHandler<T>
        {
            var handlerToRemove = FindHandlerToRemove<T, TH>();
            if (handlerToRemove != null)
            {
                var key = GetEventKey<T>();
                _handlers[key].Remove(handlerToRemove);
                if (!_handlers[key].Any())
                {
                    _handlers.TryRemove(key, out _);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == key);
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                        RaiseOnEventRemoved(eventType.Name);
                    }
                }

            }
        }

        public IEnumerable<Delegate> GetHandlersForEvent<T>() where T : DEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }
        public IEnumerable<Delegate> GetHandlersForEvent(string eventName)
        {
            return _handlers.TryGetValue(eventName, out var events) ? events : new List<Delegate>();
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved?.Invoke(this, eventName);
            }
        }

        private Delegate FindHandlerToRemove<T, TH>()
            where TH : IEventHandler<T>
        {
            if (!HasSubscriptionsForEvent<T>())
            {
                return null;
            }

            var key = GetEventKey<T>();
            foreach (var func in _handlers[key])
            {
                var genericArgs = func.GetType().GetGenericArguments();
                if (genericArgs.SingleOrDefault() == typeof(TH))
                {
                    return func;
                }
            }

            return null;
        }

        public bool HasSubscriptionsForEvent<T>()
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }
        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => _eventTypes.Single(t => GetEventKey(t) == eventName);

        private static string GetEventKey<T>()
        {
            return GetEventKey(typeof(T));
        }

        private static string GetEventKey(MemberInfo type)
        {
            var attr = type.GetCustomAttribute<RouteKeyAttribute>();
            return attr == null ? type.Name : attr.Key;
        }

    }
}
