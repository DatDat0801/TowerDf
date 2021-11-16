using System;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using ZitgaRankingDefendMode;

namespace EW2
{
    public class LobbyContainer : TabContainer
    {
        private const int INDEX_ADS_DATA = 7;

        [SerializeField] private Button startButton;
        [SerializeField] private Button watchAdsButton;
        [SerializeField] private Text txtNameMap;
        [SerializeField] private Image mapCover;
        [SerializeField] private Text txtTicketRemain;
        [SerializeField] private Text txtTurnWatchAds;
        [SerializeField] private TimeRemainUi timeRemainReset;
        [SerializeField] private InfoSeasonDefenseModeController infoSeason;
        [SerializeField] private HeroSelectedDefenseModeController heroSelected;
        [SerializeField] private DefensivePointSelectedController defensivePointSelected;
        [SerializeField] private PreviewRewardController previewReward;

        private HeroDefendModeConfig.DataConfig _dataConfig;
        private RankingDefense _currDataRanking;

        private void Awake()
        {
            this._dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();

            EventManager.StartListening(GamePlayEvent.OnRefreshFormationDefense, () => {
                this.heroSelected.UpdateListHeroSelected();
            });

            this.startButton.onClick.AddListener(StartClick);

            this.watchAdsButton.onClick.AddListener(WatchAdsClick);

            EventManager.StartListening(GamePlayEvent.OnNewSeasonHeroDefense, () => {
                ShowUi();
            });
        }

        private void OnEnable()
        {
            VideoAdPlayer.Instance.OnRewarded += WatchAdsSuccess;
        }

        private void OnDisable()
        {
            if (VideoAdPlayer.Instance != null)
                VideoAdPlayer.Instance.OnRewarded -= WatchAdsSuccess;
        }

        public void SetUserDataServer(RankingDefense dataUser)
        {
            this._currDataRanking = dataUser;
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            ShowUi();
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void ShowUi()
        {
            ShowUiMapCover();

            ShowUiBottom();

            if (this._currDataRanking != null)
            {
                UserData.Instance.UserHeroDefenseData.SetHighestScore(this._currDataRanking.WaveCleared);
                this.infoSeason.SetData(this._currDataRanking.Rank, this._currDataRanking.WaveCleared);
                this.previewReward.SetData(this._currDataRanking.WaveCleared);
            }
            else
            {
                this.infoSeason.SetData(-1, 0);
                this.previewReward.SetData(0);
            }

            this.heroSelected.ShowListHeroUsed();

            this.defensivePointSelected.ShowInfo();
            
        }

        private void ShowUiMapCover()
        {
            var userData = UserData.Instance.UserHeroDefenseData;
            if (userData != null)
            {
                this.txtNameMap.text =
                    Localization.Current.Get("playable_mode", $"hero_defens_map_name_{userData.currMapId + 1}");
                this.mapCover.sprite =
                    ResourceUtils.GetSpriteAtlas("map_defense_mode", $"defensive_mode_map_{userData.currMapId}");
            }
        }

        private void ShowUiBottom()
        {
            var userData = UserData.Instance.UserHeroDefenseData;
            this.txtTicketRemain.text = string.Format(L.playable_mode.turn_left_txt, userData.numberTicket);
            this.timeRemainReset.SetTimeRemain(userData.GetTimeRemainResetNewDay(), TimeRemainFormatType.Hhmmss,
                RetsetNewDay);
            this.startButton.gameObject.SetActive(userData.numberTicket > 0);
            this.watchAdsButton.gameObject.SetActive(userData.numberTicket <= 0 && userData.timesWatchAds > 0);
            this.startButton.GetComponentInChildren<Text>().text = L.button.btn_start.ToUpper();
            this.watchAdsButton.GetComponentInChildren<Text>().text = L.button.watched_video.ToUpper();
            this.txtTurnWatchAds.text = $"{userData.timesWatchAds}/{this._dataConfig.numberWatchAds}";
        }

        private void RetsetNewDay()
        {
            UserData.Instance.UserHeroDefenseData.CheckNewDay();
            ShowUiBottom();
        }

        #region Button Click

        private void WatchAdsClick()
        {
            var userData = UserData.Instance.UserHeroDefenseData;

            if (userData.timesWatchAds <= 0) return;

            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var addData = adRewardData.ads[INDEX_ADS_DATA];
            if (addData != null)
            {
                VideoAdPlayer.Instance.PlayAdClick(addData.placementName);
            }
        }

        private void StartClick()
        {
            var userData = UserData.Instance.UserHeroDefenseData;

            if (userData.numberTicket <= 0) return;

            if (!userData.CheckDefensePointUnlocked(userData.defensePointId) && userData.numberTrial > 0)
            {
                userData.numberTrial--;
            }

            userData.numberTicket--;
            UserData.Instance.Save();
            GoToGamePlay();
        }

        #endregion

        private void GoToGamePlay()
        {
            ConfigGamePlay();

#if TRACKING_FIREBASE
            var userData = UserData.Instance.UserHeroDefenseData;
            FirebaseLogic.Instance.DefenseModeStart(userData.battleId, userData.defensePointId,
                userData.listHeroSelected);
#endif

            LoadSceneUtils.LoadScene(SceneName.HeroDefenseGamePlay);
        }

        private void ConfigGamePlay()
        {
            UserData.Instance.UserHeroDefenseData.battleId++;

            GamePlayControllerBase.CampaignId = UserData.Instance.UserHeroDefenseData.currMapId;

            GamePlayControllerBase.gameMode = GameMode.DefenseMode;

            GamePlayControllerBase.heroList.Clear();

            GamePlayControllerBase.heroList.AddRange(UserData.Instance.UserHeroDefenseData.GetListHeroes());
        }

        private void WatchAdsSuccess(string arg0)
        {
            var userData = UserData.Instance.UserHeroDefenseData;
            if (!userData.CheckDefensePointUnlocked(userData.defensePointId) && userData.numberTrial > 0)
            {
                userData.numberTrial--;
            }

            userData.timesWatchAds--;
            UserData.Instance.Save();
            GoToGamePlay();
        }
    }
}