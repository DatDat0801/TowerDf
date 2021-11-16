using UnityEngine;
using Zitga.ContextSystem;

namespace EW2
{
    public class StaminaDataBase
    {
        private ServiceContainer container;

        public StaminaDataBase()
        {
            container = new ServiceContainer();
        }

        public StaminaData GetConfig()
        {
            var obj = container.Resolve<StaminaData>();
            if (obj == null)
            {
                obj = Resources.Load<StaminaData>($"CSV/Stamina/StaminaData");

                container.Register(obj);
            }

            return obj;
        }
    }
}