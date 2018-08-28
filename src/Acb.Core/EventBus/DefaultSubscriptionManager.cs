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
        private readonly ConcurrentDictionary<string, Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public DefaultSubscriptionManager()
        {
            _handlers = new ConcurrentDictionary<string, List<Delegate>>();
            _eventTypes = new ConcurrentDictionary<string, Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>(Func<TH> handler, string eventKey = null)
            where TH : IEventHandler<T>
        {
            var key = string.IsNullOrWhiteSpace(eventKey) ? GetEventKey<T>() : eventKey;
            if (!HasSubscriptionsForEvent(key))
            {
                _handlers.TryAdd(key, new List<Delegate>());
            }

            _handlers[key].Add(handler);
            if (!_eventTypes.ContainsKey(key))
                _eventTypes.TryAdd(key, typeof(T));
        }

        public void RemoveSubscription<T, TH>(string eventKey = null)
            where TH : IEventHandler<T>
        {
            var key = string.IsNullOrWhiteSpace(eventKey) ? GetEventKey<T>() : eventKey;
            var handlerToRemove = FindHandlerToRemove<T, TH>(key);
            if (handlerToRemove != null)
            {
                _handlers[key].Remove(handlerToRemove);
                if (!_handlers[key].Any())
                {
                    _handlers.TryRemove(key, out _);
                    _eventTypes.TryGetValue(key, out var eventType);
                    if (eventType != null)
                    {
                        _eventTypes.TryRemove(key, out _);
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
        public IEnumerable<Delegate> GetHandlersForEvent(string eventKey)
        {
            return _handlers.TryGetValue(eventKey, out var events) ? events : new List<Delegate>();
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved?.Invoke(this, eventName);
            }
        }

        private Delegate FindHandlerToRemove<T, TH>(string eventKey = null)
            where TH : IEventHandler<T>
        {
            var key = string.IsNullOrWhiteSpace(eventKey) ? GetEventKey<T>() : eventKey;
            if (!HasSubscriptionsForEvent(key))
            {
                return null;
            }

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
        public bool HasSubscriptionsForEvent(string eventKey) => _handlers.ContainsKey(eventKey);

        public Type GetEventTypeByName(string eventKey)
        {
            _eventTypes.TryGetValue(eventKey, out var type);
            return type;
        }

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
