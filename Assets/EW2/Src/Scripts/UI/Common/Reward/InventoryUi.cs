using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class InventoryUi : RewardUI
    {
        [SerializeField] protected Image bgr;

        [SerializeField] protected Image icon;

        [SerializeField] protected Image border;

        [SerializeField] protected GameObject iconFragment;

        [SerializeField] protected Text txtValue;

        [SerializeField] private Image iconRune;

        [SerializeField] protected Image iconSymbolRune;

        protected Reward data;

        public override void SetData<T>(T data)
        {
            this.data = data;
            SwitchIcon();
            UpdateUi();
        }

        protected override void UpdateUi()
        {
            iconFragment.SetActive(false);
            txtValue.gameObject.SetActive(data.number > 0);
            if (data.itemType == InventoryType.Spell)
            {
                var rarity = GetRaritySpell(data.id);
                bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{rarity}");
                border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{rarity}_rect");
                icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_{data.id}_0");
                txtValue.text = $"{data.number.ToString()}";
                iconFragment.SetActive(data.number > 1);
            }
            else if (data.itemType == InventoryType.SpellFragment)
            {
                bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{data.id}");
                border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{data.id}_rect");
                icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_fragment_{data.id}");
                txtValue.text = $"{data.number.ToString()}";
            }
            else if (data.itemType == InventoryType.Rune)
            {
                var dataCompare = InventoryDataBase.GetRuneId(data.id);
                bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{dataCompare.Item2}");
                border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{dataCompare.Item2}_rect");
                iconSymbolRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_{dataCompare.Item1}");
                iconRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{dataCompare.Item2}");
                txtValue.text = $"{data.number.ToString()}";
                SetPositonSymbolRune(dataCompare.Item2);
            }
            else if (data.itemType == InventoryType.RandomRune0 || data.itemType == InventoryType.RandomRune1 ||
                     data.itemType == InventoryType.RandomRune2 || data.itemType == InventoryType.RandomRune3 ||
                     data.itemType == InventoryType.RandomRune4)
            {
                var rarity = GetRarityRune();

                bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{rarity}");
                border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{rarity}_rect");
                iconSymbolRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_unknow");
                iconRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{rarity}");
                txtValue.text = $"{data.number.ToString()}";
                SetPositonSymbolRune(rarity);
            }
            else if (data.itemType == InventoryType.SpellSpecial)
            {
                bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_2");
                border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_2_rect");
                icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_random_spell");
                txtValue.text = $"{data.number.ToString()}";
            }
            else if (data.itemType == InventoryType.RandomSpell0 || data.itemType == InventoryType.RandomSpell1 ||
                     data.itemType == InventoryType.RandomSpell2 || data.itemType == InventoryType.RandomSpell3 ||
                     data.itemType == InventoryType.RandomSpell4)
            {
                var rarity = GetRaritySpell();

                bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{rarity.ToString()}");
                border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{rarity.ToString()}_rect");
                icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_random_spell_{rarity.ToString()}");
                txtValue.text = $"{data.number.ToString()}";
            }
        }

        protected override void ItemClick()
        {
            var data = new ItemInfoWindowProperties(this.data.type, this.data.id, this.data.itemType,
                this.data.number, iconFragment.activeSelf);
            UIFrame.Instance.OpenWindow(ScreenIds.item_info, data);
        }

        private int GetRaritySpell(int idSpell)
        {
            var db = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>().GetSpellDataBase(idSpell);
            if (db != null)
                return db.spellStatDatas.rarity;
            return 0;
        }

        private void SwitchIcon()
        {
            if (data.itemType == InventoryType.Rune || data.itemType == InventoryType.RandomRune0 ||
                data.itemType == InventoryType.RandomRune1 ||
                data.itemType == InventoryType.RandomRune2 || data.itemType == InventoryType.RandomRune3 ||
                data.itemType == InventoryType.RandomRune4)
            {
                icon.gameObject.SetActive(false);
                iconRune.gameObject.SetActive(true);
            }
            else
            {
                icon.gameObject.SetActive(true);
                iconRune.gameObject.SetActive(false);
            }
        }

        private int GetRarityRune()
        {
            var rarity = 0;

            switch (data.itemType)
            {
                case InventoryType.RandomRune0:
                    rarity = 0;
                    break;
                case InventoryType.RandomRune1:
                    rarity = 1;
                    break;
                case InventoryType.RandomRune2:
                    rarity = 2;
                    break;
                case InventoryType.RandomRune3:
                    rarity = 3;
                    break;
                case InventoryType.RandomRune4:
                    rarity = 4;
                    break;
            }

            return rarity;
        }

        private int GetRaritySpell()
        {
            var rarity = 0;

            switch (data.itemType)
            {
                case InventoryType.RandomSpell0:
                    rarity = 0;
                    break;
                case InventoryType.RandomSpell1:
                    rarity = 1;
                    break;
                case InventoryType.RandomSpell2:
                    rarity = 2;
                    break;
                case InventoryType.RandomSpell3:
                    rarity = 3;
                    break;
                case InventoryType.RandomSpell4:
                    rarity = 4;
                    break;
            }

            return rarity;
        }

        private void SetPositonSymbolRune(int rarity)
        {
            if (rarity == 0)
            {
                iconSymbolRune.rectTransform.anchoredPosition = new Vector2(1, 6);
            }
            else
            {
                iconSymbolRune.rectTransform.anchoredPosition = new Vector2(1, -3);
            }
        }
    }
}