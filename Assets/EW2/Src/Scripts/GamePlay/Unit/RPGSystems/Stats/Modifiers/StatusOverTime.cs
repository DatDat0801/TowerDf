using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zitga.Observables;
using Random = UnityEngine.Random;

namespace EW2
{
    public enum StatusType
    {
        Stun = 0,
        Freeze = 1,
        Taunt = 2,
        Silent = 3,
        Terrify = 4,
        HealInstantOverTime = 5,
        HealPercentOverTime = 6,
        ShieldPoint = 7,
        DOT = 8,
        DOTFollowTrigger = 9,
        ModifierStatOverTime = 10,
        Bleed = 11,
        Poison = 12,
        BuffMoveSpeed = 13,
        Slow = 14,
        Cold = 15
    }

    public abstract class StatusOverTime : ObservableObject
    {
        public Action SpawningAnyVfxStatusWhenExecute { get; set; }
        public Action OnStatusExecute { get; set; }
        protected StatusType statusType;

        protected readonly StatusOverTimeConfig config;

        private float _value;

        protected bool isExecuting;

        public bool IsExecuting => isExecuting;

        public float TimeLife
        {
            get => config.lifeTime;
            set => Set(ref config.lifeTime, value);
        }

        public float Value
        {
            get => this._value;
            set => Set(ref this._value, value);
        }

        protected float BaseValue
        {
            get => config.baseValue;
            private set
            {
                Set(ref config.baseValue, value);
                UpdateValue();
            }
        }

        public StatusType Status => statusType;

        public bool IsPermanent => Math.Abs(this.TimeLife - float.MaxValue) < 0.001f;

        public bool Stacks { get; set; } = true;


        private Coroutine coroutine;

        protected StatusOverTime(StatusOverTimeConfig config)
        {
            this.config = config;

            TimeLife = config.lifeTime;

            BaseValue = config.baseValue;

            if (this.config.intervalTime == 0)
            {
                this.config.intervalTime = this.config.lifeTime;
            }
        }

        public abstract void UpdateValue();

        protected abstract void DoStatus(UnityAction callback = null);

        public virtual void Execute(UnityAction callback = null)
        {
            if (isExecuting || this.config.owner is DefensivePointBase)
            {
                return;
            }

            Stop();
            SpawningAnyVfxStatusWhenExecute?.Invoke();
            coroutine = CoroutineUtils.Instance.StartCoroutine(IExecute(callback));
        }

        protected virtual IEnumerator IExecute(UnityAction callback)
        {
            float runTime = 0;

            isExecuting = true;

            Prepare();

            yield return new WaitForSeconds(config.delayTime);

            runTime += config.delayTime;

            var waitForSeconds = new WaitForSeconds(config.intervalTime);

            while (config.owner != null && runTime < TimeLife && config.owner.IsAlive)
            {
                DoStatus(callback);
                OnStatusExecute?.Invoke();
                yield return waitForSeconds;

                runTime += config.intervalTime;
            }

            Complete();

            Remove();
        }

        public bool CanApplyStatus()
        {
            if (this.config.owner.Immutable || this.config.owner is DefensivePointBase)
            {
                return false;
            }
            var percentRate = Random.Range(0, 1f);
            return percentRate < config.chanceApply;
        }

        public virtual void Stop()
        {
            isExecuting = false;

            if (coroutine != null && CoroutineUtils.Instance)
            {
                CoroutineUtils.Instance.StopCoroutine(coroutine);

                coroutine = null;
            }
        }

        public virtual void Remove()
        {
            Stop();

            if (config.owner != null)
                config.owner.StatusController.RemoveStatus(this);
        }

        public abstract void Prepare();

        public abstract void Complete();
    }
}