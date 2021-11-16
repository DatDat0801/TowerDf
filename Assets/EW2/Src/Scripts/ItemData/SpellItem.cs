using EW2.Spell;

namespace EW2
{
    public class SpellItem : ItemInventoryBase
    {
        public int HeroIdEquip { get; set; }

        public SpellItem(int id, int quantity, int inventoryType, int level = 1) : base(id, quantity,
            inventoryType)
        {
            this.Level = level;
            this.Rarity = GetRaritySpell();
        }

        public long GetFragments()
        {
            return (Quantity - 1);
        }

        private int GetRaritySpell()
        {
            var db = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>().GetSpellDataBase(ItemId);
            if (db != null)
                return db.spellStatDatas.rarity;
            return 0;
        }

        public bool IsLevelMax()
        {
            var levelMax = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                .GetSpellLevelMax(ItemId);
            return Level >= levelMax;
        }
    }
}