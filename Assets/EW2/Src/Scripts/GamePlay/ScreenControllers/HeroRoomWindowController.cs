using System;
using System.Collections.Generic;
using EW2.Spell;
using EW2.Tutorial.General;
using Invoke;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class HeroRoomWindowProperties : WindowProperties
    {
        public int heroIdSelect;
        public int subTab;
        public bool ignoreTutorial;

        public HeroRoomWindowProperties(int heroId, int subTabId = (int)HeroRoomTabId.Skill,
            bool ignoreTutorial = false)
        {
            this.heroIdSelect = heroId;
            this.subTab = subTabId;
            this.ignoreTutorial = ignoreTutorial;
            if (heroIdSelect == 0)
            {
                heroIdSelect = (int)HeroType.Jave;
            }

            //if (!TutorialManager.Instance.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_1))
            //{
            //    heroIdSelect = (int)HeroType.Jave;
            //    this.subTab = (int)HeroRoomTabId.Skill;
            //}
        }
    }

    public class HeroRoomWindowController : AWindowController<HeroRoomWindowProperties>, IUpdateTabBarChanged
    {
        [SerializeField] private Text titleHeroRoom;

        [SerializeField] private Button buttonClose;

        [SerializeField] private HeroListController heroList;

        [SerializeField] private HeroStatsInfoController heroInfoStat;

        [SerializeField] private HeroRuneInfoController heroRuneInfo;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private MoneyHeroRoomController _moneyHeroRoomController;


        [SerializeField] private List<TabContainer> tabContainers;
        
        private Dictionary<int, HeroCacheData> listHeroDataCache = new Dictionary<int, HeroCacheData>();

        private int currentTabId;

        private int currentHeroId;

        private bool isRefresh;

        protected override void Awake()
        {
            base.Awake();
            buttonClose.onClick.AddListener(OnClose);
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnHeroSelectChange, OnUpdateHeroSelectChange);

            EventManager.StartListening(GamePlayEvent.OnHeroUnlocked, OnHeroUnlocked);

            EventManager.StartListening(GamePlayEvent.OnHeroLevelUp, OnHeroLevelUp);

            EventManager.StartListening(GamePlayEvent.OnUpgradeSkillHero, OnSkillUpgrade);

            EventManager.StartListening(GamePlayEvent.OnResetAllSkill, OnResetAllSkillHero);

            EventManager.StartListening(GamePlayEvent.OnRefreshHeroRoom, OnRefreshHeroRoom);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnHeroSelectChange, OnUpdateHeroSelectChange);

            EventManager.StopListening(GamePlayEvent.OnHeroUnlocked, OnHeroUnlocked);

            EventManager.StopListening(GamePlayEvent.OnHeroLevelUp, OnHeroLevelUp);

            EventManager.StopListening(GamePlayEvent.OnUpgradeSkillHero, OnSkillUpgrade);

            EventManager.StopListening(GamePlayEvent.OnResetAllSkill, OnResetAllSkillHero);

            EventManager.StopListening(GamePlayEvent.OnRefreshHeroRoom, OnRefreshHeroRoom);
        }

        private void OnClose()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.hero_room_scene);
            UIFrame.Instance.OpenWindow(ScreenIds.home);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            InitData();
            currentTabId = Properties.subTab;
            CheckUpdateDataHero();
            heroList.SetData(listHeroDataCache);
            ShowInfoHero(Properties.heroIdSelect);
            titleHeroRoom.text = L.worldmap.name_hero.ToUpper();
            if (!tabsManager.IsInited)
            {
                tabsManager.InitTabManager(this, currentTabId);
            }
            else
            {
                tabsManager.SetSelected(currentTabId);
            }

            if (UserData.Instance.OtherUserData.UnlockSpellData.isInUnlockFlow)
            {
                UserData.Instance.OtherUserData.UnlockSpellData.FinishedOrEndedFlow();
            }

            if (UserData.Instance.OtherUserData.UnlockRuneData.isInUnlockFlow)
            {
                UserData.Instance.OtherUserData.UnlockRuneData.FinishedOrEndedFlow();
            }

            if (!TutorialManager.Instance.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_1) &&
                currentTabId == 0 && currentHeroId == (int)HeroType.Jave)
            {
                TutorialManager.Instance.ExecuteStepTutorial(AnyTutorialConstants.FOCUS_UPGRADE_LEVEL_HERO);
            }

            if (Properties.heroIdSelect == (int)HeroType.Neetan)
            {
                OpenIntroduceNeetanPopup();
            }
        }


        private void InitData()
        {
            listHeroDataCache.Clear();
            for (int i = 0; i < GameConfig.HeroCount; i++)
            {
                var heroId = 1001 + i;

                var heroDataCache = new HeroCacheData();

                heroDataCache.InitData(heroId);

                listHeroDataCache.Add(heroId, heroDataCache);
            }
        }

        private void CheckUpdateDataHero()
        {
            if (listHeroDataCache.Count == 0) return;
            foreach (var heroCache in listHeroDataCache.Values)
            {
                var heroId = heroCache.HeroId;
                if (!CheckSameData(heroCache.HeroItemData,
                    UserData.Instance.UserHeroData.GetHeroById(heroId)))
                {
                    heroCache.UpdateData();
                }
            }
        }

        private bool CheckSameData(HeroItem currData, HeroItem otherData)
        {
            return currData == otherData;
        }

        private void OnRefreshHeroRoom()
        {
            var heroId = EventManager.GetData<int>(GamePlayEvent.OnRefreshHeroRoom);
            if (heroId <= 0) heroId = Properties.heroIdSelect;
            listHeroDataCache[heroId].UpdateData();
            ShowInfoHero(currentHeroId, true);
        }

        private void OnUpdateHeroSelectChange()
        {
            var heroId = EventManager.GetData<int>(GamePlayEvent.OnHeroSelectChange);
            Properties.heroIdSelect = heroId;
            ShowInfoHero(Properties.heroIdSelect);
            tabsManager.Reload();
            OpenIntroduceNeetanPopup();
        }

        private void OpenIntroduceNeetanPopup()
        {
            if (Properties.heroIdSelect == (int)HeroType.Neetan
                && !UserData.Instance.OtherUserData.isSelectedNeetan
                && !UserData.Instance.UserHeroData.CheckHeroUnlocked((int)HeroType.Neetan)
                && UserData.Instance.UserEventData.HeroAcademyUserData.CheckCanShow()
            )
            {
                UserData.Instance.OtherUserData.SetFirstTimeSelectNeetan();
                string checkBtnLb = string.Format(L.button.check_btn, L.game_event.glory_road_title_txt);
                string content = string.Format(L.game_event.buy_now_direction_popup, L.game_event.glory_road_title_txt);
                string unlockLb = L.button.btn_unlock_hero;
                var bundleUnlock = ProductsManager.HeroShopProducts[(int)HeroType.Neetan];
                if (bundleUnlock != null)
                {
                    var price = ProductsManager.GetLocalPriceStringById(bundleUnlock.productId);

                    if (price.Equals("$0.01") || string.IsNullOrEmpty(price))
                    {
                        price = $" ${bundleUnlock.price}";
                    }

                    unlockLb += price;
                }

                PopupIntroduceNeetanProperties properties = new PopupIntroduceNeetanProperties(L.popup.notice_txt,
                    content, checkBtnLb, unlockLb,
                    () => {
                        UIFrame.Instance.CloseCurrentWindow();
                        DirectionGoTo.GoToGloryRoad();
                    },
                    () => {
                        ShopService.Instance.BuyHero(ProductsManager.HeroShopProducts[Properties.heroIdSelect],
                            UnlockHeroSuccess);
                    });
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_introduce_neetan, properties);
            }
        }

        private void UnlockHeroSuccess(bool sucess, Reward[] gifts)
        {
            if (sucess)
            {
                Ultilities.ShowToastNoti(string.Format(L.popup.hero_unlocked,
                    Ultilities.GetNameHero(Properties.heroIdSelect)));
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            }
        }

        private void ShowInfoHero(int heroId, bool isRefresh = false)
        {
            this.currentHeroId = heroId;
            this.isRefresh = isRefresh;

            heroList.SetHeroSelect(heroId);
            heroInfoStat.ShowInfoHero(listHeroDataCache[heroId], isRefresh);
            heroRuneInfo.ShowInfoRune(heroId, SlotRuneClick);

            if (this.isRefresh)
                tabsManager.Reload();
        }

        private void OnHeroLevelUp()
        {
            var heroId = EventManager.GetData<int>(GamePlayEvent.OnHeroLevelUp);
            UpdateInfoHero(heroId);
        }

        private void OnHeroUnlocked()
        {
            var heroId = EventManager.GetData<int>(GamePlayEvent.OnHeroUnlocked);

            var heroDataCache = new HeroCacheData();
            heroDataCache.InitData(heroId);
            listHeroDataCache[heroId] = heroDataCache;

            UpdateInfoHero(heroId);
        }

        private void OnSkillUpgrade()
        {
            listHeroDataCache[Properties.heroIdSelect].UpdateData();
            heroList.SetHeroSelect(Properties.heroIdSelect);
            heroInfoStat.ShowInfoHero(listHeroDataCache[Properties.heroIdSelect], true);
            FirebaseLogic.Instance.LogHeroStatChange(Properties.heroIdSelect);
        }


        private void UpdateInfoHero(int heroId)
        {
            if (listHeroDataCache.ContainsKey(heroId))
            {
                listHeroDataCache[heroId].UpdateData();
            }
            else
            {
                var heroDataCache = new HeroCacheData();

                heroDataCache.InitData(heroId);

                listHeroDataCache.Add(heroId, heroDataCache);
            }

            ShowInfoHero(heroId, true);
        }

        private void OnResetAllSkillHero()
        {
            var heroId = EventManager.GetData<int>(GamePlayEvent.OnResetAllSkill);
            listHeroDataCache[heroId].ResetStat();
            ShowInfoHero(heroId, true);
            FirebaseLogic.Instance.LogHeroStatChange(heroId);
        }

        public void OnTabBarChanged(int indexActive)
        {
            if (indexActive == (int)HeroRoomTabId.Spell && !UnlockFeatureUtilities.IsSpellAvailable())
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    (UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID + 1).ToString());
                Ultilities.ShowToastNoti(titleNoti);

                PlayerPrefs.SetInt("lock_spell_tab_click", 1);

                tabsManager.SetSelected(currentTabId);
                return;
            }

            if (indexActive == (int)HeroRoomTabId.Rune && !UnlockFeatureUtilities.IsRuneAvailable())
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    (UnlockFeatureUtilities.RUNE_AVAILABLE_AT_STAGE_ID + 1).ToString());
                Ultilities.ShowToastNoti(titleNoti);
                tabsManager.SetSelected(currentTabId);

                return;
            }

            int tempCurrentId = currentTabId;
            currentTabId = indexActive;

            if (currentTabId == (int)HeroRoomTabId.Skill)
            {
                var skillContent = (HeroSkillContentController)tabContainers[currentTabId];
                if (skillContent != null)
                    skillContent.ShowInfo(listHeroDataCache[currentHeroId], isRefresh);
            }
            else if (currentTabId == (int)HeroRoomTabId.Spell)
            {
                if (BadgeUI.firstOpenSpell > 0 || BadgeUI.openNewSpell)
                {
                    BadgeUI.firstOpenSpell = 0;
                    PlayerPrefs.SetInt("first_open_spell", 0);
                    BadgeUI.openNewSpell = false;
                    EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                }

                var spellContent = (HeroSpellController)tabContainers[currentTabId];
                if (spellContent != null)
                    spellContent.SetCurrHeroId(currentHeroId);
            }
            else if (currentTabId == (int)HeroRoomTabId.Rune)
            {
                var runeTab = (RuneTab)tabContainers[currentTabId];
                if (runeTab != null)
                    runeTab.SetCurrHeroId(currentHeroId);
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

            if (!TutorialManager.Instance.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_1) &&
                currentTabId != tempCurrentId && currentTabId == (int)HeroRoomTabId.Skill &&
                currentHeroId == (int)HeroType.Jave)
            {
                TutorialManager.Instance.ExecuteStepTutorial(AnyTutorialConstants.FOCUS_UPGRADE_LEVEL_HERO);
            }

            _moneyHeroRoomController.OnTabBarChanged(indexActive);
        }

        private void SlotRuneClick()
        {
            tabsManager.SetSelected((int)HeroRoomTabId.Rune);
        }
    }
}