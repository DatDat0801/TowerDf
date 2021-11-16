using System.Collections.Generic;
using EW2.CampaignInfo.HeroSelect;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class SelectHeroDefendModeWindow : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtTip;
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnAppy;
        [SerializeField] private GameObject btnAppyDisable;
        [SerializeField] private HeroDefenseBarController heroBar;
        [SerializeField] private HeroSelectedDefenseModeController heroSelectedBar;

        private List<HeroSelectedData> _datasPreview = new List<HeroSelectedData>();
        private HeroDefendModeConfig.DataConfig _dataConfig;

        protected override void Awake()
        {
            base.Awake();
            this.btnClose.onClick.AddListener(CloseClick);
            this.btnAppy.onClick.AddListener(ApplyFormation);
            this.heroBar.selectHeroDefendModeWindow = this;
            EventManager.StartListening(GamePlayEvent.OnHeroUnlocked, OnUnlockHero);
        }

        private void OnUnlockHero()
        {
            ShowUi();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            GetDataPreview();
            GetDataConfig();
            ShowUi();
        }

        private void GetDataConfig()
        {
            this._dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();
        }

        private void GetDataPreview()
        {
            this._datasPreview.Clear();
            this._datasPreview.AddRange(UserData.Instance.UserHeroDefenseData.listHeroSelected);
        }

        private void ShowUi()
        {
            txtTitle.text = L.playable_mode.hero_selection_txt;
            txtTip.text = L.playable_mode.hero_required_txt;
            this.btnAppy.GetComponentInChildren<Text>().text = L.button.btn_confirm;
            this.heroSelectedBar.ShowListHeroUsed();
            this.heroBar.SetInfo();
            ShowHideApplyButton();
        }

        private void ShowHideApplyButton()
        {
            var passCondition = this._datasPreview.Count >= this._dataConfig.numberHeroMinimum;
            this.btnAppy.gameObject.SetActive(passCondition);
            this.btnAppyDisable.SetActive(!passCondition);
            if (passCondition)
            {
                txtTip.color = Ultilities.GetColorFromHtmlString(GameConfig.TextColorBrown);
            }
            else
            {
                txtTip.color = Ultilities.GetColorFromHtmlString(GameConfig.TextColorRed);
            }
        }

        public bool CanAddHero()
        {
            return this._datasPreview.Count < GameConfig.MAX_SLOT_HERO_DEFENSE;
        }

        public void AddHero(HeroSelectedData data)
        {
            this._datasPreview.Add(data);
            heroSelectedBar.UpdateListHeroPreview(this._datasPreview);
            ShowHideApplyButton();
        }

        public void RemoveHero(int heroId)
        {
            for (int i = 0; i < _datasPreview.Count; i++)
            {
                var heroData = _datasPreview[i];
                if (heroData.heroId == heroId)
                {
                    this._datasPreview.RemoveAt(i);
                    heroSelectedBar.UpdateListHeroPreview(this._datasPreview);
                    ShowHideApplyButton();
                    return;
                }
            }
        }

        public bool IsHeroSelected(int heroId)
        {
            foreach (var heroData in this._datasPreview)
            {
                if (heroData.heroId == heroId)
                {
                    return true;
                }
            }

            return false;
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.hero_defend_select_hero);
        }

        private void ApplyFormation()
        {
            UserData.Instance.UserHeroDefenseData.listHeroSelected.Clear();
            UserData.Instance.UserHeroDefenseData.listHeroSelected.AddRange(this._datasPreview);
            UserData.Instance.Save();
            EventManager.EmitEvent(GamePlayEvent.OnRefreshFormationDefense);
            CloseClick();
        }
    }
}