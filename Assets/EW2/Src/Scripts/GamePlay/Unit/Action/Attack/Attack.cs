using System;
using Cysharp.Threading.Tasks;
using Spine;
using UnityEngine;

namespace EW2
{
    public abstract class Attack
    {
        protected Unit owner;

        protected float totalTime;

        protected float timeTriggerAttack;

        protected float minTimeTriggerAttack;

        private AttackSpeed _attackSpeed;

        protected string animationName;

        public Action onComplete = delegate { };
        public Action onStart;

        public bool IsAttacking { get; private set; }

        public Attack(Unit owner)
        {
            this.owner = owner;
        }

        protected void Init()
        {
            minTimeTriggerAttack = SpineUtils.GetAnimationTime(owner.UnitSpine.AnimationState, animationName);

            if (owner.Stats != null)
            {
                this._attackSpeed = owner.Stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed);

                this._attackSpeed.PropertyChanged += (sender, args) => { SetTimeTriggerAttack(); };
            }

            SetTimeTriggerAttack();
        }

        public void UpdateTimeTrigger(string animName)
        {
            this._attackSpeed = owner.Stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed);
            minTimeTriggerAttack = SpineUtils.GetAnimationTime(owner.UnitSpine.AnimationState, animName);
            SetTimeTriggerAttack();
        }

        public void SetTimeTriggerAttack()
        {
            float animationTimeBySpeed = 0f;
            if (this._attackSpeed != null)
            {
                if (this._attackSpeed.StatValue > 1)
                {
                    animationTimeBySpeed = minTimeTriggerAttack / this._attackSpeed.StatValue;
                }
                else
                {
                    animationTimeBySpeed = minTimeTriggerAttack;
                }
            }

            timeTriggerAttack = Mathf.Max(animationTimeBySpeed > 0 ? animationTimeBySpeed : minTimeTriggerAttack,
                this._attackSpeed?.TimeTriggerAttack() ?? 0);

            // Debug.LogWarning($"{owner.name} attackSpeed {attackSpeed.StatValue}| animationTimeBySpeed {animationTimeBySpeed} | minTimeTriggerAttack {minTimeTriggerAttack} | timeTriggerAttack {timeTriggerAttack}");

            ResetTime();
        }

        public void ResetTime()
        {
            totalTime = timeTriggerAttack;
        }

        public void Execute(float deltaTime)
        {
            if (owner.IsAlive == false || owner.UnitState.IsStatusCC())
                return;

            totalTime += deltaTime;
            IsAttacking = true;
            if (totalTime < timeTriggerAttack)
            {
                IsAttacking = false;
                return;
            }

            var trackEntry = DoAnimation();

            onStart?.Invoke();

            if (trackEntry == null)
            {
                IsAttacking = false;
                return;
            }


            // Debug.Log($"{owner.name} Attack: total[{totalTime}] time[{timeTriggerAttack}]");

            totalTime = 0;

            trackEntry.Complete += OnComplete;

            if (trackEntry.AnimationTime > timeTriggerAttack)
            {
                IsAttacking = false;
                throw new Exception(
                    $"TimeTrigger({timeTriggerAttack}) can't be smaller than TimeAnimation({trackEntry.AnimationTime})");
            }
        }

        protected abstract TrackEntry DoAnimation();

        public virtual void OnComplete(TrackEntry track)
        {
            Dummy dummy = (Dummy)owner;
            IsAttacking = false;
            if (!dummy.IsAlive || dummy.UnitState.Current == ActionState.Move || dummy.UnitState.Current == ActionState.Rally)
            {
                track.Complete -= OnComplete;
                // if (!dummy.IsAlive)
                // {
                //     dummy.Remove();
                // }
                return;
            }

            if (timeTriggerAttack > minTimeTriggerAttack && dummy.CanControl && !dummy.UnitState.IsStatusCC())
            {
                this.owner.UnitSpine.Idle();
            }

            onComplete?.Invoke();

            track.Complete -= OnComplete;
        }
    }
}
