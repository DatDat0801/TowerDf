using System;
using System.Collections.Generic;

namespace Zitga.ContextSystem
{
    public class Context : IDisposable
    {
        private static Context context = new Context();
        
        public static Context Current => context;
        
        private readonly Dictionary<string, object> attributes;
        
        private readonly IServiceContainer container;

        private Context()
        {
            attributes = new Dictionary<string, object>();
            
            this.container = new ServiceContainer();
        }
        
        public virtual bool Contains(string name, bool cascade = true)
        {
            if (attributes.ContainsKey(name))
                return true;

            return false;
        }
        
        public virtual object Get(string name, bool cascade = true)
        {
            return Get<object>(name, cascade);
        }

        public virtual T Get<T>(string name, bool cascade = true)
        {
            object v;
            if (attributes.TryGetValue(name, out v))
                return (T) v;

            return default;
        }
        
        public virtual void Set(string name, object value)
        {
            Set<object>(name, value);
        }

        public virtual void Set<T>(string name, T value)
        {
            attributes[name] = value;
        }

        public virtual object Remove(string name)
        {
            return Remove<object>(name);
        }

        public virtual T Remove<T>(string name)
        {
            if (!attributes.ContainsKey(name))
                return default;

            object v = attributes[name];
            attributes.Remove(name);
            return (T) v;
        }
        
        public virtual IServiceContainer GetContainer()
        {
            return container;
        }
        
        public virtual object GetService(Type type)
        {
            object result = container.Resolve(type);
            if (result != null)
                return result;

            return null;
        }

        public virtual object GetService(string name)
        {
            object result = container.Resolve(name);
            return result;
        }

        public virtual T GetService<T>()
        {
            T result = container.Resolve<T>();
            if (result != null)
                return result;

            return default;
        }

        public virtual T GetService<T>(string name)
        {
            T result = container.Resolve<T>(name);
            if (result != null)
                return result;

            return default;
        }
        
        #region IDisposable Support

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    IDisposable dis = container as IDisposable;
                    dis?.Dispose();
                }

                disposed = true;
            }
        }

        ~Context()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}