using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using ZitgaRankingDefendMode;
using AuthProvider = ZitgaRankingDefendMode.AuthProvider;
using LogicCode = ZitgaRankingDefendMode.LogicCode;

namespace EW2
{
    public class ZitgaRankingDefense
    {
        private const string PRODUCTION_HOST = "34.87.170.169";
        private const string SANDBOX_HOST = "35.198.248.251";
        private const int PORT = 8902;
        private const int API_VERSION = 1;
        private const string SECRET_KEY = "2hKh6YWG8EXC5QB2";
        public UnityAction<int, RankingDefenseOutbound> OnLoadResult;
        public UnityAction<bool> OnUpdateRanking;

        private RankingDefenseModeService _rankingDefenseModeService;

        public ZitgaRankingDefense()
        {
            RankingDefenseOption option =
                new RankingDefenseOption(GameLaunch.isCheat ? SANDBOX_HOST : PRODUCTION_HOST, PORT) {
                    ApiVersion = API_VERSION, SecretKey = SECRET_KEY
                };

            option.UpdateRankingDefenseCallback = OnUpdate;
            option.GetRankingDefenseCallback = OnLoad;

            _rankingDefenseModeService = new RankingDefenseModeService(option);
        }

        #region Update

        public void UpdateRanking(AuthProvider provider, string token, RankingDefenseInbound dataRanking)
        {
            UpdateRankingDefenseOutbound saveOutbound = new UpdateRankingDefenseOutbound(provider, token);
            var jsonSerializerSettings =
                new JsonSerializerSettings() {TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects};
            var json = JsonConvert.SerializeObject(dataRanking, jsonSerializerSettings);
            saveOutbound.SetData(json);
            Debug.Log($"[Update Ranking Defense]: {token}, data: {json}");
            this._rankingDefenseModeService.UpdateRanking(saveOutbound);
        }

        private void OnUpdate(int logicCode)
        {
            Debug.Log("[Update Ranking Defense] Update: logicCode=" + logicCode);
            if (logicCode == 200)
            {
                this.OnUpdateRanking?.Invoke(true);
            }
            else
            {
                this.OnUpdateRanking?.Invoke(false);
            }
        }

        #endregion

        #region Load

        public void Load(AuthProvider provider, string token)
        {
            GetRankingDefenseModeOutbound loadOutbound = new GetRankingDefenseModeOutbound();
            loadOutbound.SetData(provider, token);

            Debug.Log("[Get Ranking Defen] Load sent");
            this._rankingDefenseModeService.Load(loadOutbound);
        }

        private async void OnLoad(int logicCode, RankingDefenseOutbound dataRanking)
        {
            Debug.Log("Load: logicCode=" + logicCode);

            if (logicCode == LogicCode.DATA_NOT_FOUND || dataRanking == null)
            {
                Ultilities.ShowToastNoti(L.popup.no_data_found);
                return;
            }


            OnLoadResult?.Invoke(logicCode, dataRanking);
        }

        #endregion
    }
}