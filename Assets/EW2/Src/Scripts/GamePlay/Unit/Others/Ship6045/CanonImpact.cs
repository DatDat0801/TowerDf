using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class CanonImpact : DamageBox<Ship6045Controller>
    {
        private float damageAtk;

        public void InitAOE(Ship6045Controller shooter, float damage)
        {
            owner = shooter;

            damageAtk = damage;

            Trigger(0f, 0.2f);
        }

        protected override bool CanGetDamage(Unit target)
        {
            return isAoe;
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

                damageType = DamageType.Physical,

                value = damageAtk
            };

            return damageInfo;
        }

        private void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
        }
    }
}