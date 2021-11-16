using System;
using UnityEngine;

namespace EW2
{
    public class Hero1005Passive1Impact : DamageBox<Hero1005>
    {
        private HeroData1005.PassiveSkill1 _passive1Data;
        public void Init()
        {
            this._passive1Data = this.owner.PassiveSkill1.SkillData;
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

            var damageInfo = new DamageInfo {
                creator = owner,
                damageType = this._passive1Data.damageType,
                showVfxNormalAtk = false,
                target = target,
                value = this._passive1Data.damage,
                isCritical = false
            };
            //Debug.LogAssertion(damageInfo.value + "," + damageInfo.target);
            // if (target is EnemyBase enemyBase)
            //     this.owner.AddEnemyToTrack(enemyBase);
            return damageInfo;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy is EnemyBase enemyBase)
                {
                    var hero5 = (Hero1005)owner;
                    hero5.AddEnemyToTrack(enemyBase);
                }
            }
        }
    }
}