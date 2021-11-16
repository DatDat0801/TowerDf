using Newtonsoft.Json;
using UnityEngine;

namespace ZitgaUtils
{
    public class ZitgaUtilsExample : MonoBehaviour
    {

        public string Host;

        public int Port;

        public int ApiVersion;

        public string SecretKey;

        private ZitgaUtilsService zitgaUtilsService;

        void Start()
        {
            ZitgaUtilsOption option = new ZitgaUtilsOption(Host, Port);

            option.ApiVersion = ApiVersion;
            option.SecretKey = SecretKey;

            option.GetLocationInfoCallback = OnGetLocationInfo;

            zitgaUtilsService = new ZitgaUtilsService(option);
            zitgaUtilsService.GetLocationInfo();
        }

        #region Get location info
        public void OnGetLocationInfo(int logicCode, LocationInbound inbound)
        {
            Debug.Log("OnGetLocationInfo: logicCode=" + logicCode);

            if (logicCode == LogicCode.SUCCESS)
            {
                Debug.Log("OnGetLocationInfo: inbound=" + JsonConvert.SerializeObject(inbound));
            }
        }
        #endregion
    }
}