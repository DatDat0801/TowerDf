using System;
using EW2.Spell;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class SpellConversionWindowProperties : WindowProperties
    {
        public SpellItem spellItem;

        public SpellConversionWindowProperties(SpellItem spell)
        {
            this.spellItem = spell;
        }
    }

    public class SpellConversionWindowController : AWindowController<SpellConversionWindowProperties>
    {
        [SerializeField] private Text title;
        [SerializeField] private Text descConversion;
        [SerializeField] private ItemSpellConversionUi itemFragmentUi;
        [SerializeField] private ItemSpellConversionUi itemSpellUi;
        [SerializeField] private Slider slider;
        [SerializeField] private Button btnConvert;
        [SerializeField] private GameObject btnConvertDisable;
        [SerializeField] private Button btnMinus;
        [SerializeField] private Button btnPlus;
        [SerializeField] private Button btnMin;
        [SerializeField] private Button btnMax;
        [SerializeField] private Button btnClose;

        private int numberFragment = 1;
        private SpellItem spellData;
        private ItemInventoryBase spellFragment;

        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(() => { UIFrame.Instance.CloseCurrentWindow(); });
            btnConvert.onClick.AddListener(ConvertClick);
            btnMin.onClick.AddListener(MinClick);
            btnMax.onClick.AddListener(MaxClick);
            btnMinus.onClick.AddListener(MinusClick);
            btnPlus.onClick.AddListener(PlusClick);
        }

        private void PlusClick()
        {
            if (numberFragment >= slider.maxValue || spellFragment.Quantity == 0) return;

            numberFragment++;
            slider.value = numberFragment;
            itemFragmentUi.UpdateValue($"{numberFragment}/{itemFragmentUi.ItemData.Quantity}");
            itemSpellUi.UpdateValue($"{GetNumberSpellConvert()}");
        }

        private void MinusClick()
        {
            if (numberFragment == slider.minValue || spellFragment.Quantity == 0) return;

            numberFragment--;
            slider.value = numberFragment;
            itemFragmentUi.UpdateValue($"{numberFragment}/{itemFragmentUi.ItemData.Quantity}");
            itemSpellUi.UpdateValue($"{GetNumberSpellConvert()}");
        }

        private void MaxClick()
        {
            if (spellFragment.Quantity == 0 || slider.maxValue <= 0) return;

            numberFragment = GetNumberMaxConvert();
            slider.value = numberFragment;
            itemFragmentUi.UpdateValue($"{numberFragment}/{itemFragmentUi.ItemData.Quantity}");
            itemSpellUi.UpdateValue($"{GetNumberSpellConvert()}");
        }

        private void MinClick()
        {
            if (spellFragment.Quantity == 0 || slider.minValue <= 0) return;

            numberFragment = 1;
            slider.value = numberFragment;
            itemFragmentUi.UpdateValue($"{numberFragment}/{itemFragmentUi.ItemData.Quantity}");
            itemSpellUi.UpdateValue($"{GetNumberSpellConvert()}");
        }

        private void ConvertClick()
        {
            if (numberFragment > 0)
            {
                spellData.Quantity += numberFragment;
                UserData.Instance.UpdateInventory(InventoryType.Spell, spellData, AnalyticsConstants.SourceHeroRoom,
                    false);
                spellFragment.Quantity -= numberFragment;
                UserData.Instance.UpdateInventory(InventoryType.SpellFragment, spellFragment,
                    AnalyticsConstants.SourceHeroRoom, false);
                ShowUi();
                EventManager.EmitEventData(GamePlayEvent.OnConvertFragmentSpell, spellData);
                EventManager.EmitEvent(GamePlayEvent.OnRefreshSpell);
            }
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            this.spellData = Properties.spellItem;
            ShowUi();
        }

        private void ShowUi()
        {
            title.text = L.gameplay.shard_conversion;

            var spellDataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();
            var fragmentUsed = spellDataBases.GetTotalFragmentUsed(spellData.ItemId, spellData.Level);
            var fragmentToMax = spellDataBases.GetTotalFragmentToLevelMax(spellData.ItemId) - fragmentUsed;

            if (spellData.GetFragments() >= fragmentToMax)
            {
                numberFragment = 0;
            }
            else
            {
                numberFragment = 1;
            }

            spellFragment =
                UserData.Instance.GetInventory(InventoryType.SpellFragment, spellData.Rarity);
            if (spellFragment == null)
                spellFragment = new SpellFragmentItem(spellData.Rarity, 0, InventoryType.SpellFragment);

            itemFragmentUi.SetData(spellFragment);

            itemFragmentUi.UpdateValue($"{numberFragment}/{spellFragment.Quantity}");

            itemSpellUi.SetData(spellData);

            if (spellFragment.Quantity > 0)
                itemSpellUi.UpdateValue($"{GetNumberSpellConvert()}");
            else
                itemSpellUi.UpdateValue("0");

            btnConvert.GetComponentInChildren<Text>().text = L.button.convert_btn;
            btnConvertDisable.GetComponentInChildren<Text>().text = L.button.convert_btn;

            btnConvertDisable.SetActive((spellFragment.Quantity <= 0) || (spellData.GetFragments() >= fragmentToMax));
            btnConvert.gameObject.SetActive((spellFragment.Quantity > 0) && (spellData.GetFragments() < fragmentToMax));

            if (spellFragment.Quantity <= 0 || (spellData.GetFragments() >= fragmentToMax))
            {
                slider.value = 0f;
                slider.minValue = 0;
                slider.maxValue = 0;
            }
            else
            {
                slider.value = numberFragment;
                slider.minValue = 1;
                slider.maxValue = spellFragment.Quantity;
            }

            descConversion.text = GetDesc();
        }

        public void SliderValueChange()
        {
            if (numberFragment != (int) slider.value)
            {
                numberFragment = (int) slider.value;
                itemFragmentUi.UpdateValue($"{numberFragment}/{itemFragmentUi.ItemData.Quantity}");
                itemSpellUi.UpdateValue($"{GetNumberSpellConvert()}");
            }
        }

        private int GetNumberSpellConvert()
        {
            var ratio = GameContainer.Instance.Get<InventoryDataBase>().Get<FragmentSpellConvert>()
                .GetRatioConvertById(spellData.Rarity);

            return numberFragment / ratio;
        }

        private string GetDesc()
        {
            string desc = "";
            desc = L.popup.spell_upgrade_require;
            var spellUpgradeData = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                .GetSpellDataUpgrade(spellData.ItemId, spellData.Level + 1);

            if (spellData.GetFragments() >= spellUpgradeData.reqFragment)
                desc = string.Format(desc, Ultilities.GetNameSpell(spellData.ItemId),
                    $"<color='#69ae10'>{spellData.GetFragments()}/{spellUpgradeData.reqFragment}</color>");
            else
                desc = string.Format(desc, Ultilities.GetNameSpell(spellData.ItemId),
                    $"<color='#e91b24'>{spellData.GetFragments()}/{spellUpgradeData.reqFragment}</color>");

            return desc;
        }

        private int GetNumberMaxConvert()
        {
            var number = 0;
            var dataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();

            var fragmentUsed = dataBases.GetTotalFragmentUsed(spellData.ItemId, spellData.Level);

            var fragmentToMax = dataBases.GetTotalFragmentToLevelMax(spellData.ItemId) -
                                (fragmentUsed + spellData.GetFragments());

            var ratio = GameContainer.Instance.Get<InventoryDataBase>().Get<FragmentSpellConvert>()
                .GetRatioConvertById(spellData.Rarity);

            var totalFragmentCanConvert = spellFragment.Quantity / ratio;

            if (totalFragmentCanConvert <= fragmentToMax)
                number = (int) totalFragmentCanConvert;
            else
                number = (int) fragmentToMax;

            return number;
        }
    }
}