﻿using CodeMonkeys.Core.Messaging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CodeMonkeys.Messaging
{
    internal class EventTypeCache
    {
        private readonly ConcurrentDictionary<Type, IList<Type>> _this;

        internal IList<Type> GetOrAddEventTypesOf(ISubscriber subscriber)
        {
            var type = subscriber.GetType();

            if (_this.TryGetValue(type, out var eventTypes))
                return eventTypes;

            var interfaces = GetTypesWhichImplementSubscriberOfInterface(type);
            eventTypes = GetGenericTypeArguments(interfaces);

            _this.TryAdd(type, eventTypes);

            return eventTypes;
        }

        private IEnumerable<Type> GetTypesWhichImplementSubscriberOfInterface(Type type)
        {
            return type
                .GetInterfaces()
                .Where(@interface =>
                    @interface.IsGenericType &&
                    @interface.GetGenericTypeDefinition() == typeof(ISubscriberOf<>));
        }

        private IList<Type> GetGenericTypeArguments(IEnumerable<Type> interfaces)
        {
            return interfaces
                .Select(@interface => @interface.GenericTypeArguments.FirstOrDefault())
                .ToList();
        }
    }
}
