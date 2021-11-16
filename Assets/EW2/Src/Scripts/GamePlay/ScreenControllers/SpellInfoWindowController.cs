using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using EW2.Spell;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class SpellInfoWindowProperties : WindowProperties
    {
        public SpellItem spellItem;
        public int currHero;

        public SpellInfoWindowProperties(SpellItem spell, int currHeroId)
        {
            this.spellItem = spell;
            this.currHero = currHeroId;
        }
    }

    public class SpellInfoWindowController : AWindowController<SpellInfoWindowProperties>
    {
        [SerializeField] private Image iconSpell;
        [SerializeField] private Image border;
        [SerializeField] private Image iconCurrency;
        [SerializeField] private Text txtNameSpell;
        [SerializeField] private Text txtRarity;
        [SerializeField] private Text txtLevel;
        [SerializeField] private Text txtLevelMax;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private InfoSkillSpellController infoActive;
        [SerializeField] private InfoSkillSpellController infoPassive;
        [SerializeField] private Text txtLvlUp;
        [SerializeField] private Text txtPriceUpgrade;
        [SerializeField] private Text txtEquip;
        [SerializeField] private Text txtUnequip;
        [SerializeField] private Text txtFragment;
        [SerializeField] private Text txtLvlMax;
        [SerializeField] private Text txtTapToClose;
        [SerializeField] private Image progressFragment;
        [SerializeField] private Image iconUpgrade;
        [SerializeField] private Button btnUpgrade;
        [SerializeField] private Button btnEquip;
        [SerializeField] private Button btnUnequip;
        [SerializeField] private Button btnMaxLevel;
        [SerializeField] private Image avatarHeroUsed;
        [SerializeField] private GameObject avatarHeroPanel;
        [SerializeField] private Sprite imgProgressNormal;
        [SerializeField] private Sprite imgProgressFull;
        [SerializeField] private Sprite imgProgressLvlMax;
        [SerializeField] private Button btnFragmentConversion;
        [SerializeField] private GameObject btnFragmentConversionDisable;

        private SpellItem spellData;
        private SpellUpgradeData spellUpgradeData;

        protected override void Awake()
        {
            base.Awake();
            btnUpgrade.onClick.AddListener(UpgradeClick);
            btnEquip.onClick.AddListener(EquipClick);
            btnUnequip.onClick.AddListener(UnequipClick);
            btnFragmentConversion.onClick.AddListener(FragmentConversionClick);
            EventManager.StartListening(GamePlayEvent.OnConvertFragmentSpell, UpdateSpellInfo);
        }

        private void UpdateSpellInfo()
        {
            var data = EventManager.GetData<SpellItem>(GamePlayEvent.OnConvertFragmentSpell);
            if (data != null)
            {
                this.spellData = data;
                ShowUi();
            }
        }

        private void FragmentConversionClick()
        {
            var data = new SpellConversionWindowProperties(spellData);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_shard_conversion, data);
        }

        private void EquipClick()
        {
            var heroData = UserData.Instance.UserHeroData.GetHeroById(Properties.currHero);

            if (heroData.spellId > 0)
            {
                var oldSpell = heroData.spellId;
                var spellEquiped = (SpellItem) UserData.Instance.GetInventory(InventoryType.Spell, oldSpell);
                if (spellEquiped != null)
                {
                    spellEquiped.HeroIdEquip = -1;
                    UserData.Instance.UpdateInventory(InventoryType.Spell, spellEquiped,
                        AnalyticsConstants.SourceHeroRoom, false);
                }
            }

            UserData.Instance.UserHeroData.GetHeroById(Properties.currHero).EquipSpell(spellData.ItemId);
            spellData.HeroIdEquip = Properties.currHero;
            UserData.Instance.UpdateInventory(InventoryType.Spell, spellData, AnalyticsConstants.SourceHeroRoom, false);

            if (!UserData.Instance.OtherUserData.listHeroEquipedSpell.Contains(Properties.currHero))
            {
                UserData.Instance.OtherUserData.listHeroEquipedSpell.Add(Properties.currHero);
                EventManager.EmitEventData(GamePlayEvent.OnEquipSpell,Properties.currHero);
            }

            EventManager.EmitEventData(GamePlayEvent.OnRefreshHeroRoom,0);
            EventManager.EmitEvent(GamePlayEvent.OnRefreshSpell);
            ShowUi();
            UIFrame.Instance.CloseWindow(ScreenIds.popup_spell_info);
            FirebaseLogic.Instance.LogHeroStatChange(Properties.currHero);
        }

        private void UnequipClick()
        {
            if (spellData.HeroIdEquip > 0)
            {
                UserData.Instance.UserHeroData.GetHeroById(spellData.HeroIdEquip).UnequipSpell();
                spellData.HeroIdEquip = -1;
                UserData.Instance.UpdateInventory(InventoryType.Spell, spellData, AnalyticsConstants.SourceHeroRoom,
                    false);
            }

            EventManager.EmitEventData(GamePlayEvent.OnRefreshHeroRoom,0);
            EventManager.EmitEvent(GamePlayEvent.OnRefreshSpell);
            ShowUi();
            UIFrame.Instance.CloseWindow(ScreenIds.popup_spell_info);
            FirebaseLogic.Instance.LogHeroStatChange(Properties.currHero);
        }

        private void UpgradeClick()
        {
            var titleNoti = "";

            if (spellData.GetFragments() < spellUpgradeData.reqFragment &&
                UserData.Instance.GetMoney(MoneyType.Crystal) < spellUpgradeData.cost)
            {
                titleNoti = L.popup.insuficent_resource;
            }
            else if (spellData.GetFragments() < spellUpgradeData.reqFragment)
            {
                titleNoti = L.popup.insuficent_shade;
            }
            else if (UserData.Instance.GetMoney(MoneyType.Crystal) < spellUpgradeData.cost)
            {
                titleNoti = string.Format(L.popup.insufficient_resource, L.currency_type.currency_1);
            }

            if (titleNoti.Length > 0)
            {
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            spellData.Level++;
            spellData.Quantity -= spellUpgradeData.reqFragment;
            UserData.Instance.SubMoney(spellUpgradeData.moneyType, spellUpgradeData.cost,
                AnalyticsConstants.SourceSpellUpgrade ,spellData.ItemId.ToString(),
                false);
            UserData.Instance.UpdateInventory(InventoryType.Spell, spellData, AnalyticsConstants.SourceSpellUpgrade,
                false);
            this.spellUpgradeData = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                .GetSpellDataUpgrade(spellData.ItemId, spellData.Level + 1);
            ShowUi();
            FirebaseLogic.Instance.LogSpellLevelUp($"{spellUpgradeData.moneyType}", $"{spellData.ItemId}",
                $"{spellData.Level}");
            EventManager.EmitEvent(GamePlayEvent.OnRefreshSpell);
            EventManager.EmitEventData(GamePlayEvent.OnSpellUpgrade, spellData.ItemId);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            this.spellData = Properties.spellItem;
            this.spellUpgradeData = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                .GetSpellDataUpgrade(spellData.ItemId, spellData.Level + 1);

            ShowUi();

            scrollRect.content.anchoredPosition = Vector2.zero;
        }

        private void ShowUi()
        {
            txtTapToClose.text = L.popup.tap_to_close;
            iconSpell.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_{spellData.ItemId}_0");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{spellData.Rarity}_rect");

            txtNameSpell.text = Ultilities.GetNameSpell(spellData.ItemId);
            txtNameSpell.color = Ultilities.GetColorRarity(spellData.Rarity);

            var spellDataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();
            var levelMax = spellDataBases.GetSpellLevelMax(spellData.ItemId);

            txtRarity.text = Ultilities.GetRarity(spellData.Rarity);
            txtRarity.color = Ultilities.GetColorRarity(spellData.Rarity);

            txtLevel.text = $"{L.gameplay.level} {spellData.Level}";
            txtLevelMax.text = $"/{levelMax}";

            txtUnequip.text = L.button.unequip_name;
            txtEquip.text = L.button.equip_name;

            if (spellData.HeroIdEquip > 0)
            {
                avatarHeroUsed.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_info_{spellData.HeroIdEquip}");
                avatarHeroPanel.SetActive(true);
                btnEquip.gameObject.SetActive(false);
                btnUnequip.gameObject.SetActive(true);
            }
            else
            {
                avatarHeroPanel.SetActive(false);
                btnEquip.gameObject.SetActive(true);
                btnUnequip.gameObject.SetActive(false);
            }

            infoActive.SetData(spellData.ItemId, spellData.Rarity, Ultilities.GetNameSpell(spellData.ItemId),
                GetDescSkill(true), true);
            infoPassive.SetData(spellData.ItemId, spellData.Rarity, Ultilities.GetNameSpell(spellData.ItemId),
                GetDescSkill(false), false);

            txtLvlUp.text = L.button.btn_upgrade;

            txtPriceUpgrade.text = spellUpgradeData.cost.ToString();
            iconCurrency.sprite = ResourceUtils.GetIconMoney(spellUpgradeData.moneyType);

            if (spellData.Level < levelMax)
            {
                var percent = spellData.GetFragments() * 1f / spellUpgradeData.reqFragment * 1f;
                if (percent < 1)
                {
                    if (UserData.Instance.OtherUserData.IsSpellCanUpgrade(spellData.ItemId))
                    {
                        UserData.Instance.OtherUserData.RemoveSpellCanUpgrade(spellData.ItemId);
                        UserData.Instance.Save();
                        EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                    }

                    progressFragment.sprite = imgProgressNormal;
                }
                else
                {
                    progressFragment.sprite = imgProgressFull;
                }

                progressFragment.fillAmount = percent;
                txtFragment.text = $"{spellData.GetFragments()}/{spellUpgradeData.reqFragment}";
                btnMaxLevel.gameObject.SetActive(false);
                iconUpgrade.gameObject.SetActive(progressFragment.fillAmount >= 1);
                btnFragmentConversionDisable.SetActive(false);
                btnFragmentConversion.gameObject.SetActive(true);
            }
            else
            {
                txtFragment.text = L.popup.level_max_skill_txt;
                txtLvlMax.text = L.popup.level_max_skill_txt;
                progressFragment.sprite = imgProgressLvlMax;
                btnMaxLevel.gameObject.SetActive(true);
                iconUpgrade.gameObject.SetActive(false);
                btnFragmentConversionDisable.SetActive(true);
                btnFragmentConversion.gameObject.SetActive(false);

                if (UserData.Instance.OtherUserData.IsSpellCanUpgrade(spellData.ItemId))
                {
                    UserData.Instance.OtherUserData.RemoveSpellCanUpgrade(spellData.ItemId);
                    UserData.Instance.Save();
                    EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                }
            }
        }

        private string GetDescSkill(bool isActive)
        {
            var dbHero = GameContainer.Instance.Get<UnitDataBase>().GetSpellDataById(spellData.ItemId);

            var statDesc = new List<string>();
            if (isActive)
                statDesc = dbHero.GetDescStatSkillActive(spellData.Level);
            else
                statDesc = dbHero.GetDescStatSkillPassive(spellData.Level);

            var descLocalize = "";

            if (isActive)
                descLocalize = Ultilities.GetDescSpellSkillActive(spellData.ItemId);
            else
                descLocalize = Ultilities.GetDescSpellSkillPassive(spellData.ItemId);

            var desc = HandleStringDescSkill(descLocalize, statDesc);

            return desc;
        }

        private string HandleStringDescSkill(string desc, List<string> listStat)
        {
            var descConvert = desc;

            for (int i = 0; i < listStat.Count; i++)
            {
                descConvert = descConvert.Replace("{" + i + "}", listStat[i]);
            }

            return descConvert;
        }
    }
}