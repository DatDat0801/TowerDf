using UnityEngine;
using Zitga.ContextSystem;

namespace EW2.Events
{
    public class EventDatabase
    {
        private ServiceContainer container;

        public EventDatabase()
        {
            container = new ServiceContainer();
        }
        
        public T Get<T>() where T : ScriptableObject
        {
            var obj = container.Resolve<T>();
            if (obj == null)
            {
                obj = LoadFromResource<T>();

                container.Register(obj);
            }

            return obj;
        }
        
        private T LoadFromResource<T>() where T : ScriptableObject
        {
            return Resources.Load<T>($"CSV/GameOpsEvent/{typeof(T).Name}");
        }
    }
}