using System;
using System.Collections.Generic;

public static class EventManager 
{
    private static readonly Dictionary<string, Action<Dictionary<string, object>>> s_eventDictionary = new Dictionary<string, Action<Dictionary<string, object>>>();

    public static void StartListening(string eventName, Action<Dictionary<string, object>> listener) 
    {
        if (s_eventDictionary.TryGetValue(eventName, out var thisEvent)) 
        {
            thisEvent += listener;
            s_eventDictionary[eventName] = thisEvent;
        } 
        else 
        {
            thisEvent += listener;
            s_eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<Dictionary<string, object>> listener) 
    {
        if (s_eventDictionary.TryGetValue(eventName, out var thisEvent)) 
        {
            thisEvent -= listener;
            s_eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, Dictionary<string, object> message) 
    {
        if (s_eventDictionary.TryGetValue(eventName, out var thisEvent)) 
        {
            thisEvent?.Invoke(message);
        }
    }
}