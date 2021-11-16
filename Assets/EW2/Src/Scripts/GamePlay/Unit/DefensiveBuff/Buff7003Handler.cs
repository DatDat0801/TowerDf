namespace EW2
{
    public class Buff7003Handler : BuffHandler
    {
        public override void Buff(IBuffItem buffItem, int heroid, int stackNumber)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7003")
            {
                var data = (Buff7003)buff.BuffData;
                HeroBase heroBase = GamePlayController.Instance.GetHeroes(heroid);
                if (heroBase)
                {
                    foreach (SpellEnhancement dataSpellEnhancement in data.spellEnhancements)
                    {
                        if (heroBase.Spell != null)
                            if (heroBase.Spell.Id == dataSpellEnhancement.spellId)
                            {
                                var attackBuff = new AttackBuff(heroBase.Spell);
                                attackBuff.Buff(ModifierType.TotalPercent, dataSpellEnhancement.increaseDamageRatio * stackNumber);
                            }
                    }
                }
            }
            else
            {
                this._nextHandler.Buff(buffItem, heroid, stackNumber);
            }
        }

        public override void Buff(IBuffItem buffItem)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7003")
            {
                var data = (Buff7003)buff.BuffData;
                var heroes = GamePlayController.Instance.GetHeroes();

                foreach (HeroBase heroBase in heroes)
                {
                    foreach (SpellEnhancement dataSpellEnhancement in data.spellEnhancements)
                    {
                        if (heroBase.Spell != null)
                            if (heroBase.Spell.Id == dataSpellEnhancement.spellId)
                            {
                                var attackBuff = new AttackBuff(heroBase.Spell);
                                attackBuff.Buff(ModifierType.TotalPercent, dataSpellEnhancement.increaseDamageRatio);
                            }
                    }
                }
            }
            else
            {
                this._nextHandler.Buff(buffItem);
            }
        }
    }
}
