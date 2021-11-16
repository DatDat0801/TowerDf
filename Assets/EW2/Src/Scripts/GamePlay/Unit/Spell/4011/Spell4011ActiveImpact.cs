using System;
using System.Collections.Generic;
using EW2.Spell;
using UnityEngine;

namespace EW2
{
    public class Spell4011ActiveImpact : MonoBehaviour, IGetDamage
    {
        //private float _damage;
        private Totem _owner;
        //private SpellStatBase _dataSkill;
        private DamageOverTime _damageOverTime;
        private RPGStatCollection _stats;

        private Dictionary<EnemyBase, StatusOverTime> _affectedEnemies = new Dictionary<EnemyBase, StatusOverTime>();


        public void Init(Totem shooter, RPGStatCollection rpgStatCollection)
        {
            this._owner = shooter;
            //this._dataSkill = dataActiveSkill;
            this._stats = rpgStatCollection;
            this._owner.onDead = OnTotemDead;
        }

        public DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo {
                creator = this._owner,
                damageType = this._owner.DamageType,
                value =  this._stats.GetStat<Damage>(RPGStatType.Damage).StatValue,
                target = target
            };

            return damageInfo;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy != null && enemy.CanChooseIsTarget)
                {
                    if(enemy.MoveType == MoveType.Fly) return;
                    var damageOverTime = new DamageOverTime(new StatusOverTimeConfig() {
                        creator = this._owner,
                        owner = enemy,
                        lifeTime = float.MaxValue,
                        intervalTime = GameConfig.IntervalRegeneration,
                        delayTime = 0,
                        baseValue = this._stats.GetStat<Damage>(RPGStatType.Damage).StatValue//this._dataSkill.damage
                    });
                    enemy.StatusController.AddStatus(damageOverTime);
                    if (!this._affectedEnemies.ContainsKey(enemy))
                        this._affectedEnemies.Add(enemy, damageOverTime);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy != null)
                {
                    this._affectedEnemies.TryGetValue(enemy, out var damageOverTime);
                    enemy.StatusController.RemoveStatus(damageOverTime);
                    this._affectedEnemies.Remove(enemy);
                }
            }
        }

        private void OnTotemDead(Unit victim, Unit killer)
        {
            if (victim.Equals(this._owner))
            {
                foreach (KeyValuePair<EnemyBase, StatusOverTime> s in this._affectedEnemies)
                {
                    s.Key.StatusController.RemoveStatus(s.Value);
                }
            }
        }
    }
}