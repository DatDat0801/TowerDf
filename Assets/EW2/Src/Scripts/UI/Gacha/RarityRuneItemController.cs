using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class RarityRuneItemController : MonoBehaviour
    {
        [SerializeField] private Image bgr;

        [SerializeField] private Image icon;

        [SerializeField] private Image iconSymbolRune;

        [SerializeField] private Image border;

        public void SetData(RuneDataBase runeData)
        {
            bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{runeData.rarity}");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{runeData.rarity}_rect");
            iconSymbolRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_{runeData.runeId}");
            icon.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{runeData.rarity}");
        }
    }
}