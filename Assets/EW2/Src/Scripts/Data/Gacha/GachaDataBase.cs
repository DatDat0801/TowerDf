
using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class GachaDataBase 
    {
        private ServiceContainer container;

        public GachaDataBase()
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
            return Resources.Load<T>($"CSV/Gacha/{typeof(T).Name}");
        }
    }
}