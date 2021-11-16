using System;
using UnityEngine;

namespace EW2
{
    [Serializable]
    public class SkillEnemy
    {
        protected EnemyBase enemyBase;
        protected float timeCooldown;

        protected float lastTimeCastSkill = 0f;

        public virtual void CastSkill()
        {
            lastTimeCastSkill = Time.time;
        }

        public virtual void Init(EnemyBase enemyBase)
        {
            this.enemyBase = enemyBase;
        }

        public virtual bool CanCastSkill()
        {
            return Time.time - lastTimeCastSkill >= timeCooldown && enemyBase.IsAlive &&
                   enemyBase.StatusController.CanUseSkill() && this.enemyBase.UnitSpine.AnimationState.TimeScale > 0;
        }

        public virtual void StatCooldown()
        {
            lastTimeCastSkill = Time.time;
        }
    }
}