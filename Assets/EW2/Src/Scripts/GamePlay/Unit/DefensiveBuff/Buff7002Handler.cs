using UnityEngine;

namespace EW2
{
    public class Buff7002Handler : BuffHandler
    {
        public override void Buff(IBuffItem buffItem, int heroid, int stackNumber)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7002")
            {
                Buff7002 data = (Buff7002)buff.BuffData;
                HeroBase heroBase = GamePlayController.Instance.GetHeroes(heroid);
                DefenseBuff attackBuff = new DefenseBuff(heroBase);
                attackBuff.Buff(ModifierType.TotalAdd, data.GetFirst().armor * stackNumber, data.GetFirst().magicResistance * stackNumber);
                Debug.Log("Handle success 7002");
            }
            else
            {
                this._nextHandler.Buff(buffItem, heroid, stackNumber);
            }
        }

        public override void Buff(IBuffItem buffItem)
        {
            var buff = (BuffItem)buffItem;
            if (buff.BuffData.buffId == "7002")
            {
                var data = (Buff7002)buff.BuffData;
                var heroes = GamePlayController.Instance.GetHeroes();
                foreach (HeroBase heroBase in heroes)
                {
                    var attackBuff = new DefenseBuff(heroBase);
                    attackBuff.Buff(ModifierType.TotalAdd, data.GetFirst().armor, data.GetFirst().magicResistance);
                }
                Debug.Log("Handle success 7002");
            }
            else
            {
                this._nextHandler.Buff(buffItem);
            }
        }
    }
}
