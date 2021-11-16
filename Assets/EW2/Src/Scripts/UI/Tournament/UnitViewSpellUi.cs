using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class UnitViewSpellUi : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image border;

        public void ShowUi(OptionShowInfoTournament option, int unitId)
        {
            var rarity = GetRaritySpell(unitId);
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{rarity}_rect");
            icon.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_{unitId}_0");
        }

        private int GetRaritySpell(int idSpell)
        {
            var db = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>().GetSpellDataBase(idSpell);
            if (db != null)
                return db.spellStatDatas.rarity;
            return 0;
        }
    }
}