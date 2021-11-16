using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class DefendModeDataBase
    {
        private ServiceContainer container;

        public DefendModeDataBase()
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
            return Resources.Load<T>($"CSV/HeroDefendMode/{typeof(T).Name}");
        }

        public DefensiveModeMapData GetMap(int mapId)
        {
            var mapName = $"defensive_map_{mapId}";

            var obj = container.Resolve<DefensiveModeMapData>(mapName);

            if (obj == null)
            {
                obj = Resources.Load<DefensiveModeMapData>($"CSV/DefensiveMaps/{mapName}");

                container.Register(mapName, obj);
            }

            return obj;
        }
    }
}