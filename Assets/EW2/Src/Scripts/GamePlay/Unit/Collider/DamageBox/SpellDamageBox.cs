using EW2.Spell;
using UnityEngine;

namespace EW2
{
    public class SpellDamageBox : DamageBox<SpellUnitBase>
    {
        private DamageInfo customDamgeInfo;

        public void SetCustomDamgeInfo(DamageInfo damageInfo)
        {
            this.customDamgeInfo = damageInfo;
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

            var spell = (SpellUnitBase) owner;
            if (customDamgeInfo != null)
            {
                customDamgeInfo.target = target;
                return customDamgeInfo;
            }
            var damageInfo = new DamageInfo
            {
                creator = owner,
                damageType = owner.DamageType,
                showVfxNormalAtk = true,
                target = target,
                value = spell.SpellStatBase.damage,
                isCritical = false
            };

            return damageInfo;
        }
    }
}
