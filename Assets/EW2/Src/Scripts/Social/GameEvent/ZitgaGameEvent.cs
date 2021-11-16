using UnityEngine;
using UnityEngine.Events;
using ZitgaSaveLoad;
using ZitgaUtils;
using LogicCode = ZitgaTournamentMode.LogicCode;

namespace EW2
{
    public class ZitgaGameEvent
    {
        private const string PRODUCTION_HOST = "34.87.170.169";
        private const string SANDBOX_HOST = "35.198.248.251";
        private const int PORT = 8902;
        private const int API_VERSION = 1;
        private const string SECRET_KEY = "2hKh6YWG8EXC5QB2";

        private ZitgaUtilsService _zitgaUtilsService;

        public UnityAction<int, EventTimeOutbound> OnLoadResult;

        public ZitgaGameEvent()
        {
            var option =
                new ZitgaUtilsOption(GameLaunch.isCheat ? SANDBOX_HOST : SANDBOX_HOST, PORT) {
                    ApiVersion = API_VERSION, SecretKey = SECRET_KEY
                };

            option.GetEventDataCallback = OnGetEvent;

            _zitgaUtilsService = new ZitgaUtilsService(option);
        }

        public void GetEventData(AuthProvider provider, string token)
        {
            GetEventDataInbound loadInbound = new GetEventDataInbound();
            loadInbound.SetData(provider, token);

            Debug.Log("[Get Event Data] Load sent");
            this._zitgaUtilsService.GetEventInfo(loadInbound);
        }

        private void OnGetEvent(int logiccode, EventTimeOutbound outbound)
        {
            Debug.Log("Load: logicCode=" + logiccode);

            if (logiccode == LogicCode.DATA_NOT_FOUND || outbound == null)
            {
                Ultilities.ShowToastNoti(L.popup.no_data_found);
                return;
            }


            OnLoadResult?.Invoke(logiccode, outbound);
        }
    }
}