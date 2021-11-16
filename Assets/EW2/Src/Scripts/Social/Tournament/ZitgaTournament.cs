using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using ZitgaSaveLoad;
using ZitgaTournamentMode;
using LogicCode = ZitgaTournamentMode.LogicCode;

namespace EW2
{
    public class ZitgaTournament
    {
        private const string PRODUCTION_HOST = "34.87.170.169";
        private const string SANDBOX_HOST = "35.198.248.251";
        private const int PORT = 8902;
        private const int API_VERSION = 1;
        private const string SECRET_KEY = "2hKh6YWG8EXC5QB2";
        public UnityAction<int, RankingOutbound> OnLoadResult;
        public UnityAction<int, TournamentInfoOutbound> OnLoadInfoResult;
        public UnityAction<bool> OnUpdateRanking;

        private TournamentService _tournamentService;

        public ZitgaTournament()
        {
            var option =
                new TournamentOption(GameLaunch.isCheat ? SANDBOX_HOST : SANDBOX_HOST, PORT) {
                    ApiVersion = API_VERSION, SecretKey = SECRET_KEY
                };

            option.UpdateRankingTournamentCallback = OnUpdate;
            option.GetRankingTournamentCallback = OnLoad;
            option.GetTournamentInfoCallback = OnLoadInfo;
            
            _tournamentService = new TournamentService(option);
        }


        public void LoadRanking(AuthProvider provider, string token)
        {
            GetRankingOutbound loadOutbound = new GetRankingOutbound();
            loadOutbound.SetData(provider, token);

            Debug.Log("[Get Ranking Tournament] Load sent");
            this._tournamentService.Load(loadOutbound);
        }

        private void OnLoad(int logiccode, RankingOutbound rankingdata)
        {
            Debug.Log("Load: logicCode=" + logiccode);

            if (logiccode == LogicCode.DATA_NOT_FOUND || rankingdata == null)
            {
                Ultilities.ShowToastNoti(L.popup.no_data_found);
                return;
            }


            OnLoadResult?.Invoke(logiccode, rankingdata);
        }


        public void UpdateRanking(AuthProvider provider, string token, RankingInbound dataRanking)
        {
            var saveOutbound = new UpdateRankingOutbound(provider, token);
            var jsonSerializerSettings =
                new JsonSerializerSettings() {TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects};
            var json = JsonConvert.SerializeObject(dataRanking, jsonSerializerSettings);
            saveOutbound.SetData(json);
            Debug.Log($"[Update Ranking Tournament]: {token}, data: {json}");
            this._tournamentService.UpdateRanking(saveOutbound);
        }

        private void OnUpdate(int logiccode)
        {
            Debug.Log("[Update Ranking Tournament] Update: logicCode=" + logiccode);
            if (logiccode == 200)
            {
                this.OnUpdateRanking?.Invoke(true);
            }
            else
            {
                this.OnUpdateRanking?.Invoke(false);
            }
        }
        
        public void LoadTournamentInfo(AuthProvider provider, string token)
        {
            GetTournamentInfoOutbound loadOutbound = new GetTournamentInfoOutbound();
            loadOutbound.SetData(provider, token);

            Debug.Log("[Get Tournament Info] Load info sent");
            this._tournamentService.LoadTournamentInfo(loadOutbound);
        }

        private void OnLoadInfo(int logiccode, TournamentInfoOutbound tournamentInfo)
        {
            Debug.Log("Load: logicCode=" + logiccode);

            if (logiccode == LogicCode.DATA_NOT_FOUND || tournamentInfo == null)
            {
                Ultilities.ShowToastNoti(L.popup.no_data_found);
                return;
            }


            OnLoadInfoResult?.Invoke(logiccode, tournamentInfo);
        }
    }
}