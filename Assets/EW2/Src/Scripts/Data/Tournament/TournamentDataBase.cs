using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class TournamentDataBase
    {
        private ServiceContainer container;

        public TournamentDataBase()
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
            return Resources.Load<T>($"CSV/Tournament/{typeof(T).Name}");
        }
    }
}