using System;
using System.Text;
using Hellmade.Sound;
using Invoke;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;
using ZitgaSaveLoad;
using ZitgaTournamentMode;
using ZitgaUtils;
using LogicCode = ZitgaTournamentMode.LogicCode;

namespace EW2
{
    public class TournamentLobbyWindow : AWindowController
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button rewardButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button heroButton;
        [SerializeField] private Button infoButton;

        [SerializeField] private Button enterButton;
        [SerializeField] private Button changeBuffButton;
        [SerializeField] private TimeCountDownUi timer;

        //text
        [SerializeField] private Text txtTitleTournament;
        [SerializeField] private Text changeBuffTxt;
        [SerializeField] private Text rankNameTxt;
        [SerializeField] private Text rankTxt;
        [SerializeField] private Text rankEmptyTxt;
        [SerializeField] private Text leaderboardTxt;
        [SerializeField] private Text rewardTxt;
        [SerializeField] private Text shopTxt;
        [SerializeField] private Text heroTxt;
        [SerializeField] private Text enterTxt;

        [SerializeField] private Image rankIcon;
        [SerializeField] private GameObject lobby1;
        [SerializeField] private GameObject lobby2;

        [SerializeField] private TournamentTimeTicketUi timeTicketUi;
        [SerializeField] private GameObject iconNoticeReward;

        private ZitgaTournament _zitgaTournament;
        private ZitgaGameEvent _zitgaGameEvent;
        private RankingOutbound _currDataRanking;

        private RuntimePlatform _currentPlatform;
        private AuthProvider _authProvider = AuthProvider.WINDOWS_DEVICE;
        private string _userId = "";
        private bool _isInit = false;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            this.leaderboardButton.onClick.AddListener(LeaderboardClick);
            this.rewardButton.onClick.AddListener(RewardClick);
            this.shopButton.onClick.AddListener(ShopClick);
            this.heroButton.onClick.AddListener(HeroClick);
            this.infoButton.onClick.AddListener(InfoButtonClick);
            this.changeBuffButton.onClick.AddListener(ChangeBuffClick);
            this.enterButton.onClick.AddListener(EnterClick);
            EventManager.StartListening(GamePlayEvent.OnClaimRewardTournament, () => {
                this.iconNoticeReward.SetActive(!UserData.Instance.TournamentData.isClaimedReward);
            });
        }


        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            SetLocalization();
            ShowUi();

            if (!_isInit)
            {
                _isInit = true;
                PrepareData();
            }

            GetDataRanking();
        }

        public override void SetLocalization()
        {
            base.SetLocalization();

            if (txtTitleTournament)
            {
                txtTitleTournament.text = L.worldmap.name_tournament.ToUpper();
            }

            if (this.changeBuffTxt)
            {
                this.changeBuffTxt.text = L.button.change_btn;
            }

            if (this.leaderboardTxt)
            {
                this.leaderboardTxt.text = L.playable_mode.leaderboard_txt.ToUpper();
            }

            if (this.rewardTxt)
            {
                this.rewardTxt.text = L.button.rewards.ToUpper();
            }

            if (this.shopTxt)
            {
                this.shopTxt.text = L.worldmap.name_shop.ToUpper();
            }

            if (this.heroTxt)
            {
                this.heroTxt.text = L.worldmap.name_hero.ToUpper();
            }

            if (this.enterTxt)
            {
                this.enterTxt.text = L.button.prepare_btn.ToUpper();
            }

            if (rankEmptyTxt)
            {
                rankEmptyTxt.text = L.playable_mode.unrank_txt;
            }
        }

        void RepaintSeasonTime()
        {
            if (timer)
            {
                timer.SetTitle(L.popup.end_time_txt);
                timer.SetData(UserData.Instance.TournamentData.GetTimeRemainSeason(), TimeRemainFormatType.Ddhhmmss,
                    HandleOnEnd);
            }
        }

        private void HandleOnEnd()
        {
            GetInfoResetTournament();
            GetDataRanking();
        }

        private void InfoButtonClick()
        {
            StringBuilder desc = new StringBuilder();
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_map_title)).Append("\n");
            desc.Append(L.playable_mode.tournament_rule_map).Append("\n\n");
            var dataConfig = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentTicketConfig>();
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_limit_title)).Append("\n");
            desc.Append(string.Format(L.playable_mode.tournament_rule_limit,
                dataConfig.tournamentTicketExchanges[0].ticketMax)).Append("\n\n");
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_grand_title)).Append("\n");
            desc.Append(L.playable_mode.tournament_rule_grand).Append("\n\n");
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_season_title)).Append("\n");
            desc.Append(L.playable_mode.tournament_rule_season).Append("\n\n");
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_buff_title)).Append("\n");
            desc.Append(L.playable_mode.tournament_rule_buff).Append("\n\n");
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_nerf_title)).Append("\n");
            desc.Append(L.playable_mode.tournament_rule_nerf).Append("\n\n");
            desc.Append(GetTitleHelp(L.playable_mode.tournament_rule_ban_title)).Append("\n");
            desc.Append(L.playable_mode.tournament_rule_ban);
            var properties = new PopupInfoWindowProperties(L.playable_mode.tournament_rules_title, desc.ToString());
            UIFrame.Instance.OpenWindow(ScreenIds.popup_info_big, properties);
        }

        private string GetTitleHelp(string titleDesc)
        {
            var formatTitle = $"<size=40><b>{titleDesc}</b></size>";
            return formatTitle;
        }

        private void HeroClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.tournament_lobby);
            DirectionGoTo.GotoHeroRoom();
        }

        private void ShopClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.tournament_lobby);
            ShopWindowProperties properties = new ShopWindowProperties(ShopTabId.Tournament);
            UIFrame.Instance.OpenWindow(ScreenIds.shop_scene, properties);
        }

        private void EnterClick()
        {
            this.lobby1.SetActive(false);
            this.lobby2.SetActive(true);
        }

        private void RewardClick()
        {
            if (this.iconNoticeReward.activeSelf)
            {
                UIFrame.Instance.OpenWindow(ScreenIds.popup_claim_reward_tournament);
            }
            else
            {
                UIFrame.Instance.OpenWindow(ScreenIds.popup_reward_tournament);
            }
        }

        private void LeaderboardClick()
        {
            var dataLeaderboard = new LeaderboardArenaWindowProperties(this._currDataRanking.ListRankingSeasonData,
                this._currDataRanking.ListRankingTopPlayerData, this._currDataRanking.MyRankingSeasonData,
                this._currDataRanking.MyRankingTopPlayerData);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_leaderboard_top_player, dataLeaderboard);
        }

        private void CloseClick()
        {
            if (this.lobby1.activeSelf && !this.lobby2.activeSelf)
            {
                UIFrame.Instance.CloseWindow(ScreenIds.tournament_lobby);
                var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                UIFrame.Instance.OpenWindow(ScreenIds.home);
            }
            else if (!this.lobby1.activeSelf && this.lobby2.activeSelf)
            {
                this.lobby1.SetActive(true);
                this.lobby2.SetActive(false);
            }
        }

        private void ChangeBuffClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_change_buff);
        }

        private void ShowUi()
        {
            this.lobby1.SetActive(true);
            this.lobby2.SetActive(false);
            RepaintSeasonTime();
            this.timeTicketUi.ShowUi();
            ShowRank();
            this.iconNoticeReward.SetActive(!UserData.Instance.TournamentData.isClaimedReward);
        }

        private void ShowRank()
        {
            var rank = UserData.Instance.TournamentData.currRank;
            var rankId = Ultilities.GetRankTournamentId(rank);
            if (UserData.Instance.TournamentData.currRank >= 0)
            {
                this.rankIcon.sprite = ResourceUtils.GetRankIconLargeTournament(rankId);
                this.rankIcon.SetNativeSize();
                this.rankNameTxt.text = Localization.Current.Get("playable_mode", $"tournament_rank_{rankId}");
                this.rankTxt.text = $"{L.playable_mode.rank_txt} {rank}";
                this.rankIcon.gameObject.SetActive(true);
                this.rankNameTxt.gameObject.SetActive(true);
                this.rankTxt.gameObject.SetActive(true);
                this.rankEmptyTxt.gameObject.SetActive(false);
            }
            else
            {
                this.rankIcon.gameObject.SetActive(false);
                this.rankNameTxt.gameObject.SetActive(false);
                this.rankTxt.gameObject.SetActive(false);
                this.rankEmptyTxt.gameObject.SetActive(true);
            }
        }

        #region Server

        private void PrepareData()
        {
            if (this._zitgaTournament == null)
            {
                this._zitgaTournament = new ZitgaTournament();
            }

            if (this._zitgaGameEvent == null)
            {
                this._zitgaGameEvent = new ZitgaGameEvent();
            }

            this._currentPlatform = Application.platform;
            switch (_currentPlatform)
            {
                case RuntimePlatform.Android:
                    _authProvider = AuthProvider.ANDROID_DEVICE;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _authProvider = AuthProvider.IOS_DEVICE;
                    break;
            }

            this._userId = UserData.Instance.AccountData.tokenId;

            this._zitgaTournament.OnLoadResult = OnLoadRankingResult;
            this._zitgaGameEvent.OnLoadResult = OnLoadResult;
            this._zitgaTournament.OnLoadInfoResult = OnLoadInfoResult;
        }

        private void GetDataRanking()
        {
            try
            {
                this._zitgaTournament.LoadRanking(_authProvider, this._userId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void GetInfoResetTournament()
        {
            try
            {
                this._zitgaGameEvent.GetEventData(_authProvider, _userId);
                this._zitgaTournament.LoadTournamentInfo(_authProvider, _userId);
                this._zitgaTournament.LoadRanking(_authProvider, this._userId);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private void OnLoadRankingResult(int logicCode, RankingOutbound dataRanking)
        {
            if (logicCode == LogicCode.SUCCESS && dataRanking != null)
            {
                this._currDataRanking = dataRanking;

                if (this._currDataRanking.MyRankingSeasonData != null)
                {
                    UserData.Instance.TournamentData.currRank = this._currDataRanking.MyRankingSeasonData.RankSeason;
                    UserData.Instance.TournamentData.isClaimedReward =
                        this._currDataRanking.MyRankingSeasonData.RewardStatus != 1;
                }
                else
                {
                    UserData.Instance.TournamentData.isClaimedReward = true;
                }

                ShowUi();
            }
            else
            {
                CloseClick();
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

        #endregion
    }
}