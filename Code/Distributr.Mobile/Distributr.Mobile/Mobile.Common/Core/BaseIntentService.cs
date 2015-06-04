using Android.App;

namespace Mobile.Common.Core
{
    [Service]
    public abstract class BaseIntentService<U> : IntentService
    {
        protected BaseApplication<U> App { get; private set; }

        protected T Resolve<T>() where T : class
        {
            return App.Resolve<T>();
        }

        protected U User { get { return App.User; } }

        protected void Publish(object message)
        {
            App.Publish(message);
        }

        public override sealed void OnCreate()
        {
            base.OnCreate();
            App = (BaseApplication<U>) Application;
            App.Register(this);
            Created();
        }

        public virtual void Created()
        {
        }

        public override sealed void OnDestroy()
        {
            App.Unregister(this);
            base.OnDestroy();
            Destroyed();
        }

        public virtual void Destroyed()
        {
        }
    }
}