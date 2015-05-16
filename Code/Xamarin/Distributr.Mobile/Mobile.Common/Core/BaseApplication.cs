using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Mobile.Common.Core.Event;
using Mobile.Common.Core.Net;

namespace Mobile.Common.Core
{
    public abstract class BaseApplication<U> : Application
    {
        private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
        protected readonly JavaDictionary<string, int> layouts = new JavaDictionary<string, int>();
        protected readonly JavaDictionary<string, int> menus = new JavaDictionary<string, int>();

        private readonly ConnectivityMonitor<U> connectivityMonitor;
        private readonly EventBus eventBus = new EventBus();

        public U User { get; set; }

        protected BaseApplication(Type layoutClassType, Type menuClassType, IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            this.connectivityMonitor = new ConnectivityMonitor<U>(this);
            RegisterResources(layoutClassType, menuClassType);
        }

        private void RegisterResources(Type layoutClassType, Type menuClassType)
        {
            RegisterResources(typeof(Resource.Layout), layouts);
            RegisterResources(layoutClassType, layouts);
            RegisterResources(menuClassType, menus);
        }

        private void RegisterResources(Type layoutClassType, JavaDictionary<string, int> container)
        {
            foreach (var field in layoutClassType.GetFields())
            {
                var name = field.Name.Replace("_", "").ToLower();
                if (container.ContainsKey(name))
                {
                    container.Remove(name);
                }
                container.Add(name, field.GetValue(null));
            }
        }

        public void WaitForNetwork()
        {
            connectivityMonitor.WaitForNetwork();
        }

        public bool IsNetworkAvailable()
        {
            return connectivityMonitor.IsNetworkAvailable();
        }

        public void Put(object value)
        {
            if (cache.ContainsKey(value.GetType()))
            {
                cache.Remove(value.GetType());
            }
            cache[value.GetType()] = value;
        }

        public T Get<T>(T defaultValue = default(T))
        {
            var type = typeof (T);

            if (cache.ContainsKey(type))
            {
                return (T) cache[type];
            }
            return defaultValue;
        }

        public void Register(Object listener)
        {
            eventBus.Register(listener);
        }

        public void Unregister(Object listener)
        {
            eventBus.Unregister(listener);
        }

        public void Publish(Object message)
        {
            eventBus.Publish(message);
        }

        public int ResolveLayout(string name)
        {
            if (layouts.ContainsKey(name))
            {
                return layouts[name];
            }
            return -1;
        }

        public int ResolveMenu(string name)
        {
            if (menus.ContainsKey(name))
            {
                return menus[name];
            }
            return -1;
        }

        public T Resolve<T>() where T : class
        {
            return _Resolve<T>();
        }

        protected abstract T _Resolve<T>() where T : class;
        public abstract bool Initialised();
        public abstract void InitialiseFor(U user);
        public abstract void Logout();
    }
}