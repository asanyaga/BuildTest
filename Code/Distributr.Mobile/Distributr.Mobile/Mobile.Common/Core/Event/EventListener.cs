using System;
using System.Reflection;

namespace Mobile.Common.Core.Event
{
    public class EventListener
    {
        private readonly object instance;
        private readonly MethodInfo method;

        public EventListener(object instance, MethodInfo method)
        {
            this.instance = instance;
            this.method = method;
        }

        public bool IsEqualTo(Object instance)
        {
            return this.instance == instance;
        }

        public void Receive(Object message)
        {
            method.Invoke(instance, new[] {message});
        }
    }
}