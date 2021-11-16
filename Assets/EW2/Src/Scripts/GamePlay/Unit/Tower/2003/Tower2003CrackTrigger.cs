using Invoke;
using UnityEngine;

namespace EW2
{
    public class Tower2003CrackTrigger : DamageBox<Tower2003>
    {
        private const float MaxSizeXBox = 5.5f;

        private const float Offset = 1.4f;

        private const float TimeDelay = 0.1f;

        private float damageSkill;

        public void InitTrigger(Tower2003 shooter, float damage)
        {
            this.damageSkill = damage;

            owner = shooter;

            collider2D.offset = Vector2.zero;

            collider2D.enabled = true;

            ((BoxCollider2D) collider2D).size = new Vector2(0.01f, 1f);

            InvokeProxy.Iinvoke.InvokeRepeating(this, IncreaseSixeBoxCollider, TimeDelay, TimeDelay);
        }

        private void IncreaseSixeBoxCollider()
        {
            if (((BoxCollider2D) collider2D).size.x >= MaxSizeXBox)
            {
                InvokeProxy.Iinvoke.CancelInvoke(this, IncreaseSixeBoxCollider);
                return;
            }


            collider2D.offset = new Vector2(collider2D.offset.x + Offset / 2f, collider2D.offset.y);

            ((BoxCollider2D) collider2D).size = new Vector2(((BoxCollider2D) collider2D).size.x + Offset, 1);
        }

        protected override bool CanGetDamage(Unit target)
        {
            return isAoe && target.IsAlive && ((EnemyBase) target).MoveType != MoveType.Fly;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                //Debug.Log("Can't get damage");
                return null;
            }

            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = damageSkill,

                showVfxNormalAtk = true
            };

            return damageInfo;
        }
    }
}