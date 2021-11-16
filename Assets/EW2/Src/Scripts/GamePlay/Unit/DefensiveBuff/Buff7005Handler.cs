using UnityEngine;

namespace EW2
{
    public class Buff7005Handler : BuffHandler
    {
        public override void Buff(IBuffItem buffItem, int heroid, int stackNumber)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7005")
            {
                Buff7005 data = (Buff7005)buff.BuffData;
                HeroBase heroBase = GamePlayController.Instance.GetHeroes(heroid);
                CriticalBuff attackBuff = new CriticalBuff(heroBase);
                attackBuff.Buff(ModifierType.TotalAdd, data.GetFirst().increaseCritical * stackNumber);
                Debug.Log("Handle success 7005");
            }
            else
            {
                this._nextHandler.Buff(buffItem, heroid, stackNumber);
            }
        }

        public override void Buff(IBuffItem buffItem)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7005")
            {
                var data = (Buff7005)buff.BuffData;
                var heroes = GamePlayController.Instance.GetHeroes();
                foreach (HeroBase heroBase in heroes)
                {
                    var attackBuff = new CriticalBuff(heroBase);
                    attackBuff.Buff(ModifierType.TotalAdd, data.GetFirst().increaseCritical);
                }
            }
            else
            {
                this._nextHandler.Buff(buffItem);
            }
        }
    }
}