using System;
using System.Text;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class RuneInfoWindowProperties : WindowProperties
    {
        public RuneItem runeItem;
        public int equipHero;
        public RuneTab inventory;

        public RuneInfoWindowProperties(RuneItem runeItem, RuneTab inventory, int currHeroId)
        {
            this.runeItem = runeItem;
            this.equipHero = currHeroId;
            this.inventory = inventory;
        }

        public void RemoveFromInventory()
        {
            inventory.RemoveFromInventory(runeItem);
            inventory.ShowContainer();
        }
    }

    public class RuneInfoWindowController : AWindowController<RuneInfoWindowProperties>
    {
        [SerializeField] private Text runeNameText;
        [SerializeField] private Text rarityNameText;
        [SerializeField] private Text txtLevel;
        [SerializeField] private Text txtLevelMax;
        [SerializeField] private Text txtLabelBaseAttribute;
        [SerializeField] private Text txtLabelBonusAttribute;
        [SerializeField] private Button dismantleButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unequipButton;
        [SerializeField] private Text txtTapToClose;

        //info
        [SerializeField] private ItemInfoBonusRune infoBonusSet2;
        [SerializeField] private ItemInfoBonusRune infoBonusSet4;
        [SerializeField] private ItemInfoBonusRune infoBonusSet6;

        //stat
        [SerializeField] private Text txtBaseAttribute;

        //rune item
        [SerializeField] private Image runeIcon;
        [SerializeField] private Image runeTypeIcon;
        [SerializeField] private Image frame;
        [SerializeField] private Image background;
        [SerializeField] private Image equippedHero;
        [SerializeField] private GameObject avatarHero;

        protected override void Awake()
        {
            base.Awake();
            dismantleButton.onClick.AddListener(DismantleClick);
            enhanceButton.onClick.AddListener(EnhanceClick);
            equipButton.onClick.AddListener(EquipClick);
            unequipButton.onClick.AddListener(UnequipClick);

            EventManager.StartListening(GamePlayEvent.OnEnhanceRune, OnEnhanceRune);
        }

        private void OnEnhanceRune()
        {
            Repaint();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            Repaint();
        }

        #region UI

        public void Repaint()
        {
            txtTapToClose.text = L.popup.tap_to_close;
            txtLabelBaseAttribute.text = L.popup.basic_attributes_txt;
            txtLabelBonusAttribute.text = L.popup.bonus_attributes_txt;
            dismantleButton.GetComponentInChildren<Text>().text = L.button.dismantle_btn;
            enhanceButton.GetComponentInChildren<Text>().text = L.button.enhance_btn;
            equipButton.GetComponentInChildren<Text>().text = L.button.equip_name;
            unequipButton.GetComponentInChildren<Text>().text = L.button.unequip_name;
            equipButton.gameObject.SetActive(Properties.runeItem.HeroIdEquip <= 0);
            unequipButton.gameObject.SetActive(Properties.runeItem.HeroIdEquip > 0);

            var selectedRune = Properties.runeItem;
            var dataCompare = InventoryDataBase.GetRuneId(selectedRune.RuneIdConvert);

            SetIconRune(dataCompare.Item1, dataCompare.Item2);

            ToggleDismantleDisable(!Properties.runeItem.IsEquipped());

            if (Properties.runeItem.IsEquipped())
            {
                equippedHero.sprite =
                    ResourceUtils.GetSpriteHeroIcon($"hero_icon_info_{Properties.runeItem.HeroIdEquip}");
                avatarHero.SetActive(true);
            }
            else
            {
                avatarHero.SetActive(false);
            }

            if (runeNameText != null)
            {
                runeNameText.text = Localization.Current.Get("rune", $"rune_name_{dataCompare.Item1.ToString()}");
                runeNameText.color = Ultilities.GetColorRarity(selectedRune.Rarity);
            }

            if (rarityNameText != null)
            {
                rarityNameText.text = Localization.Current.Get("gameplay", $"rarity_{selectedRune.Rarity.ToString()}");
                rarityNameText.color = Ultilities.GetColorRarity(selectedRune.Rarity);
            }

            if (txtLevel && txtLevelMax)
            {
                txtLevel.text = $"{L.gameplay.level} {selectedRune.Level}";
                txtLevelMax.text = $"/{selectedRune.GetMaxLevel()}";
            }

            var runeData = GameContainer.Instance.Get<InventoryDataBase>().GetRuneData(selectedRune.RuneIdConvert);
            var rune = runeData.GetRuneStatBaseByLevel(selectedRune.Level);
            if (txtBaseAttribute != null)
            {
                txtBaseAttribute.text = GetDescBaseAttribute(rune);
            }

            var runeUserData = UserData.Instance.GetInventory(InventoryType.Rune, selectedRune.ItemId);
            if (runeUserData != null)
            {
                var currSet = UserData.Instance.UserHeroData.GetHeroById(Properties.equipHero)
                    .GetRuneSetType((RuneItem) runeUserData);
                infoBonusSet2.SetInfo((int)RuneSet.RuneSet2, (RuneItem)runeUserData, currSet);
                infoBonusSet4.SetInfo((int)RuneSet.RuneSet4, (RuneItem)runeUserData, currSet);
                infoBonusSet6.SetInfo((int)RuneSet.RuneSet6, (RuneItem)runeUserData, currSet);
            }
        }

        private void SetIconRune(int runeId, int rarity)
        {
            background.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{rarity}");
            frame.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{rarity}_rect");
            runeTypeIcon.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_{runeId}");
            runeIcon.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{rarity}");
        }

        public void ToggleDismantleDisable(bool enable)
        {
            dismantleButton.interactable = enable;
        }

        #endregion


        void DismantleClick()
        {
            (int, int) rune = InventoryDataBase.GetRuneId(Properties.runeItem.RuneIdConvert);
            string content = string.Format(L.popup.dismantle_warning_txt_2,
                $"{Localization.Current.Get("rune", $"rune_name_{rune.Item1}")} ({L.gameplay.level} {Properties.runeItem.Level})");

            RuneDismantleDatabase dismantleDb = GameContainer.Instance.GetRuneDismantleDatabase();
            int runId = InventoryDataBase.GetRuneId(Properties.runeItem.RuneIdConvert).Item1;
            RuneDismantleData dismantleData = dismantleDb.GetDataBy(runId, Properties.runeItem.Rarity);
            int totalExp = dismantleData.expValue;
            if (Properties.runeItem.Level > 1)
            {
                RuneEnhanceData runeEnhanceData =  GameContainer.Instance.Get<InventoryDataBase>().Get<RuneEnhanceDatabase>()
                   .GetRuneEnhanceData(Properties.runeItem.Rarity);
                float refundRate = GameContainer.Instance.Get<InventoryDataBase>().Get<RuneDismantleRefundDatabase>().runeDismantleRefundData.refundRate;
                LevelEnhanceData levelEnhanceData = runeEnhanceData.GetLevelEnhanceData(Properties.runeItem.Level);
                float dustReqUpgraded = 0;
                for (int i = 1; i < Properties.runeItem.Level; i++)
                {
                    dustReqUpgraded += runeEnhanceData.GetLevelEnhanceData(i).dustReq;
                }

                totalExp += (int)(dustReqUpgraded * refundRate);
            }
            var properties = new DismantlePromptProperties(L.popup.notice_txt, content, Properties.runeItem,totalExp,
                PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_confirm, DoDismantle, L.button.btn_cancel,
                null);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_dismantle, properties);

            void DoDismantle()
            {
                if (Properties.runeItem.IsEquipped())
                {
                    return;
                }

                UIFrame.Instance.CloseCurrentWindow();

                var reward = new Reward()
                {
                    id = MoneyType.ExpRune,
                    type = ResourceType.Money,
                    number = totalExp,
                    itemType = InventoryType.None
                };
                Properties.RemoveFromInventory();
                PopupUtils.ShowReward(reward);
                //Add to user data
                Reward.AddToUserData(new[] { reward }, AnalyticsConstants.SourceDismantleRune, "", false);
            }
        }

        void EnhanceClick()
        {
            EnhanceWindowProperties data = new EnhanceWindowProperties(Properties.runeItem);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_enhance_rune, data);
        }

        void EquipClick()
        {
            var heroData = UserData.Instance.UserHeroData.GetHeroById(Properties.equipHero);

            if (!heroData.CanEquipRune())
            {
                var nameHero = Ultilities.GetNameHero(heroData.heroId);
                var content = string.Format(L.popup.equip_slot_full_txt, nameHero);
                Ultilities.ShowToastNoti(content);
                return;
            }

            heroData.EquipRune(Properties.runeItem);

            Properties.runeItem.HeroIdEquip = Properties.equipHero;

            UserData.Instance.UpdateInventory(InventoryType.Rune, Properties.runeItem,
                AnalyticsConstants.SourceHeroRoom, false);

            EventManager.EmitEventData(GamePlayEvent.OnRefreshHeroRoom, 0);
            EventManager.EmitEvent(GamePlayEvent.OnRefreshRune);
            EventManager.EmitEventData(GamePlayEvent.OnEquipRune, Properties.equipHero);

            Repaint();
            UIFrame.Instance.CloseWindow(ScreenIds.rune_inventory_info);
            if (Properties.runeItem.HeroIdEquip > 0)
                FirebaseLogic.Instance.LogHeroStatChange(Properties.runeItem.HeroIdEquip);
        }

        private void UnequipClick()
        {
            if (Properties.runeItem.HeroIdEquip > 0)
            {
                var heroIdCache = Properties.runeItem.HeroIdEquip;
                UserData.Instance.UserHeroData.GetHeroById(Properties.runeItem.HeroIdEquip)
                    .UnequipRune(Properties.runeItem);
                Properties.runeItem.UnequipRune();
                UserData.Instance.UpdateInventory(InventoryType.Rune, Properties.runeItem,
                    AnalyticsConstants.SourceHeroRoom, false);

                EventManager.EmitEventData(GamePlayEvent.OnRefreshHeroRoom, heroIdCache);
                EventManager.EmitEvent(GamePlayEvent.OnRefreshRune);
                Repaint();
                UIFrame.Instance.CloseWindow(ScreenIds.rune_inventory_info);
                if (heroIdCache > 0)
                    FirebaseLogic.Instance.LogHeroStatChange(heroIdCache);
            }
        }

        #region Handle Desc

        private string GetDescBaseAttribute(RuneStatBase data)
        {
            var textBuilder = new StringBuilder();
            if (data.ratioBonus > 0 && data.statApplyChance > 0)
            {
                var dataCompare = InventoryDataBase.GetRuneId(Properties.runeItem.RuneIdConvert);
                // rune special
                if (dataCompare.Item1 == (int)RuneId.ImmortalRune)
                {
                    var nameStatHpRegen = Ultilities.GetNameStat(RPGStatType.HpRegeneration);
                    var nameStatRespawn = Ultilities.GetNameStat(RPGStatType.TimeRevive);
                    var desc = L.rune.rune_set_basic_des_6;
                    var descPercent = L.rune.rune_set_basic_des_percent;
                    textBuilder
                        .Append(string.Format(descPercent, nameStatHpRegen,
                            $"<color={GameConfig.TextColorOrange}>{data.ratioBonus * 100}</color>"))
                        .Append("\n");
                    textBuilder.Append(string.Format(desc, nameStatRespawn,
                        $"<color={GameConfig.TextColorOrange}>{data.valueBonus}</color>"));
                }
                else if (dataCompare.Item1 == (int)RuneId.WisdomRune)
                {
                    textBuilder.Append(Ultilities.GetNameStat(data.statBonus));

                    textBuilder.Append($"<color={GameConfig.TextColorOrange}> -{data.ratioBonus * 100}%</color>");
                }
                else if (dataCompare.Item1 == (int)RuneId.ArgonyRune || dataCompare.Item1 == (int)RuneId.MiseryRune)
                {
                    var nameStat = Ultilities.GetNameStat(data.statApplyChance);
                    var desc = L.rune.bonus_damage_rune_des;
                    textBuilder.Append(string.Format(desc,
                        $"<color={GameConfig.TextColorOrange}>{data.ratioBonus * 100}</color>",
                        $"<color={GameConfig.TextColorOrange}>{data.valueBonus * 100}</color>",
                        nameStat));
                }
            }
            else if (data.ratioBonus > 0)
            {
                textBuilder.Append(Ultilities.GetNameStat(data.statBonus));

                textBuilder.Append($"<color={GameConfig.TextColorOrange}> +{data.ratioBonus * 100}%</color>");
            }
            else
            {
                textBuilder.Append(Ultilities.GetNameStat(data.statBonus));

                textBuilder.Append($"<color={GameConfig.TextColorOrange}> +{data.valueBonus}</color>");
            }


            return textBuilder.ToString();
        }

        #endregion
    }
}
