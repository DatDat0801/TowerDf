using System;
using UnityEngine;

namespace EW2
{
    public class Hero1005DamageBox : HeroNormalDamageBox
    {
        
        public override DamageInfo GetDamage(Unit target)
        {
            return base.GetDamage(target);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;

                if (enemy is EnemyBase enemyBase)
                {
                    if (CanGetDamage(enemy) == false)
                    {
                        return;
                    }
                    var hero5 = (Hero1005)owner;
                    hero5.AddEnemyToTrack(enemyBase);
                }
            }
        }
    }
}