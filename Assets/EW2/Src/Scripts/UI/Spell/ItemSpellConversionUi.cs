using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemSpellConversionUi : MonoBehaviour
    {
        [SerializeField] private Image bgr;

        [SerializeField] private Image icon;

        [SerializeField] private Image border;

        [SerializeField] private Text txtValue;

        private ItemInventoryBase data;
        public ItemInventoryBase ItemData => data;
        
        public void SetData(ItemInventoryBase dataItem)
        {
            data = dataItem;
            
            if (dataItem is SpellItem)
            {
                ShowUiSpell((SpellItem) dataItem);
            }
            else
            {
                ShowUiFragment((SpellFragmentItem) dataItem);
            }
        }

        private void ShowUiFragment(SpellFragmentItem dataFragment)
        {
            bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{dataFragment.ItemId}");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{dataFragment.ItemId}_rect");
            icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_fragment_{dataFragment.ItemId}");
        }

        private void ShowUiSpell(SpellItem dataSpell)
        {
            bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{dataSpell.Rarity}");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{dataSpell.Rarity}_rect");
            icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_{dataSpell.ItemId}_0");
        }

        public void UpdateValue(string value)
        {
            txtValue.text = value;
        }
    }
}