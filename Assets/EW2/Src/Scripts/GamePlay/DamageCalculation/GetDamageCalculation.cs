using UnityEngine;

namespace EW2
{
    public class GetDamageCalculation
    {
        protected readonly Unit owner;

        protected readonly Damage damage;

        protected readonly CritChance critChance;

        protected readonly CritDamage critDamage;
        protected readonly CritDamageBonus _critDamageBonus;

        public GetDamageCalculation(Unit owner)
        {
            this.owner = owner;

            damage = owner.Stats.GetStat<Damage>(RPGStatType.Damage);

            critChance = owner.Stats.GetStat<CritChance>(RPGStatType.CritChance);

            critDamage = owner.Stats.GetStat<CritDamage>(RPGStatType.CritDamage);

            _critDamageBonus = owner.Stats.GetStat<CritDamageBonus>(RPGStatType.CritDamageBonus);
        }

        public virtual (float, bool) Calculate(Unit target)
        {
            var rate = Random.Range(0, 1.0f);

            if (rate < critChance.StatValue)
            {
                return (damage.StatValue * (1 + critDamage.StatValue) + this._critDamageBonus.StatValue, true);
            }

            return (damage.StatValue, false);
        }
    }
}