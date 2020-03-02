﻿using CodeMonkeys.Core;
using CodeMonkeys.Core.Messaging;
using CodeMonkeys.Messaging.Configuration;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("UnitTests")]
namespace CodeMonkeys.Messaging
{
    public sealed class EventAggregator : IEventAggregator, IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly ConcurrentDictionary<Type, IList<Type>> _eventTypeCache;
        private readonly EventTypeCache _cache;

        private readonly ISubscriptionManager _subscriptionManager;

        public EventAggregator()
        {
            _cts = new CancellationTokenSource();
            _eventTypeCache = new ConcurrentDictionary<Type, IList<Type>>();
            _cache = new EventTypeCache();
        }

        public EventAggregator(SubscriptionManagerOptions options = null)
            
            : this()
        {
            _subscriptionManager = new SubscriptionManager(
                _cts.Token,
                options);
        }

        public EventAggregator(
            IEnumerable<ISubscriber> subscribers,
            SubscriptionManagerOptions options = null)
            
            : this(options)
        {
            Argument.NotNull(
                subscribers,
                nameof(subscribers));

            Register(subscribers);                
        }

        /// <inheritdoc/>
        public void Publish<TEvent>(TEvent @event)
            where TEvent : class, IEvent
        {
            Argument.NotNull(
                @event,
                nameof(@event));

            PublishAsync(@event);
        }

        /// <inheritdoc/>
        public async Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : class, IEvent
        {
            Argument.NotNull(
                @event,
                nameof(@event));

            var subscribers = _subscriptionManager.GetSubscribersOf<TEvent>();

            foreach (var subscriber in subscribers)
                await subscriber.ReceiveEventAsync(@event);
        }

        /// <inheritdoc/>
        public void RegisterTo<TEvent>(ISubscriberOf<TEvent> subscriber)
            where TEvent : class, IEvent
        {
            Argument.NotNull(
                subscriber,
                nameof(subscriber));

            _subscriptionManager.Add(typeof(TEvent), subscriber);
        }

        /// <inheritdoc/>
        public void Register(ISubscriber subscriber)
        {
            Argument.NotNull(
                subscriber,
                nameof(subscriber));

            foreach (var type in _cache.GetOrAddEventTypesOf(subscriber))
                _subscriptionManager.Add(type, subscriber);
        }        

        /// <inheritdoc/>
        public void DeregisterFrom<TEvent>(ISubscriberOf<TEvent> subscriber)
            where TEvent : class, IEvent
        {
            Argument.NotNull(
                subscriber,
                nameof(subscriber));

            _subscriptionManager.Remove(typeof(TEvent), subscriber);
        }

        /// <inheritdoc/>
        public void Deregister(ISubscriber subscriber)
        {
            Argument.NotNull(
                subscriber,
                nameof(subscriber));

            foreach (var type in _cache.GetOrAddEventTypesOf(subscriber))
                _subscriptionManager.Remove(type, subscriber);
        }

        private void Register(IEnumerable<ISubscriber> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                try
                {
                    Register(subscriber);
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
        }

        ~EventAggregator()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
