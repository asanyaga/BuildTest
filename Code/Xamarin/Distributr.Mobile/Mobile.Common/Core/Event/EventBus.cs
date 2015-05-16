using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Mobile.Common.Core.Event
{
    // A simple reflection-based Event Bus. It adds subscribers if they declare OnEvent methods 
    // with a single parameter: the event/message they are interested in.
    public class EventBus
    {
        private readonly ConcurrentDictionary<Type, List<EventListener>> listeners =
            new ConcurrentDictionary<Type, List<EventListener>>();

        private readonly object lockObject = new object();

        public void Register(Object listener)
        {
            lock (lockObject)
            {
                var methods = listener.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

                foreach (var method in methods)
                {
                    if (method.Name.Equals("OnEvent"))
                    {
                        if (method.GetParameters().Length != 1)
                        {
                            throw new Exception("OnEvent methods must take exactly one parameter: " + listener);
                        }
                        var eventType = method.GetParameters()[0].ParameterType;

                        listeners.GetOrAdd(eventType, new List<EventListener>())
                            .Add(new EventListener(listener, method));
                    }
                }
            }
        }

        public void Unregister(Object listener)
        {
            lock (lockObject)
            {
                foreach (var list in listeners.Values)
                {
                    for (var i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].IsEqualTo(listener))
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void Publish(Object message)
        {
            lock (lockObject)
            {
                var listenersForEvent = listeners.GetOrAdd(message.GetType(), new List<EventListener>());

                foreach (var listener in listenersForEvent)
                {
                    listener.Receive(message);
                }
            }
        }
    }
}