using UnityEngine;
using Zitga.ContextSystem;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class ShopDataBase
    {
        private ServiceContainer container;

        public ShopDataBase()
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
            var idData = "";

            T result = null;

#if TRACKING_FIREBASE
            var idDataRemoteConfig = FirebaseLogic.Instance.GetIapDataId();
            Debug.Log($"IdIapDataRemoteConfig {idDataRemoteConfig}");
            if (idDataRemoteConfig > 0)
                idData = $"_{idDataRemoteConfig}";
#endif

            result = Resources.Load<T>($"CSV/Shops{idData}/{typeof(T).Name}");

            if (result == null)
                result = Resources.Load<T>($"CSV/Shops/{typeof(T).Name}");

            return result;
        }
    }
}