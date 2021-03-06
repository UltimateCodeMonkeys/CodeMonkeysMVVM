﻿using System;
using System.Collections.Generic;

namespace CodeMonkeys.Messaging
{
    public sealed partial class EventAggregator
    {
        private static readonly HashSet<Subscription> _subscriptions = 
            new HashSet<Subscription>();

        private void AddSubscriber(
            Type eventType,
            ISubscriber subscriber)
        {
            _subscriptions.Add(
                Subscription.Create(
                    eventType,
                    subscriber));
        }

        private void RemoveSubscriber(
            Type eventType,
            ISubscriber subscriber)
        {
            var subscriptionToRemove = _subscriptions
                .RemoveWhere(
                    s => s.EventType.Equals(eventType) &&
                    s.Reference.Target?.Equals(subscriber) == true);
        }
    }
}
