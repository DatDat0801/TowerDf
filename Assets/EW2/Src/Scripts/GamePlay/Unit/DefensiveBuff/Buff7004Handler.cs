using UnityEngine;

namespace EW2
{
    public class Buff7004Handler : BuffHandler
    {
        public override void Buff(IBuffItem buffItem, int heroid, int stackNumber)
        {
            if (stackNumber <= 0) return;
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7004")
            {
                var data = (Buff7004)buff.BuffData;
                var heroBase = GamePlayController.Instance.GetHeroes(heroid);
                var decreaseRatio = data.GetFirst().decreaseCooldown;
                var cooldownReduction = new BuffCooldownDecay(heroBase);
                cooldownReduction.Buff(ModifierType.TotalAdd, Mathf.Pow(1 - decreaseRatio, stackNumber));
            }
            else
            {
                this._nextHandler.Buff(buffItem, heroid, stackNumber);
            }
        }

        public override void Buff(IBuffItem buffItem)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7004")
            {
                var data = (Buff7004)buff.BuffData;
                var heroes = GamePlayController.Instance.GetHeroes();
                var decreaseRatio = data.GetFirst().decreaseCooldown;
                foreach (HeroBase heroBase in heroes)
                {
                    var coodown = heroBase.Stats.GetStat<SkillActiveCooldown>(RPGStatType.SkillActiveCooldown);
                    var reduction = heroBase.Stats.GetStat(RPGStatType.CooldownReduction).StatValue;
                    var previousCd = coodown.StatBaseValue - coodown.StatBaseValue * reduction;
                    var cooldownReduction = (previousCd * decreaseRatio) / coodown.StatBaseValue;
                    var buffCooldown = new BuffCooldownDecay(heroBase);
                    buffCooldown.Buff(ModifierType.TotalAdd, cooldownReduction);
                }
            }
            else
            {
                this._nextHandler.Buff(buffItem);
            }
        }
    }
}