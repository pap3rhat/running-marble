using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SignalBus
{
    private static Dictionary<Type, List<object>> _subscribers = new();

    public static void Subscribe<T>(ISubscriber<T> sub) where T : ISignal
    {
        if (!_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers.Add(typeof(T), new());
        }

        _subscribers[typeof(T)].Add(sub);
    }

    public static void Unsubscribe<T>(ISubscriber<T> sub) where T : ISignal
    {
        if (_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers[typeof(T)].Remove(sub);
        }
    }

    public static void Fire<T>(T e) where T : ISignal
    {
        if (_subscribers.TryGetValue(typeof(T), out var subs))
        {
            foreach (var sub in subs.ToList())
            {
                ((ISubscriber<T>)sub).OnEventHappen(e);
            }
        }
    }
}
