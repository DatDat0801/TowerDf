using UnityEngine;

namespace EW2
{
    public class Buff7006Handler : BuffHandler
    {
        public override void Buff(IBuffItem buffItem, int heroid, int stackNumber)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7006")
            {
                Buff7006 data = (Buff7006)buff.BuffData;
                HeroBase heroBase = GamePlayController.Instance.GetHeroes(heroid);
                HealthPoint hp = heroBase.Stats.GetStat<HealthPoint>(RPGStatType.Health);
                bool currentIsFull = hp.IsFull;
                MaxHpBuff maxHpBuff = new MaxHpBuff(heroBase);
                maxHpBuff.Buff(ModifierType.TotalAdd, data.GetFirst().increaseMaxHp * stackNumber);

                if (currentIsFull)
                {
                    hp.CurrentValue += hp.CurrentValue;
                }
                Debug.Log("Handle success 7006");
            }
            else
            {
                this._nextHandler.Buff(buffItem, heroid, stackNumber);
            }
        }
        public override void Buff(IBuffItem buffItem)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7006")
            {
                var data = (Buff7006)buff.BuffData;

                var heroes = GamePlayController.Instance.GetHeroes();
                foreach (HeroBase heroBase in heroes)
                {
                    var hp = heroBase.Stats.GetStat<HealthPoint>(RPGStatType.Health);
                    var currentIsFull = hp.IsFull;
                    var maxHpBuff = new MaxHpBuff(heroBase);
                    maxHpBuff.Buff(ModifierType.TotalAdd, data.GetFirst().increaseMaxHp);

                    if (currentIsFull)
                    {
                        hp.CurrentValue += hp.CurrentValue;
                    }
                }
                Debug.Log("Handle success 7006");
            }
            else
            {
                this._nextHandler.Buff(buffItem);
            }
        }
    }
}
