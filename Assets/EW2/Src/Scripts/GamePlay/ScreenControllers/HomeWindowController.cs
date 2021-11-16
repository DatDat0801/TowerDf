using System;
using BestHTTP.Connections.HTTP2;
using EW2.DailyCheckin;
using EW2.Events;
using EW2.Spell;
using Hellmade.Sound;
using Invoke;
using Sirenix.OdinInspector;
using SocialTD2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class HomeWindowController : AWindowController
    {
        [SerializeField] private Text[] titleZones;

        [SerializeField] private Button btnAvatar;

        [SerializeField] private Button btnShop;

        [SerializeField] private Button btnHeroRoom;

        [SerializeField] private Button btnUpgrade;

        [SerializeField] private Button btnMine;

        [SerializeField] private Button btnHeroTrial;

        [SerializeField] private Button btnArena;

        [SerializeField] private Button btnCampaign;

        [SerializeField] private Button btnStamina;

        [SerializeField] private Button btnStar;

        [SerializeField] private Button btnVip;

        [SerializeField] private Button btnGacha;

        [SerializeField] private Button btnQuest;

        [SerializeField] private Button btnDefendMode;


        [SerializeField] private GameObject panelStaminaInfo;

        [SerializeField] private GameObject panelMoreStar;

        [SerializeField] private Text txtName;

        [SerializeField] private Text txtVip;

        [SerializeField] private Image avatarIcon;

        [SerializeField, BoxGroup("More feature")]
        private Button checkInButton;

        [SerializeField, BoxGroup("More feature")]
        private Button wikiButton;

        [SerializeField, BoxGroup("More feature")]
        private Button adsSystemBtn;


        private bool isPlaySound;

        private const int LEVEL_NOT_SHOW_FIRST_PURCHASE = 15;

        /// <summary>
        /// show only one time when home show
        /// </summary>
        private bool shownThisTime;

        #region Cheat

        [SerializeField] private Button btnCheat;

        [SerializeField] private GameObject cheat;

        #endregion

        #region MonobehaviorMethod

        protected override void Awake()
        {
            SetListenButton();

            InTransitionFinished += controller => { EventManager.EmitEvent(GamePlayEvent.CloseLoading); };

            SetListenEvent();

            UpdataEventData();

            UpdateQuestData();

            if (UserData.Instance.OtherUserData.CheckCanResetNewDay())
            {
                HandleResetNewDay();
                EventManager.EmitEvent(GamePlayEvent.OnGameStart);
                UserData.Instance.OtherUserData.SetTimeResetNewDay();
                UserData.Instance.Save();
            }

            if (TimeManager.IsSyncedTimeWithServer || GameLaunch.isCheat)
                UserData.Instance.UserHeroDefenseData.CheckNewDay();
        }

        private void OnDisable()
        {
            isPlaySound = false;
        }

        #endregion

        private void LogOpenGame()
        {
            UserData.Instance.AccountData.LogOpenGame();
            UserData.Instance.Save();
        }

        private void EventStartScene()
        {
            if (GamePlayControllerBase.playMode != PlayMode.None && GamePlayControllerBase.gameMode == GameMode.CampaignMode)
            {
                switch (GamePlayControllerBase.playMode)
                {
                    case PlayMode.Campaign:
                        UIFrame.Instance.OpenWindow(ScreenIds.campaign_info,
                            new CampaignInfoWindowProperties(GamePlayControllerBase.CampaignId, false));
                        break;
                    case PlayMode.ReplayCampaign:
                        UIFrame.Instance.OpenWindow(ScreenIds.campaign_info,
                            new CampaignInfoWindowProperties(GamePlayControllerBase.CampaignId, true));
                        break;
                }

                if (GamePlayControllerBase.playMode != PlayMode.None)
                    GamePlayControllerBase.playMode = PlayMode.None;
            }
            else if (GamePlayControllerBase.gameMode == GameMode.DefenseMode || GamePlayControllerBase.gameMode == GameMode.TournamentMode)
            {
                var request = GamePlayControllerBase.RequestScreenOpenOnMenu;
                if (request != null)
                {
                    if (request.Properties != null)
                    {
                        if (!string.IsNullOrEmpty(request.ScreenId))
                            UIFrame.Instance.OpenWindow(request.ScreenId, request.Properties);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.ScreenId))
                            UIFrame.Instance.OpenWindow(request.ScreenId);
                    }

                    request.ExecuteRequest();
                }
            }
            else
            {
                //Only show if there is no window is open
                if (UIFrame.Instance.IsHome())
                {
                    if (!UserData.Instance.BackUpData.isTriggeredPreregister)
                    {
                        UIFrame.Instance.OpenWindow(ScreenIds.pre_register);
                    }
                    else if (UserData.Instance.BackUpData.isTriggeredPreregister &&
                             UserData.Instance.AccountData.idMapShowRating < 0)
                    {
                        InvokeProxy.Iinvoke.Invoke(this, ShowPopupPromotion, 0.5f);
                    }
                }
            }

            ShowPromotionWithoutCondition();

            CheckFlow();
        }

        private void CheckFlow()
        {
            var canShow = true;

            if (canShow)
            {
                var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                    .GetDataConfig();
                if (!UserData.Instance.UserHeroDefenseData.showFlowPopupOne &&
                    UserData.Instance.CampaignData.IsUnlockedStage(0, dataConfig.mapUnlock))
                {
                    canShow = false;
                    UserData.Instance.UserHeroDefenseData.showFlowPopupOne = true;
                    UserData.Instance.Save();
                    var properties = new PopupNoticeWindowProperties(L.popup.notice_txt, L.popup.new_mode_unlocked_txt,
                        PopupNoticeWindowProperties.PopupType.OneOption, L.button.go_to_btn,
                        () => {
                            DefendModeClick();
                        });
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                }
            }
            //Tournament
            if (canShow)
            {
                var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                    .GetDataConfig();
                if (!UserData.Instance.TournamentData.showUnlockPopup &&
                    UserData.Instance.CampaignData.IsUnlockedStage(0, dataConfig.mapUnlock))
                {
                    canShow = false;
                    UserData.Instance.TournamentData.showUnlockPopup = true;
                    UserData.Instance.Save();
                    var properties = new PopupNoticeWindowProperties(L.popup.notice_txt, L.popup.new_mode_unlocked_txt,
                        PopupNoticeWindowProperties.PopupType.OneOption, L.button.go_to_btn, ArenaOnClick);
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                }
            }

            if (canShow)
            {
                UserData userData = UserData.Instance;
                var numberStar =
                    UserData.Instance.CampaignData.GetStar(0, UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID);
                if (numberStar > 0 && userData.OtherUserData.UnlockSpellData.canShow)
                {
                    userData.OtherUserData.UnlockSpellData.UnlockFlowStep1();
                }

                numberStar =
                    UserData.Instance.CampaignData.GetStar(0, UnlockFeatureUtilities.RUNE_AVAILABLE_AT_STAGE_ID);
                if (numberStar > 0 && userData.OtherUserData.UnlockRuneData.canShow)
                {
                    userData.OtherUserData.UnlockRuneData.UnlockFlowStep1();
                }
            }
        }

        public override void HierarchyFixOnShow()
        {
            base.HierarchyFixOnShow();

            InvokeProxy.Iinvoke.Invoke(this, EventStartScene, 0.5f);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SetTitleZone();

            SetUiInfoPlayer();

            if (!isPlaySound)
            {
                EazySoundManager.PauseAllMusic();
                EazySoundManager.PlayMusic(ResourceSoundManager.GetAudioMusic(SoundConstant.MusicMenu),
                    EazySoundManager.GlobalMusicVolume, true, false);
                isPlaySound = true;
            }

            btnUpgrade.GetComponentInChildren<TowerUpgradeNotifyUI>().StarChanged(MoneyType.GoldStar);

            DailyCheckinWindowController.AutoCheckin();
            LogOpenGame();

            shownThisTime = false;

            if (cheat)
                cheat.SetActive(GameLaunch.isCheat);
        }


        private void SetTitleZone()
        {
            for (int i = 0; i < titleZones.Length; i++)
            {
                titleZones[i].text = Localization.Current.Get("worldmap", $"region_name_w1_{i}");
            }
        }

        private void SetUiInfoPlayer()
        {
            txtName.text = UserData.Instance.AccountData.userName;

            avatarIcon.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
        }

        private void SetListenEvent()
        {
            EventManager.StartListening(GamePlayEvent.OnChangeAvatarSuccess, ChangedAvatarSuccess);

            EventManager.StartListening(GamePlayEvent.OnChangeNameSuccess, ChangedNameSeccess);
        }

        private void SetListenButton()
        {
            btnAvatar.onClick.AddListener(AvatarOnClick);

            btnShop.onClick.AddListener(ShopOnClick);

            btnHeroRoom.onClick.AddListener(HeroRoomOnClick);

            btnUpgrade.onClick.AddListener(UpgradeOnClick);

            btnMine.onClick.AddListener(MineOnClick);

            btnHeroTrial.onClick.AddListener(HeroTrialOnClick);

            btnArena.onClick.AddListener(ArenaOnClick);

            btnCampaign.onClick.AddListener(CampaignOnClick);

            btnStamina.onClick.AddListener(ShowInfoStamina);

            btnStar.onClick.AddListener(ShowMoreStar);

            btnVip.onClick.AddListener(VipOnClick);

            btnGacha.onClick.AddListener(GachaOnClick);

            btnQuest.onClick.AddListener(QuestOnClick);

            btnDefendMode.onClick.AddListener(DefendModeClick);

            btnCheat.onClick.AddListener(CheatClick);
            checkInButton.onClick.AddListener(CheckInClick);
            wikiButton.onClick.AddListener(WikiClick);
            adsSystemBtn.onClick.AddListener(AdSystemClick);
        }

        private void DefendModeClick()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.check_internet_warning_txt);
                return;
            }

            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();
            if (!UserData.Instance.CampaignData.IsUnlockedStage(0, dataConfig.mapUnlock))
            {
                var str = string.Format(L.popup.spell_unlock_condition, (dataConfig.mapUnlock + 1).ToString());
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, str);
                return;
            }

            if (!LoadSaveUtilities.IsAuthenticated())
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.require_login_first);
                return;
            }

            UIFrame.Instance.OpenWindow(ScreenIds.hero_defend_mode_scene);
            FirebaseLogic.Instance.ButtonClick("main_menu", "hero_defense", 4);
        }


        void AdSystemClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.rewarded_ads);
        }

        void CheckInClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.daily_reward);
        }

        void WikiClick()
        {
            ShowComingSoon();
        }

        private void QuestOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.quest_scene);
        }

        private void GachaOnClick()
        {
            var campaignData = UserData.Instance.CampaignData;
            var unlockedStage = campaignData.HighestPassLevel();

            if (GameConfig.AVAILABLE_AT_STAGE_ID_UNLOCK_GACHA >= unlockedStage)
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    (GameConfig.AVAILABLE_AT_STAGE_ID_UNLOCK_GACHA + 1).ToString());
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            UIFrame.Instance.OpenWindow(ScreenIds.gacha_scene);
        }

        private void CheatClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_cheat);
        }

        private void VipOnClick()
        {
            ShowComingSoon();
        }

        private void AvatarOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_profile);
        }

        private void ShopOnClick()
        {
            ShopWindowProperties properties = new ShopWindowProperties(ShopTabId.Gem);
            UIFrame.Instance.OpenWindow(ScreenIds.shop_scene, properties);
        }

        private void HeroRoomOnClick()
        {
            var data = new HeroRoomWindowProperties((int)HeroType.Jave);

            UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene, data);
            FirebaseLogic.Instance.ButtonClick("main_menu", "heroes", 1);
        }

        private void UpgradeOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.tower_upgrade_system);
        }

        private void MineOnClick()
        {
            ShowComingSoon();
        }

        private void HeroTrialOnClick()
        {
            ShowComingSoon();
        }

        private void ArenaOnClick()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.check_internet_warning_txt);
                return;
            }

            var dataConfig = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentConfigData>()
                .GetTournamentConfig();
            if (!UserData.Instance.CampaignData.IsUnlockedStage(0, dataConfig.mapUnlock))
            {
                var str = string.Format(L.popup.spell_unlock_condition, (dataConfig.mapUnlock + 1).ToString());
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, str);
                return;
            }

            if (!LoadSaveUtilities.IsAuthenticated())
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.require_login_first);
                return;
            }


            UIFrame.Instance.OpenWindow(ScreenIds.tournament_lobby);
        }

        private void CampaignOnClick()
        {
            ShowComingSoon();
            //UIFrame.Instance.OpenWindow(ScreenIds.pre_register);
        }

        private void ShowInfoStamina()
        {
            panelStaminaInfo.SetActive(true);
        }

        private void ShowMoreStar()
        {
            panelMoreStar.SetActive(true);
        }

        private void ShowComingSoon()
        {
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
        }

        #region Listen Event Game

        private void ChangedNameSeccess()
        {
            txtName.text = UserData.Instance.AccountData.userName;
        }

        private void ChangedAvatarSuccess()
        {
            avatarIcon.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
        }

        private void ShowPopupPromotion()
        {
            var isOpen = true;

            // rating
            if (UserData.Instance.AccountData.idMapShowRating >= 0)
            {
                isOpen = false;
                RatingController.Instance.ShowPopupRating();
            }
            //

            if (!UserData.Instance.UserEventData.HeroAcademyUserData.CheckCanShow())
            {
                if (!UserData.Instance.UserEventData.HeroAcademyUserData.receivedExchange)
                {
                    var gloryRoadData = GameContainer.Instance.GetGloryRoadData();
                    var currGloryRoadPoint = UserData.Instance.GetMoney(MoneyType.GloryRoadPoint);
                    var gloryRoadPointRemain =
                        currGloryRoadPoint - gloryRoadData.items[gloryRoadData.items.Length - 1].point;

                    if (gloryRoadPointRemain > 0)
                    {
                        isOpen = false;
                        var heroAcademy = GameContainer.Instance.Get<EventDatabase>().Get<HeroAcademyCondition>();
                        if (heroAcademy)
                        {
                            var crystalExchange =
                                gloryRoadPointRemain * heroAcademy.packConditions[0].ratioExchangeMedal;
                            var detail = string.Format(L.game_event.medal_exchange_txt, gloryRoadPointRemain.ToString(),
                                crystalExchange.ToString());
                            var properties = new PopupNoticeWindowProperties(L.popup.notice_txt, detail,
                                PopupNoticeWindowProperties.PopupType.OneOption, L.button.btn_ok,
                                () => { HandleExchangeMedalHeroAcademy(crystalExchange); }, "",
                                () => { HandleExchangeMedalHeroAcademy(crystalExchange); });
                            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                        }
                    }
                }
            }

            // buy now
            if (UserData.Instance.UserEventData.BuyNowUserData.CheckCanShow() &&
                !UserData.Instance.UserEventData.BuyNowUserData.GetFirstOpen() && isOpen)
            {
                isOpen = false;
                UserData.Instance.UserEventData.BuyNowUserData.SetFirstOpen();
                UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
            }

            //rune package
            if (UserData.Instance.UserEventData.RunePackageUserData.CanAutoShow() && isOpen)
            {
                isOpen = false;
                UserData.Instance.UserEventData.RunePackageUserData.SetAutoShow();
                UserData.Instance.Save();
                UIFrame.Instance.OpenWindow(ScreenIds.rune_package);
            }

            //hero 4 resource flash sale
            if (UserData.Instance.UserEventData.CrystalFlashSaleUserData.CanAutoShow() && isOpen && !shownThisTime)
            {
                isOpen = false;
                shownThisTime = true;
                UIFrame.Instance.OpenWindow(ScreenIds.hero_4_resource_flash_sale);
            }

            //rune flash sale
            if (UserData.Instance.UserEventData.RuneFlashSaleUserData.CanAutoShow() && isOpen && !shownThisTime)
            {
                isOpen = false;
                shownThisTime = true;
                UIFrame.Instance.OpenWindow(ScreenIds.rune_flash_sale);
            }

            //spell flash sale
            if (UserData.Instance.UserEventData.SpellFlashSaleUserData.CanAutoShow() && isOpen && !shownThisTime)
            {
                isOpen = false;
                shownThisTime = true;
                UIFrame.Instance.OpenWindow(ScreenIds.spell_flash_sale);
            }

            //Spell Package 
            if (UserData.Instance.UserEventData.SpellPackageUserData.CanAutoShow() && isOpen && !shownThisTime)
            {
                isOpen = false;
                UserData.Instance.UserEventData.SpellPackageUserData.SetStopAutoShow();
                shownThisTime = true;
                UIFrame.Instance.OpenWindow(ScreenIds.popup_spell_package);
            }
        }


        private void ShowPromotionWithoutCondition()
        {
            var isOpen = true;

            if (UserData.Instance.AccountData.idMapShowRating >= 0)
            {
                isOpen = false;
                RatingController.Instance.ShowPopupRating();
            }

            var tuple = MapCampaignInfo.GetWorldMapModeId(GamePlayControllerBase.CampaignId);
            //hero 4 bundle
            if (UserData.Instance.UserEventData.Hero4BundleUserData.CanAutoShow(tuple.Item2 - 1) && isOpen)
            {
                isOpen = false;
                UserData.Instance.UserEventData.Hero4BundleUserData.SetReminded(tuple.Item2 - 1);
                UserData.Instance.Save();
                UIFrame.Instance.OpenWindow(ScreenIds.hero_4_bundle);
            }
        }

        #endregion

        #region Quest

        private void UpdateQuestData()
        {
            var dailyQuest = UserData.Instance.UserDailyQuest;
            dailyQuest.CalculateQuest();

            var achievementQuest = UserData.Instance.UserAchievementQuest;
            achievementQuest.CalculateQuest();
        }

        #endregion

        #region Event

        private void UpdataEventData()
        {
            if (UserData.Instance.UserEventData.HeroAcademyUserData.CheckCanShow())
            {
                GameContainer.Instance.Get<QuestManager>().LoadHeroAcademyCheckIn();
                GameContainer.Instance.Get<QuestManager>().LoadHeroChallengeQuest();
                UserData.Instance.UserEventData.HeroChallengeUserData.CalculateQuest();
            }

            if (UserData.Instance.UserEventData.NewHeroEventUserData.CheckCanShow())
            {
                GameContainer.Instance.Get<QuestManager>().LoadNewHeroEventQuest();
            }
        }

        private void HandleExchangeMedalHeroAcademy(long numberCrystal)
        {
            var reward = Reward.Create(ResourceType.Money, MoneyType.Crystal, (int)numberCrystal);
            if (reward != null)
            {
                reward.AddToUserData(true, AnalyticsConstants.SourceHeroAcademyEvent);
                UserData.Instance.UserEventData.HeroAcademyUserData.receivedExchange = true;
                LoadSaveUtilities.AutoSave(false);
            }
        }

        #endregion

        private void ResetAdsStamina()
        {
            UserData.Instance.UserEventData.ShopStaminaUserData.ResetAdsCount();
        }

        private void HandleResetNewDay()
        {
            ResetAdsStamina();
        }
    }
}