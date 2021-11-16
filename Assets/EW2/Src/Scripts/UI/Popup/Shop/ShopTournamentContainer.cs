using System;
using EW2.Tools;
using UnityEngine;
using UnityEngine.UI;
using ZitgaSaveLoad;
using ZitgaTournamentMode;
using LogicCode = ZitgaTournamentMode.LogicCode;

namespace EW2
{
    public class ShopTournamentContainer : TabContainer
    {
        [SerializeField] private Text titleTxt;
        [SerializeField] private Text refreshTxt;
        [SerializeField] private ShopTournamentUIItem itemTemplate;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private TimeCountDownUi timer;

        
        private ShopTournamentData _shopData;

        public override void ShowContainer()
        {

            if (this._shopData == null)
            {
                this._shopData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopTournamentData>();
                var tournamentData = UserData.Instance.TournamentData;
                if (tournamentData.ShopUserData.CanResetSeason())
                {
                    ResetSeason(tournamentData.GetSeasonId());
                }
            }

            SetLocalization();
            Repaint();

            gameObject.SetActive(true);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void SetLocalization()
        {
            if (this.titleTxt)
            {
                this.titleTxt.text = L.playable_mode.tournament_shop_title;
            }

            if (this.refreshTxt)
            {
                this.refreshTxt.text = L.popup.reset_in_txt;
            }
        }

        private void Repaint()
        {
            var userData = UserData.Instance.TournamentData.ShopUserData;
            //this.itemContainer.DespawnAllChildren();
            this.itemContainer.DestroyAllChildren();
            for (var i = 0; i < userData.items.Count; i++)
            {
                var go = Instantiate(this.itemTemplate, this.itemContainer);
                go.Repaint(userData.items[i]);
                go.gameObject.SetActive(true);
            }
            if (timer)
            {
                timer.SetData(UserData.Instance.TournamentData.GetTimeRemainSeason(), TimeRemainFormatType.Ddhhmmss,
                    HandleOnEnd);
            }
        }

        void HandleOnEnd()
        {
            GetDataServer();
        }

        private void ResetSeason(long newSeasonId)
        {
            //Todo handle for hero, trang phuc, if hero is own => hide 
            var userData = UserData.Instance.TournamentData.ShopUserData;
            userData.CreateNewSeasonItems(this._shopData, newSeasonId);
        }

        #region Server

        private void GetDataServer()
        {

            var zitgaTournament = new ZitgaTournament();

            try
            {
                zitgaTournament.OnLoadResult = OnLoadResult;
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

                zitgaTournament.LoadRanking(authProvider, userId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void OnLoadResult(int logicCode, RankingOutbound dataRanking)
        {
            if (logicCode == LogicCode.SUCCESS && dataRanking != null)
            {
                UserData.Instance.TournamentData.SetTournamentSeasonId(dataRanking.season);
                Repaint();
                ResetSeason(dataRanking.season);
            }
        }

        #endregion
    }
}