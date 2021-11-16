using System;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class EnhanceWindowProperties : WindowProperties
    {
        public RuneItem runeItem;

        public EnhanceWindowProperties(RuneItem rune)
        {
            this.runeItem = rune;
        }
    }


    public class EnhanceRuneWindowController : AWindowController<EnhanceWindowProperties>
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtTapToClose;
        [SerializeField] private Text txtNumberDust;

        [SerializeField] private Text txtNumberCrystal;

        //rune icon
        [SerializeField] private Image bgr;
        [SerializeField] private Image border;
        [SerializeField] private Text txtLvl;
        [SerializeField] private Image iconRune;
        [SerializeField] private Image iconSymbolRune;

        [SerializeField] private Button enhanceButton;
        [SerializeField] private GameObject btnMaxLevel;

        [SerializeField] private List<EnhanceStatInfo> listStatInfo;

        private RuneStatBase dataBaseLvlCurr;
        private RuneStatBase dataBaseLvlNext;
        private RuneEnhanceData runeEnhanceData;
        private int dustReqUpgrade;
        private int crystalReqUpgrade;

        protected override void Awake()
        {
            base.Awake();
            enhanceButton.onClick.AddListener(EnhanceClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            ShowUi();
        }

        private void GetData()
        {
            var runeDataBase = GameContainer.Instance.Get<InventoryDataBase>()
                .GetRuneData(Properties.runeItem.RuneIdConvert);
            if (runeDataBase != null)
            {
                dataBaseLvlCurr = runeDataBase.GetRuneStatBaseByLevel(Properties.runeItem.Level);
                var lvlNext = Properties.runeItem.Level + 1;
                if (lvlNext > runeDataBase.runeStatBases.Length)
                {
                    lvlNext = runeDataBase.runeStatBases.Length;
                }

                dataBaseLvlNext = runeDataBase.GetRuneStatBaseByLevel(lvlNext);
            }

            runeEnhanceData = GameContainer.Instance.Get<InventoryDataBase>().Get<RuneEnhanceDatabase>()
                .GetRuneEnhanceData(Properties.runeItem.Rarity);

            if (runeEnhanceData != null)
            {
                var levelEnhanceData = runeEnhanceData.GetLevelEnhanceData(Properties.runeItem.Level);
                dustReqUpgrade = levelEnhanceData.dustReq;
                crystalReqUpgrade = levelEnhanceData.crystalReq;
            }
        }

        private void ShowUi()
        {
            txtTitle.text = L.button.enhance_btn.ToUpper();
            txtTapToClose.text = L.popup.tap_to_close;
            enhanceButton.GetComponentInChildren<Text>().text = L.button.enhance_btn;
            btnMaxLevel.GetComponentInChildren<Text>().text = L.button.level_max_btn;
            btnMaxLevel.SetActive(Properties.runeItem.IsLevelMax());
            GetData();
            RuneItemUi();
            ShowStats();
            ShowResource();
        }

        private void ShowStats()
        {
            var dataCompare = InventoryDataBase.GetRuneId(Properties.runeItem.RuneIdConvert);

            if (listStatInfo.Count > 0)
            {
                listStatInfo[0].SetData(L.gameplay.level, dataBaseLvlCurr.level.ToString(),
                    dataBaseLvlNext.level.ToString());

                if (dataCompare.Item1 == (int) RuneId.ImmortalRune)
                {
                    listStatInfo[1].SetData(Ultilities.GetNameStat(dataBaseLvlCurr.statBonus), $"{dataBaseLvlCurr.ratioBonus * 100f}%", $"{dataBaseLvlNext.ratioBonus * 100f}%");
                    listStatInfo[2].SetData(Ultilities.GetNameStat(dataBaseLvlCurr.statApplyChance), $"{dataBaseLvlCurr.valueBonus}", $"{dataBaseLvlNext.valueBonus}");
                    listStatInfo[2].gameObject.SetActive(true);
                }
                else
                {
                    var statCurr = dataBaseLvlCurr.ratioBonus > 0 ? $"{dataBaseLvlCurr.ratioBonus * 100f}%" : dataBaseLvlCurr.valueBonus.ToString();
                    var statNext = dataBaseLvlNext.ratioBonus > 0 ? $"{dataBaseLvlNext.ratioBonus * 100f}%" : dataBaseLvlNext.valueBonus.ToString();
                    listStatInfo[1].SetData(Ultilities.GetNameStat(dataBaseLvlCurr.statBonus), statCurr, statNext);
                    listStatInfo[2].gameObject.SetActive(false);
                }
            }
        }

        private void RuneItemUi()
        {
            var dataCompare = InventoryDataBase.GetRuneId(Properties.runeItem.RuneIdConvert);
            bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{dataCompare.Item2}");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{dataCompare.Item2}_rect");
            iconSymbolRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_{dataCompare.Item1}");
            iconRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{dataCompare.Item2}");
            txtLvl.text = $"Lv{Properties.runeItem.Level}";
        }

        private void ShowResource()
        {
            txtNumberDust.text = $"{dustReqUpgrade}/{UserData.Instance.GetMoney(MoneyType.ExpRune)}";
            txtNumberCrystal.text = $"{crystalReqUpgrade}/{UserData.Instance.GetMoney(MoneyType.Crystal)}";
        }

        private void EnhanceClick()
        {
            if (dustReqUpgrade > UserData.Instance.GetMoney(MoneyType.ExpRune) ||
                UserData.Instance.GetMoney(MoneyType.Crystal) < crystalReqUpgrade)
            {
                var titleNoti = L.popup.insuficent_resource;
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            Properties.runeItem.Level++;
            UserData.Instance.UpdateInventory(InventoryType.Rune, Properties.runeItem,
                AnalyticsConstants.SourceEnhanceRune, false);

            UserData.Instance.SubMoney(MoneyType.ExpRune, dustReqUpgrade, AnalyticsConstants.SourceEnhanceRune,
                Properties.runeItem.RuneIdConvert.ToString());
            UserData.Instance.SubMoney(MoneyType.Crystal, crystalReqUpgrade, AnalyticsConstants.SourceEnhanceRune, "");

            EventManager.EmitEventData(GamePlayEvent.OnRefreshHeroRoom, 0);
            EventManager.EmitEvent(GamePlayEvent.OnRefreshRune);
            EventManager.EmitEvent(GamePlayEvent.OnEnhanceRune);

            GetData();
            ShowUi();
            Ultilities.ShowToastNoti(L.popup.enhance_successful_txt);

            if (Properties.runeItem.HeroIdEquip > 0)
                FirebaseLogic.Instance.LogHeroStatChange(Properties.runeItem.HeroIdEquip);
        }
    }
}