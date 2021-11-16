using Hellmade.Sound;
using UnityEngine;

namespace EW2
{
    public class Tower2002Skill1DamageBox : DamageBox<Tower2002>
    {
        private float damage;

        private Unit target;

        public void InitDamage(float damage, Tower2002 creator, Unit target)
        {
            this.damage = damage;

            this.target = target;

            this.owner = creator;
            
            Trigger();
        }

        protected override bool CanGetDamage(Unit target)
        {
            return isAoe || this.target == target;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                //Debug.Log("Can't get damage");
                return null;
            }
            
            EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(this.owner.Id,SoundConstant.Tower2002ImpactSkill1), EazySoundManager.GlobalSoundsVolume);
            
            var damageInfo = new DamageInfo
            {
                creator = owner,
                
                damageType = owner.DamageType,
                
                value = this.damage
            };

            return damageInfo;
        }
    }
}