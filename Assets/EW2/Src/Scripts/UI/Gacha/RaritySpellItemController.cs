using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class RaritySpellItemController : MonoBehaviour
    {
        [SerializeField] private Image bgr;

        [SerializeField] private Image icon;

        [SerializeField] private Image border;

        public void SetData(SpellStatData spellData)
        {
            bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{spellData.rarity}");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{spellData.rarity}_rect");
            icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_{spellData.id}_0");
        }
    }
}