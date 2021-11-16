using System;
using Invoke;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Hero1002PassiveImpact : MonoBehaviour, IGetDamage
    {
        public ParticleSystem particleParent;
        private float damage;
        private Hero1002 owner;
        private HeroData1002.PassiveSkill3 dataSkill;


        public void Init(Hero1002 shooter, HeroData1002.PassiveSkill3 dataPassive3)
        {
            this.owner = shooter;
            this.dataSkill = dataPassive3;
            this.damage = owner.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue;
            InvokeProxy.Iinvoke.Invoke(this, HideEffect, 2f);
        }

        public DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo {
                creator = owner,
                damageType = owner.DamageType,
                value = damage * dataSkill.damageBaseOnAtk,
                target = target
            };

            return damageInfo;
        }

        void HideEffect()
        {
            particleParent.Simulate(0, true, true);

            LeanPool.Despawn(this);
        }

        private void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
        }
    }
}