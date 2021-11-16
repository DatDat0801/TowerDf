using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using ZitgaUtils;

namespace EW2.Social
{
    public class ZitgaServerUtils
    {
        //Host: 34.87.170.169, Port: 9000, Api Version: 1, Secret Key: WN3e67MkANRmRTG2
        private const string HOST = "34.87.170.169";

        private const int PORT = 9000;

        private const int API_VERSION = 1;

        private const string SECRET_KEY = "WN3e67MkANRmRTG2";

        private ZitgaUtilsService zitgaUtilsService;

        public event UnityAction<int, LocationInbound> onGetInfo;

        public ZitgaServerUtils()
        {
            ZitgaUtilsOption option = new ZitgaUtilsOption(HOST, PORT);

            option.ApiVersion = API_VERSION;
            option.SecretKey = SECRET_KEY;

            option.GetLocationInfoCallback = OnGetLocationInfo;

            zitgaUtilsService = new ZitgaUtilsService(option);
            zitgaUtilsService.GetLocationInfo();
        }

        public void OnGetLocationInfo(int logicCode, LocationInbound inbound)
        {
            Debug.Log("OnGetLocationInfo: logicCode=" + logicCode);
            onGetInfo?.Invoke(logicCode, inbound);
            if (logicCode == LogicCode.SUCCESS)
            {
                Debug.Log("OnGetLocationInfo: inbound=" + JsonConvert.SerializeObject(inbound));
            }
            else
            {
                Debug.LogAssertion("FAIL TO SYNC INFO WITH SERVER");
            }
        }
    }
}