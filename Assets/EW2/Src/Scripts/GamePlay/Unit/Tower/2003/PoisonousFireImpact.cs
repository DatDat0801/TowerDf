using System;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class PoisonousFireImpact : DamageBox<Tower2003>
    {
        //private GetDamageCalculation damageCalculation;

        public void InitAOE(Tower2003 shooter)
        {
            owner = shooter;

            //damageCalculation = new GetDamageCalculation(owner);

            //Trigger(0.01f);
            TriggerDamage();
        }


        protected override bool CanGetDamage(Unit target)
        {
            return isAoe && target.IsAlive && ((EnemyBase) target).MoveType != MoveType.Fly;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                Debug.Log("Can't get damage");
                return null;
            }

            //var tower2003 = (Tower2003) owner;
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                showVfxNormalAtk = true,

                value = owner.towerData.BonusStat.level3Stat.poisonousFire
            };

            Debug.Log($"<color=red>Damage by poisonous Fire 2003: {damageInfo.value} on {damageInfo.target}<color>");

            return damageInfo;
        }

        public async void TriggerDamage()
        {
            float runTime = 0;
            var rate = owner.towerData.BonusStat.level3Stat.poisonousFireRate;
            var timeLife = owner.towerData.BonusStat.level3Stat.affectedTime;
            while (runTime < timeLife)
            {
                Trigger(0.01f);
                //Debug.LogAssertion("trigger damage poisonous fire");
                await UniTask.Delay(TimeSpan.FromSeconds(rate));
                runTime += rate;
            }

            if (transform.parent.gameObject)
                LeanPool.Despawn(transform.parent.gameObject);
        }
    }
}