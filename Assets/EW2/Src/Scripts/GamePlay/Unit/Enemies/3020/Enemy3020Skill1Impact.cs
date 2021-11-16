using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Enemy3020Skill1Impact : DamageBox<Enemy3020>
    {
        [SerializeField] private Enemy3002Skill1Poison poison;

        private EnemyData3020.EnemyData3020Skill1 dataImpact;

        private void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this, EnablePoison);
        }

        public void InitImpact(Enemy3020 enemy3020, EnemyData3020.EnemyData3020Skill1 dataSkill)
        {
            owner = enemy3020;
            dataImpact = dataSkill;
            if (poison)
                poison.gameObject.SetActive(false);
            Trigger(0, 2.98f);
            InvokeProxy.Iinvoke.Invoke(this, EnablePoison, 3f);
        }

        protected override bool CanGetDamage(Unit target)
        {
            return target.IsAlive;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (!CanGetDamage(target))
            {
                return null;
            }

            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = dataImpact.damageType,

                value = dataImpact.damage,

                showVfxNormalAtk = true
            };

            return damageInfo;
        }

        private void EnablePoison()
        {
            if (poison)
            {
                poison.InitPoison(owner, dataImpact);
                poison.gameObject.SetActive(true);
            }
        }
    }
}