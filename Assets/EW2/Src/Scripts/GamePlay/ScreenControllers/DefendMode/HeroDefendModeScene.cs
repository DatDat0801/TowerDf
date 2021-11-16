using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;
using ZitgaRankingDefendMode;

namespace EW2
{
    public enum DefendModeTabId
    {
        Lobby = 0,
        Rewards = 1,
        Leaderboard = 2
    }

    public class HeroDefendModeScene : AWindowController, IUpdateTabBarChanged
    {
        [SerializeField] private Text titleDefendMode;

        [SerializeField] private Button buttonClose;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private Button helpButton;

        [SerializeField] private List<TabContainer> tabContainers;

        private int _currentTabId;
        private ZitgaRankingDefense _zitgaRankingDefense;
        private RankingDefenseOutbound _currDataRanking;

        protected override void Awake()
        {
            base.Awake();

            this.buttonClose.onClick.AddListener(CloseClick);

            this.helpButton.onClick.AddListener(HelpClick);
        }

        public void OnTabBarChanged(int indexActive)
        {
            this._currentTabId = indexActive;

            if (this._currentTabId == (int)DefendModeTabId.Lobby)
            {
                ((LobbyContainer)tabContainers[this._currentTabId]).SetUserDataServer(this._currDataRanking
                    .RankingData);
            }
            else if (this._currentTabId == (int)DefendModeTabId.Leaderboard)
            {
                ((LeaderboardContainer)tabContainers[this._currentTabId]).SetData(this._currDataRanking);
            }

            for (int i = 0; i < tabContainers.Count; i++)
            {
                if (i == indexActive)
                {
                    tabContainers[i].ShowContainer();
                }
                else
                {
                    tabContainers[i].HideContainer();
                }
            }
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            titleDefendMode.text = L.playable_mode.hero_defense_name_txt.ToUpper();
            UserData.Instance.UserHeroDefenseData.CheckTimeSeason();
            GetDataServer();
        }

        private void InitTabManager()
        {
            _currentTabId = 0;

            if (!tabsManager.IsInited)
            {
                tabsManager.InitTabManager(this, this._currentTabId);
            }
            else
            {
                tabsManager.SetSelected(this._currentTabId);
            }
        }

        private void GetDataServer()
        {
            if (_zitgaRankingDefense == null)
                _zitgaRankingDefense = new ZitgaRankingDefense();

            try
            {
                this._zitgaRankingDefense.OnLoadResult = OnLoadResult;
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

                this._zitgaRankingDefense.Load(authProvider, userId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void OnLoadResult(int logicCode, RankingDefenseOutbound dataRanking)
        {
            if (logicCode == LogicCode.SUCCESS && dataRanking != null)
            {
                this._currDataRanking = dataRanking;
                InitTabManager();
            }
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.hero_defend_mode_scene);
            this._currentTabId = (int)DefendModeTabId.Lobby;
            tabsManager.SetSelected(this._currentTabId);
            UIFrame.Instance.OpenWindow(ScreenIds.home);
        }

        private void HelpClick()
        {
            PopupInfoWindowProperties data = new PopupInfoWindowProperties(L.playable_mode.hero_defense_name_txt,
                L.playable_mode.hero_defense_rule);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_info, data);
        }
    }
}