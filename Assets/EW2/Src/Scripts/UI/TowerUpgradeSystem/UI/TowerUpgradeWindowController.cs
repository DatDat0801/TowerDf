using System;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class TowerUpgradeWindowController : AWindowController
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnInfo;
        [SerializeField] private Button btnUpgrade;
        [SerializeField] private Button btnReset;
        [SerializeField] private Button btnResetDisable;

        [SerializeField] private TowerSkillInfoUI infoUi;
        [SerializeField] private TowerSkillUI[] tower2001s;
        [SerializeField] private TowerSkillUI[] tower2002s;
        [SerializeField] private TowerSkillUI[] tower2003s;
        [SerializeField] private TowerSkillUI[] tower2004s;
        /// <para>
        /// star id
        /// </para>
        public static event UnityAction<int> OnStarChanged = delegate { };

        private void Start()
        {
            btnClose.onClick.AddListener(OnCloseClick);
            btnInfo.onClick.AddListener(OnInfoClick);
            btnUpgrade.onClick.AddListener(OnUpgradeClick);
            btnReset.onClick.AddListener(OnResetClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            Initialize();

            //select first item as default
            if (TowerSkillUI.CurrentSkillSelected == null)
            {
                tower2001s[0].SelectItem();
                UpdateUpgradeButtonState();
                infoUi.Repaint(2001, 1, MoneyType.SliverStar);
            }
            else
            {
                var levelIndex = TowerSkillUI.CurrentSkillSelected.currentSkillItem.towerLevel;
                infoUi.Repaint(TowerSkillUI.CurrentSkillSelected.currentSkillItem.towerId, levelIndex,
                    levelIndex > 3 ? MoneyType.GoldStar : MoneyType.SliverStar);
                UpdateUpgradeButtonState();
            }


            //init star
            StarChanged();
            RepaintResetBtn();
        }

        void RepaintResetBtn()
        {
            //init disable btn
            var canReset = UserData.Instance.UserTowerData.CanResetProgress();
            btnResetDisable.gameObject.SetActive(!canReset);
            btnReset.gameObject.SetActive(canReset);
        }

        public void UpdateUpgradeButtonState()
        {
            var selectedItem = TowerSkillUI.CurrentSkillSelected.currentSkillItem;
            if (TowerUpgradeTool.IsActivated(selectedItem.towerId, selectedItem.towerLevel))
            {
                infoUi.NoticeUpgradeActivated();
            }
            else
            {
                infoUi.NoNoticeUpgrade();
            }
        }

        public void Initialize()
        {
            var userTowerData = UserData.Instance.UserTowerData;
            //2001
            var tower2001Stat = userTowerData.GetTowerStat(2001);
            for (int i = 0; i < tower2001s.Length; i++)
            {
                tower2001s[i].Repaint(2001, i + 1, i <= 2 ? MoneyType.SliverStar : MoneyType.GoldStar);
                tower2001s[i].OnSelect = OnSelectItem;
                if (tower2001Stat != null)
                {
                    //not activated items
                    if (tower2001Stat.towerLevel < i + 1)
                    {
                        tower2001s[i].EnableIngredient(false);
                        tower2001s[i].EnableSkill(false);
                    } //activated items
                    else if (tower2001Stat.towerLevel >= i + 1)
                    {
                        tower2001s[i].DisableAllIngredient();
                        tower2001s[i].EnableSkill(true);
                    }

                    if (tower2001Stat.towerLevel == i + 1 && !tower2001s[i].IsUpgraded() &&
                        tower2001Stat.towerLevel < tower2001s.Length)
                    {
                        tower2001s[i].EnableIngredient(true);
                    }

                    // if (tower2001Stat.towerLevel == i + 2)
                    // {
                    //     tower2001s[i].DisableAllIngredient();
                    //     tower2001s[i].EnableSkill(false);
                    //     if (TowerUpgradeTool.IsEnoughIngredient(TowerUpgradeTool.GetNeededStarForLevel(2001, i + 2)))
                    //     {
                    //         tower2001s[i].EnableIngredient(true);
                    //     }
                    //     else
                    //     {
                    //         tower2001s[i].EnableIngredient(false);
                    //     }
                    // }
                }
                else
                {
                    tower2001s[i].EnableIngredient(i == 0);

                    tower2001s[i].EnableSkill(false);
                }
            }

            //2002
            var tower2002Stat = userTowerData.GetTowerStat(2002);
            for (int i = 0; i < tower2002s.Length; i++)
            {
                tower2002s[i].Repaint(2002, i + 1, i <= 2 ? MoneyType.SliverStar : MoneyType.GoldStar);
                tower2002s[i].OnSelect = OnSelectItem;
                if (tower2002Stat != null)
                {
                    //not activated items
                    if (tower2002Stat.towerLevel < i + 1)
                    {
                        tower2002s[i].EnableIngredient(false);
                        tower2002s[i].EnableSkill(false);
                    } //activated items
                    else if (tower2002Stat.towerLevel >= i + 1)
                    {
                        tower2002s[i].DisableAllIngredient();
                        tower2002s[i].EnableSkill(true);
                    }

                    if (tower2002Stat.towerLevel == i + 1 && !tower2002s[i].IsUpgraded() &&
                        tower2002Stat.towerLevel < tower2002s.Length)
                    {
                        tower2002s[i].EnableIngredient(true);
                    }
                }
                else
                {
                    tower2002s[i].EnableIngredient(i == 0);
                    tower2002s[i].EnableSkill(false);
                }
            }

            //2003
            var tower2003Stat = userTowerData.GetTowerStat(2003);
            for (int i = 0; i < tower2003s.Length; i++)
            {
                tower2003s[i].Repaint(2003, i + 1, i <= 2 ? MoneyType.SliverStar : MoneyType.GoldStar);
                tower2003s[i].OnSelect = OnSelectItem;
                if (tower2003Stat != null)
                {
                    //not activated items
                    if (tower2003Stat.towerLevel < i + 1)
                    {
                        tower2003s[i].EnableIngredient(false);
                        tower2003s[i].EnableSkill(false);
                    } //activated items
                    else if (tower2003Stat.towerLevel >= i + 1)
                    {
                        tower2003s[i].DisableAllIngredient();
                        tower2003s[i].EnableSkill(true);
                    }

                    if (tower2003Stat.towerLevel == i + 1 && !tower2003s[i].IsUpgraded() &&
                        tower2003Stat.towerLevel < tower2003s.Length)
                    {
                        tower2003s[i].EnableIngredient(true);
                    }
                }
                else
                {
                    tower2003s[i].EnableIngredient(i == 0);
                    tower2003s[i].EnableSkill(false);
                }
            }

            //2004
            var tower2004Stat = userTowerData.GetTowerStat(2004);
            for (int i = 0; i < tower2004s.Length; i++)
            {
                tower2004s[i].Repaint(2004, i + 1, i <= 2 ? MoneyType.SliverStar : MoneyType.GoldStar);
                tower2004s[i].OnSelect = OnSelectItem;
                if (tower2004Stat != null)
                {
                    //not activated items
                    if (tower2004Stat.towerLevel < i + 1)
                    {
                        tower2004s[i].EnableIngredient(false);
                        tower2004s[i].EnableSkill(false);
                    } //activated items
                    else if (tower2004Stat.towerLevel >= i + 1)
                    {
                        tower2004s[i].DisableAllIngredient();
                        tower2004s[i].EnableSkill(true);
                    }

                    if (tower2004Stat.towerLevel == i + 1 && !tower2004s[i].IsUpgraded() &&
                        tower2004Stat.towerLevel < tower2004s.Length)
                    {
                        tower2004s[i].EnableIngredient(true);
                    }
                }
                else
                {
                    tower2004s[i].EnableIngredient(i == 0);
                    tower2004s[i].EnableSkill(false);
                }
            }
        }

        void OnCloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        void OnUpgradeClick()
        {
            var selected = TowerSkillUI.CurrentSkillSelected;
            var userTowerData = UserData.Instance.UserTowerData;
            var currentLevel = userTowerData.GetCurrentLevel(selected.currentSkillItem.towerId);
            int starId = -1;
            if (currentLevel >= 3)
            {
                starId = MoneyType.GoldStar;
            }
            else
            {
                starId = MoneyType.SliverStar;
            }

            if (selected.currentSkillItem.towerLevel < currentLevel + 1)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.upgrade_condition_txt);
            }
            else if (selected.currentSkillItem.towerLevel == currentLevel + 1)
            {
                var starNeeded = TowerUpgradeTool.GetNeededStarForLevel(selected.currentSkillItem.towerId,
                    selected.currentSkillItem.towerLevel, starId);
                if (TowerUpgradeTool.IsEnoughIngredient(starNeeded, starId))
                {
                    UserData.Instance.SubMoney(starId, starNeeded,
                        AnalyticsConstants.SourceTowerUpgrade, selected.currentSkillItem.towerId.ToString(), false);
                    UserData.Instance.UpgradeTower(selected.currentSkillItem.towerId);

                    SelectNextItem();
                    Initialize();
                    var nextSkillItem = TowerSkillUI.CurrentSkillSelected.currentSkillItem;

                    infoUi.Repaint(nextSkillItem.towerId, nextSkillItem.towerLevel,
                        nextSkillItem.towerLevel > 3 ? MoneyType.GoldStar : MoneyType.SliverStar);
                    UpdateUpgradeButtonState();
                    StarChanged();

                    RepaintResetBtn();

                    EventManager.EmitEventData(GamePlayEvent.OnSpentStarCampaign, starId);
                }
                else
                {
                    if (starId == MoneyType.GoldStar)
                    {
                        EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                            string.Format(L.popup.insufficient_resource, L.currency_type.currency_9));
                    }
                    else
                    {
                        EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                            string.Format(L.popup.insufficient_resource, L.currency_type.currency_8));
                    }
                }
            }
            else
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.upgrade_condition_txt);
            }
        }

        void SelectNextItem()
        {
            var current = TowerSkillUI.CurrentSkillSelected.currentSkillItem;
            switch (current.towerId)
            {
                case 2001:
                    if (current.towerLevel < tower2001s.Length)
                    {
                        var selected = SelectItem(tower2001s[current.towerLevel]);

                        infoUi.Repaint(current.towerId, tower2001s[current.towerLevel].currentSkillItem.towerLevel,
                            selected.currentSkillItem.towerLevel <= 3 ? MoneyType.SliverStar : MoneyType.GoldStar);
                    }

                    break;
                case 2002:
                    if (current.towerLevel < tower2002s.Length)
                    {
                        var selected = SelectItem(tower2002s[current.towerLevel]);

                        infoUi.Repaint(current.towerId, tower2002s[current.towerLevel].currentSkillItem.towerLevel,
                            selected.currentSkillItem.towerLevel <= 3 ? MoneyType.SliverStar : MoneyType.GoldStar);
                    }

                    break;
                case 2003:
                    if (current.towerLevel < tower2003s.Length)
                    {
                        var selected = SelectItem(tower2003s[current.towerLevel]);

                        infoUi.Repaint(current.towerId, tower2003s[current.towerLevel].currentSkillItem.towerLevel,
                            selected.currentSkillItem.towerLevel <= 3 ? MoneyType.SliverStar : MoneyType.GoldStar);
                    }

                    break;
                case 2004:
                    if (current.towerLevel < tower2004s.Length)
                    {
                        var selected = SelectItem(tower2004s[current.towerLevel]);

                        infoUi.Repaint(current.towerId, tower2003s[current.towerLevel].currentSkillItem.towerLevel,
                            selected.currentSkillItem.towerLevel <= 3 ? MoneyType.SliverStar : MoneyType.GoldStar);
                    }

                    break;
                default:
                    break;
            }
        }

        void OnResetClick()
        {
            var data = GameContainer.Instance.GetTowerUpgradeData();
            var properties = new BuyConfirmWindowProperties(L.popup.notice_txt, L.popup.tower_reset_notice,
                data.resetCost.ToString(), L.button.btn_confirm, DoReset, L.button.btn_no);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_confirm, properties);
        }

        void DoReset()
        {
            var silverStarRefund = TowerUpgradeTool.GetConsumedStars(MoneyType.SliverStar);
            var goldenStarRefund = TowerUpgradeTool.GetConsumedStars(MoneyType.GoldStar);
            var data = GameContainer.Instance.GetTowerUpgradeData();
            var success = UserData.Instance.ResetUserTowerData(data.resetCost);

            if (success)
            {
                if (silverStarRefund > 0)
                    UserData.Instance.AddMoney(MoneyType.SliverStar, silverStarRefund,
                        AnalyticsConstants.SourceTowerUpgrade, "", false);
                if (goldenStarRefund > 0)
                    UserData.Instance.AddMoney(MoneyType.GoldStar, goldenStarRefund,
                        AnalyticsConstants.SourceTowerUpgrade, string.Empty, false);
                Initialize();
                UpdateUpgradeButtonState();
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.reset_successful);
                StarChanged();
                RepaintResetBtn();
            }
            else
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                    string.Format(L.popup.insufficient_resource, L.currency_type.currency_0));
            }
        }

        /// <summary>
        /// UI change on star change
        /// </summary>
        void StarChanged(bool notify = true)
        {
            var selected = TowerSkillUI.CurrentSkillSelected;
            int starId = -1;
            if (selected.currentSkillItem.towerLevel > 3)
            {
                starId = MoneyType.GoldStar;
            }
            else
            {
                starId = MoneyType.SliverStar;
            }

            var remainingStar = TowerUpgradeTool.GetRemainingStar(starId);
            var neededStar = TowerUpgradeTool.GetNeededStar(starId);
            infoUi.RepaintStarQuantity(remainingStar, neededStar);
            OnStarChanged?.Invoke(starId);
        }

        void OnInfoClick()
        {
            infoUi.OpenDetailPanel();
        }

        void OnSelectItem(TowerSkillItem item)
        {
            // foreach (var towerSkillUi in tower2002s)
            // {
            //     towerSkillUi.DeselectItem();
            // }
            //
            // foreach (var towerSkillUi in tower2003s)
            // {
            //     towerSkillUi.DeselectItem();
            // }
            //
            // foreach (var towerSkillUi in tower2001s)
            // {
            //     towerSkillUi.DeselectItem();
            // }
            //
            // foreach (var towerSkillUi in tower2004s)
            // {
            //     towerSkillUi.DeselectItem();
            // }

            infoUi.Repaint(item.towerId, item.towerLevel,
                item.towerLevel <= 3 ? MoneyType.SliverStar : MoneyType.GoldStar);
            UpdateUpgradeButtonState();
            //TowerSkillUI.CurrentSkillSelected.SelectItem();
            SelectItem(TowerSkillUI.CurrentSkillSelected, false);
            infoUi.CloseDetailPanel();
        }

        TowerSkillUI SelectItem(TowerSkillUI skillUi, bool notify = true)
        {
            foreach (var towerSkillUi in tower2002s)
            {
                towerSkillUi.DeselectItem();
            }

            foreach (var towerSkillUi in tower2003s)
            {
                towerSkillUi.DeselectItem();
            }

            foreach (var towerSkillUi in tower2001s)
            {
                towerSkillUi.DeselectItem();
            }

            foreach (var towerSkillUi in tower2004s)
            {
                towerSkillUi.DeselectItem();
            }

            skillUi.SelectItem();
            StarChanged(notify);
            return skillUi;
        }
    }
}