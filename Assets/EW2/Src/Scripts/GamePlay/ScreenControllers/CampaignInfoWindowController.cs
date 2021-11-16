using System;
using System.Collections.Generic;
using EW2.CampaignInfo.HeroSelect;
using EW2.CampaignInfo.ModeSelect;
using EW2.CampaignInfo.PowerUpSelect;
using EW2.CampaignInfo.StageSelect;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    /// <summary>
    /// This is the Properties class for this specific window.
    /// It carries the payload which will be used to fill up this
    /// window upon opening.
    /// </summary>
    [Serializable]
    public class CampaignInfoWindowProperties : WindowProperties
    {
        public int campaignId;
        public bool isReplay;

        public CampaignInfoWindowProperties(int data, bool isReplay = false)
        {
            campaignId = data;
            this.isReplay = isReplay;
        }
    }

    public class CampaignInfoWindowController : AWindowController<CampaignInfoWindowProperties>
    {
        private enum ButtonState
        {
            Close,
            Start,
        }

        private enum SelectState
        {
            SelectStage,
            SelectHero,
            SelectPower
        }

        [SerializeField] private StageBarController stageBarController;
        [SerializeField] private PowerUpBarController powerUpBarController;
        [SerializeField] private HeroBarController heroBarController;

        [SerializeField] private SelectModeController selectModeController;
        [SerializeField] private HeroSelectedController heroSelectedController;

        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnStart;
        [SerializeField] private Text txtStamina;
        [SerializeField] private Text txtStageName;
        [SerializeField] private Text txtStart;
        [SerializeField] private Text txtCost;

        private MapCampaignInfo campaignInfo;

        private ButtonState buttonState;

        private SelectState selectState;

        private TrialHeroData trialHeroData;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(OnClose);

            btnStart.onClick.AddListener(OnStart);

            OutTransitionFinished += controller => {
                switch (buttonState)
                {
                    case ButtonState.Close:
                        if (GamePlayController.playMode == PlayMode.Campaign)
                            GamePlayController.playMode = PlayMode.None;
                        if (UIFrame.Instance.IsHome() && !UserData.Instance.BackUpData.isTriggeredPreregister)
                        {
                            UIFrame.Instance.OpenWindow(ScreenIds.pre_register);
                        }

                        break;
                }
            };
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.Money, MoneyType.Stamina), SetStamina);
                
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.Money, MoneyType.Stamina), SetStamina);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            if (trialHeroData == null)
                trialHeroData = GameContainer.Instance.Get<MapDataBase>().GetTrialHeroData();

            buttonState = ButtonState.Close;

            var campaignIdCompare = MapCampaignInfo.GetWorldMapModeId(Properties.campaignId);

            var userHeroData = UserData.Instance.UserHeroData;
            userHeroData.SetDefaultSelectedHeroes();

            BackupHeroSelected(campaignIdCompare.Item2, campaignIdCompare.Item3);

            SetCampaignInfo(Properties.campaignId);

            SetStageName();

            SetLocalization();

            stageBarController.SetScrollRect(campaignInfo.stageId);
            UpdateUI();

            if (!Properties.isReplay)
                selectModeController.InitializeMode();
            else
            {
                selectModeController.OnSelectMode(campaignIdCompare.Item3);
            }
        }

        public override void SetLocalization()
        {
            txtStart.text = L.button.btn_start.ToUpper();

            txtCost.text = L.stages.stamina_cost;
        }

        private void SetStageName()
        {
            txtStageName.text = Localization.Current.Get("stages",
                $"stage_name_{campaignInfo.worldId + 1}_{campaignInfo.stageId + 1}").ToUpper();
        }

        public void SetStage(int stageId)
        {
            Properties.campaignId = MapCampaignInfo.GetCampaignId(campaignInfo.worldId, stageId, campaignInfo.modeId);

            BackupHeroSelected(stageId, campaignInfo.modeId);

            SetCampaignInfo(Properties.campaignId);

            SetStageName();

            SetStamina();

            selectModeController.SetInfo(campaignInfo.worldId, campaignInfo.stageId, campaignInfo.modeId);

            heroBarController.SetInfo();
        }

        public void SetMode(int modeId)
        {
            Properties.campaignId = MapCampaignInfo.GetCampaignId(campaignInfo.worldId, campaignInfo.stageId, modeId);

            BackupHeroSelected(campaignInfo.stageId, modeId);

            SetCampaignInfo(Properties.campaignId);

            SetStageName();

            SetStamina();

            stageBarController.SetInfo(campaignInfo.worldId, campaignInfo.stageId, campaignInfo.modeId);
        }

        public bool CanAddHero()
        {
            return heroSelectedController.CanAddHero();
        }

        public void SetHero(HeroSelectedData data)
        {
            if (data.level > 0)
            {
                heroSelectedController.AddHero(data);
            }
            else
            {
                heroSelectedController.RemoveHero(data.heroId);
            }

            ConfirmHeroSelected();
        }

        public bool IsHeroSelected(int heroId)
        {
            var userHeroData = UserData.Instance.UserHeroData;
            return userHeroData.SelectedHeroes.Exists(data => data.heroId == heroId);
        }

        private void ConfirmHeroSelected()
        {
            var userHeroData = UserData.Instance.UserHeroData;
            userHeroData.SelectedHeroes.Clear();

            var data = this.heroSelectedController.GetHeroList();
            var newSlectedHeroes = new List<HeroSelectedData>();
            for (int i = 0; i < data.Count; i++)
            {
                if (!userHeroData.CheckHeroUnlocked(data[i])) continue;

                newSlectedHeroes.Add(new HeroSelectedData() { slot = i, heroId = data[i] });
            }

            //Save selected hero into user data
            userHeroData.SetDefaultSelectedHeroes(newSlectedHeroes);
            UserData.Instance.Save();
        }

        private void CancelHeroSelected()
        {
            var campaignIdCompare = MapCampaignInfo.GetWorldMapModeId(Properties.campaignId);

            BackupHeroSelected(campaignIdCompare.Item2, campaignIdCompare.Item3);

            heroBarController.SetInfo();
        }

        private void BackupHeroSelected(int mapId = -1, int mode = 0)
        {
            var userHeroData = UserData.Instance.UserHeroData;
            if (mapId < 0 || !CheckMapIsTrial(mapId, mode))
            {
                GamePlayController.IsTrialCampaign = false;
                heroSelectedController.SetInfo(userHeroData.SelectedHeroes);
            }
            else
            {
                var trial = trialHeroData.GetDataTrial(Properties.campaignId);
                if (userHeroData.CheckHeroUnlocked(trial.heroTrial))
                {
                    GamePlayController.IsTrialCampaign = false;
                    heroSelectedController.SetInfo(userHeroData.SelectedHeroes);
                }
                else
                {
                    var newListHero = new List<HeroSelectedData>();
                    newListHero.Add(
                        new HeroSelectedData() { level = trial.levelHero, slot = 0, heroId = trial.heroTrial });

                    if (userHeroData.SelectedHeroes.Count == 3)
                    {
                        userHeroData.SelectedHeroes.RemoveAt(2);
                    }

                    for (int i = 0; i < userHeroData.SelectedHeroes.Count; i++)
                    {
                        if (newListHero.Count == 3)
                            break;
                        var heroData = userHeroData.SelectedHeroes[i];
                        newListHero.Add(new HeroSelectedData() {
                            level = heroData.level,
                            slot = i + 1,
                            heroId = heroData.heroId
                        });
                    }

                    GamePlayController.IsTrialCampaign = true;
                    heroSelectedController.SetInfo(newListHero, GamePlayController.IsTrialCampaign);
                }
            }
        }

        public void OnClickPowerUpSelected()
        {
            SetSelectState(SelectState.SelectPower);
        }


        private void UpdateUI()
        {
            SetSelectState(SelectState.SelectStage);

            heroBarController.SetInfo();

            selectModeController.SetInfo(campaignInfo.worldId, campaignInfo.stageId, campaignInfo.modeId);

            stageBarController.SetInfo(campaignInfo.worldId, campaignInfo.stageId, campaignInfo.modeId);

            SetStamina();
        }

        private void SetSelectState(SelectState selectState)
        {
            this.selectState = selectState;

            stageBarController.gameObject.SetActive(selectState == SelectState.SelectStage);
            powerUpBarController.gameObject.SetActive(selectState == SelectState.SelectPower);
        }

        private void SetCampaignInfo(int campaignId)
        {
            campaignInfo = GameContainer.Instance.GetMapData(campaignId);
        }

        private void SetStamina()
        {
            var currStamina = UserData.Instance.GetMoney(MoneyType.Stamina);
            txtStamina.text = campaignInfo.mapStatBase.stamina.ToString();
            if (currStamina >= campaignInfo.mapStatBase.stamina)
            {
                txtStamina.color = Color.white;
            }
            else
            {
                txtStamina.color = Ultilities.GetColorFromHtmlString(GameConfig.TextColorRed);
            }
        }

        private void OnConfirm()
        {
            if (selectState == SelectState.SelectHero)
            {
                ConfirmHeroSelected();
            }
            else if (selectState == SelectState.SelectPower)
            {
            }

            SetSelectState(SelectState.SelectStage);
        }

        private void OnClose()
        {
            buttonState = ButtonState.Close;
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseWindow(ScreenIds.campaign_info);
        }

        private void OnStart()
        {
            //confirm sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            var userHeroData = UserData.Instance.UserHeroData;

            if (userHeroData.SelectedHeroes.Count == 0)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.need_hero_txt);
                return;
            }

            int cost = campaignInfo.mapStatBase.stamina;
            if (UserData.Instance.GetMoney(MoneyType.Stamina) < cost)
            {
                //var content = string.Format(L.popup.insufficient_resource, L.)
                //EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                string content = string.Format(L.popup.insufficient_resource, L.currency_type.currency_3) +" "+L.popup.get_more_txt;
                PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(L.popup.notice_txt,content,PopupNoticeWindowProperties.PopupType.TwoOption,L.button.btn_ok,()=>
                {
                    BuyStaminaWindowProperties propertiesStamina = new BuyStaminaWindowProperties();
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_stamina, propertiesStamina);
                },
                L.button.btn_no, null);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                return;
            }

            UserData.Instance.SubMoney(MoneyType.Stamina, cost, AnalyticsConstants.SourceCampaign, "", false);

            ConfigGamePlay();

            buttonState = ButtonState.Start;
            //tracking
            FirebaseLogic.Instance.SetStageStart(GamePlayController.CampaignId, GamePlayController.heroList);
            //set as played stage
            UserData.Instance.SetAsPlayed(Properties.campaignId);

            EventManager.EmitEventData(GamePlayEvent.OnPlayCampaign, GamePlayController.CampaignId);

            LoadSceneUtils.LoadScene(SceneName.GamePlay);
        }

        private void ConfigGamePlay()
        {
            GamePlayController.CampaignId = Properties.campaignId;

            GamePlayController.gameMode = GameMode.CampaignMode;

            GamePlayController.heroList.Clear();

            GamePlayController.heroList.AddRange(heroSelectedController.GetHeroList());
        }

        private bool CheckMapIsTrial(int mapId, int mode)
        {
            foreach (var trialData in trialHeroData.trialDatas)
            {
                if (trialData.stageUnlock == mapId && trialData.modeStage == mode)
                    return true;
            }

            return false;
        }

        private void ShowPopupInfo(ShopTabId money)
        {


        }
    }
}
