using System.Collections.Generic;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Tower2003Skill2Impact : DamageBox<Tower2003>
    {
        // public Collider2D damageBoxAOE;

        public Tower2003SkillSlowTrigger slowTrigger;

        private int countTrigger;

        private TowerData2003.Skill2 dataSkill;

        public void InitImpact(Tower2003 shooter, TowerData2003.Skill2 data2003SkillActive)
        {
            dataSkill = data2003SkillActive;

            owner = shooter;
            slowTrigger.InitTrigger(shooter, dataSkill.time, dataSkill.slowPercent, dataSkill.modifierType);

            collider2D.enabled = false;

            InvokeProxy.Iinvoke.Invoke(this, TriggerDamageAOE, 0.2f);
        }

        private void TriggerDamageAOE()
        {
            countTrigger++;
            collider2D.enabled = true;
            InvokeProxy.Iinvoke.Invoke(this, () => { collider2D.enabled = false; }, 0.1f);
            if (countTrigger == 3)
            {
                InvokeProxy.Iinvoke.CancelInvoke(this, TriggerDamageAOE);
                return;
            }

            InvokeProxy.Iinvoke.Invoke(this, TriggerDamageAOE, 0.5f);
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

            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = dataSkill.damage,

                showVfxNormalAtk = true
            };

            return damageInfo;
        }
    }
}