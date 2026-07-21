using System;
using System.Collections.Generic;

public class EventBus
{
    private readonly Dictionary<Type, Delegate> events = new();
    
    public void Subscribe<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out Delegate del))
        {
            events[typeof(T)] = Delegate.Combine(del, callback);
        }
        else
        {
            events[typeof(T)] = callback;
        }
    }
    
    public void Unsubscribe<T>(Action<T> callback)
    {
        if (!events.TryGetValue(typeof(T), out Delegate del)) return;
        
        Delegate newDelegate = Delegate.Remove(del, callback);
        
        if (newDelegate == null)
        {
            events.Remove(typeof(T));
        }
        else
        {
            events[typeof(T)] = newDelegate;
        }
    }
    
    public void Fire<T>(T evt)
    {
        if (events.TryGetValue(typeof(T), out Delegate del))
        {
            ((Action<T>)del)?.Invoke(evt);
        }
    }
}