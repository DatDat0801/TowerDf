using System;
using TigerForge;
using UnityEngine;
using ZitgaSaveLoad;
using ZitgaTournamentMode;
using ZitgaUtils;
using LogicCode = ZitgaTournamentMode.LogicCode;

namespace EW2
{
    public class GameEventHandler : MonoBehaviour
    {
        private ZitgaGameEvent _zitgaGameEvent;
        private ZitgaTournament _zitgaTournament;

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnLoginSuccess, GetDataServer);
        }

        private void Start()
        {
            if (this._zitgaGameEvent == null)
            {
                this._zitgaGameEvent = new ZitgaGameEvent();
            }

            if (this._zitgaTournament == null)
            {
                this._zitgaTournament = new ZitgaTournament();
            }

            GetDataServer();
        }

        private void GetDataServer()
        {
            if (string.IsNullOrEmpty(UserData.Instance.AccountData.tokenId))
            {
                return;
            }
            
            try
            {
                this._zitgaGameEvent.OnLoadResult = OnLoadResult;
                this._zitgaTournament.OnLoadInfoResult = OnLoadInfoResult;

                var currentPlatform = Application.platform;

                var userId = UserData.Instance.AccountData.tokenId;

                var authProvider = AuthProvider.WINDOWS_DEVICE;

                switch (currentPlatform)
                {
                    case RuntimePlatform.Android:
                        authProvider = AuthProvider.ANDROID_DEVICE;
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        authProvider = AuthProvider.IOS_DEVICE;
                        break;
                }

                this._zitgaGameEvent.GetEventData(authProvider, userId);
                this._zitgaTournament.LoadTournamentInfo(authProvider, userId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void OnLoadResult(int logicCode, EventTimeOutbound dataEvent)
        {
            if (dataEvent.EventTimes == null)
            {
                return;
            }

            Debug.Log($"Number Events: {dataEvent.EventTimes.Count}");

            foreach (var eventTime in dataEvent.EventTimes)
            {
                if (eventTime.EventType == EventIds.TOURNAMENT_EVENT)
                {
                    UserData.Instance.TournamentData.startTime = eventTime.StartTime / 1000;
                    UserData.Instance.TournamentData.endTime = eventTime.EndTime / 1000;
                }
            }
        }

        private void OnLoadInfoResult(int logicCode, TournamentInfoOutbound tournamentInfo)
        {
            if (logicCode == LogicCode.SUCCESS && tournamentInfo != null)
            {
                var userData = UserData.Instance.TournamentData;

                if (userData.ShopUserData.seasonId < tournamentInfo.Season)
                {
                    userData.SetTournamentMapId();

                    userData.SetTournamentSeasonId(tournamentInfo.Season);

                    userData.SetListHeroBuff(tournamentInfo.HeroBuffIds);

                    userData.buffStatId = tournamentInfo.StatBuffId;

                    userData.buffStatIdPrev = 0;
                }

                userData.heroNerfId = tournamentInfo.HeroNerfId;
                userData.spellBanId = tournamentInfo.StatBanId;
                userData.nerfStatId = tournamentInfo.StatNerfId;

                UserData.Instance.Save();
            }
        }
    }
}