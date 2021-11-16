using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public static class ResourceUtilsAsync
    {
        private static readonly ServiceContainer cacheContainer = new ServiceContainer();

        public static async UniTask<GameObject> GetUnitPrefab(string prefabName)
        {
            string fullPath = $"Art/Prefabs/Units/{prefabName}";

            return await GetAsync<GameObject>(fullPath);
        }
    
        public static async UniTask<GameObject> GetUnit(string prefabName, Transform parent = null)
        {
            GameObject prefab = await GetUnitPrefab(prefabName);

            return LeanPool.Spawn(prefab, parent);
        }
    
        public static async UniTask<GameObject> GetUnit(string prefabName, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject prefab = await GetUnitPrefab(prefabName);

            return LeanPool.Spawn(prefab, position, rotation, parent);
        }

        private static async UniTask<T> GetAsync<T>(string path) where T : Object
        {
            T cache = cacheContainer.Resolve<T>(path);

            if (cache == null)
            {
                cache = (T)(await Resources.LoadAsync<T>(path));

                cacheContainer.Register(path, cache);
            
                Debug.Assert(cache != null, "Object is not exist: " + path);
            }

            return cache;
        }
    }

}

