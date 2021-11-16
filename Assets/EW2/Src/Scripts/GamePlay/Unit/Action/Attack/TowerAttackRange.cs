using System;
using UnityEngine;

namespace EW2
{
    public class TowerAttackRange
    {
        protected Unit owner;

        protected float totalTime;

        protected float timeTriggerAttack;

        protected AttackSpeed attackSpeed;

        protected Action doAction;

        public TowerAttackRange(Unit owner, Action action)
        {
            this.owner = owner;
            this.doAction = action;
            Init();
        }

        private void Init()
        {
            if (owner.Stats != null)
            {
                attackSpeed = owner.Stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed);

                attackSpeed.PropertyChanged += (sender, args) => { UpdateTimeTriggerAttack(); };
            }

            SetTimeTriggerAttack();
        }

        private void SetTimeTriggerAttack()
        {
            timeTriggerAttack = attackSpeed?.TimeTriggerAttack() ?? 0;
            ResetTime();
        }

        private void UpdateTimeTriggerAttack()
        {
            timeTriggerAttack = attackSpeed?.TimeTriggerAttack() ?? 0;
        }

        public void ResetTime()
        {
            totalTime = timeTriggerAttack;
        }

        public void Execute(float deltaTime)
        {
            totalTime += deltaTime;
            if (totalTime < timeTriggerAttack) return;

            doAction?.Invoke();
            totalTime = 0;
        }
    }
}