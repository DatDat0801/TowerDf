using UnityEngine;

namespace EW2
{
    public class Buff7001Handler : BuffHandler
    {
        public override void Buff(IBuffItem buffItem, int heroid, int stackNumber)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7001")
            {
                Buff7001 data = (Buff7001)buff.BuffData;
                HeroBase heroBase = GamePlayController.Instance.GetHeroes(heroid);
                AttackBuff attackBuff = new AttackBuff(heroBase);
                attackBuff.Buff(ModifierType.TotalAdd, data.GetFirst().damage * stackNumber);
                Debug.Log("Handle success 7001");
            }
            else
            {
                this._nextHandler.Buff(buffItem, heroid, stackNumber);
            }
        }

        public override void Buff(IBuffItem buffItem)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7001")
            {
                var data = (Buff7001)buff.BuffData;
                var heroes = GamePlayController.Instance.GetHeroes();

                foreach (HeroBase heroBase in heroes)
                {
                    var attackBuff = new AttackBuff(heroBase);
                    attackBuff.Buff(ModifierType.TotalAdd, data.GetFirst().damage);
                }
                //Debug.Log($"after buff {heroes[0].Stats.GetStat<Damage>(RPGStatType.Damage).StatValue}");
                Debug.Log("Handle success 7001");
            }
            else
            {
                this._nextHandler.Buff(buffItem);
            }
        }
    }
}
