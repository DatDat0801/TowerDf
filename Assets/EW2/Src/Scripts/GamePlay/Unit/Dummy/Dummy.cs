using System;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public abstract class Dummy : Unit
    {
        [SerializeField] private UnitSize mySize;

        public UnitSize MySize => mySize;

        public int Level { get; protected set; }


        protected UnitState dummyState;
        public override UnitState UnitState => dummyState ?? (dummyState = new DummyState(this));

        protected DummySpine dummySpine;
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new DummySpine(this));

        public TakeDamageCalculation takeDamageCalculation;

        protected HealthBarController healthBar;

        protected StatusBarController statusBar;
        public bool CanChooseIsTarget { get; set; } = true;

        /// <summary>
        /// (Victim, Killer)
        /// </summary>
        public UnityAction<Unit, Unit> onDead { get; set; }

        public UnityAction<DamageInfo> onGetHurt { get; set; }

        public HealthBarController HealthBar
        {
            get
            {
                if (healthBar == null)
                    healthBar = GetComponentInChildren<HealthBarController>();

                return healthBar;
            }
        }

        public StatusBarController StatusBar
        {
            get
            {
                if (this.statusBar == null)
                    this.statusBar = GetComponentInChildren<StatusBarController>();

                return statusBar;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            UnitType = UnitType.None;

            takeDamageCalculation = new TakeDamageCalculation(this);
        }

        public override bool IsAlive
        {
            get
            {
                if (gameObject == null) return false;

                return gameObject.activeSelf && Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue > 0;
            }
        }

        public virtual bool CanControl => IsAlive && UnitState.CanControl();

        public virtual float TakeDamage(DamageInfo info)
        {
            return takeDamageCalculation.Calculate(info);
        }

        public virtual void GetHurt(DamageInfo damageInfo)
        {
            if (IsAlive == false)
            {
                print("Unit was died: " + name);
                return;
            }

            CalculateHealthPoint(damageInfo);

            if (IsAlive && damageInfo.showVfxNormalAtk)
            {
                ShowEffectGetDamage(damageInfo.damageType);
            }
            
            onGetHurt?.Invoke(damageInfo);
            if (IsAlive == false)
            {
                if (damageInfo.creator != null)
                {
                    print($"{name} was killed by {damageInfo.creator.name}");
                    onDead?.Invoke(this, damageInfo.creator);
                }
                else
                {
                    print($"{name} was killed by an 'not unit'");
                }


                Remove();
            }
        }

        public abstract void AttackMelee();

        public abstract void AttackRange();

        public virtual AttackMelee GetAttackMelee() => null;


        protected virtual void CalculateHealthPoint(DamageInfo damageInfo)
        {
            var damage = TakeDamage(damageInfo);

            var health = this.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            health.TakeDamage(damage, damageInfo.creator);

            Debug.Log(
                $"{damageInfo} \n=> {name} Damage: {damage}, HP: {health.CurrentValue},  Percent: {health.CalculateCurrentPercent()}");
        }

        public override void Flip(float positionX)
        {
            base.Flip(positionX);

            FlipComponent();
        }

        public void FlipComponent()
        {
            if (HealthBar)
            {
                var scale = HealthBar.transform.localScale;

                if ((transform.localScale.x < 0 && scale.x > 0) || (transform.localScale.x > 0 && scale.x < 0))
                {
                    scale.x *= -1;

                    HealthBar.transform.localScale = scale;
                }
            }

            var shieldBar = GetComponentInChildren<ShieldBarController>();

            if (shieldBar != null)
            {
                var scale = shieldBar.transform.localScale;

                if ((transform.localScale.x < 0 && scale.x > 0) || (transform.localScale.x > 0 && scale.x < 0))
                {
                    scale.x *= -1;

                    shieldBar.transform.localScale = scale;
                }
            }

            var virtualHp = GetComponentInChildren<VirtualHpBarController>();
            if (virtualHp)
            {
                var scale = virtualHp.transform.localScale;

                if ((transform.localScale.x < 0 && scale.x > 0) || (transform.localScale.x > 0 && scale.x < 0))
                {
                    scale.x *= -1;

                    virtualHp.transform.localScale = scale;
                }
            }
        }

        public virtual void ShowEffectGetDamage(DamageType damageType)
        {
            GameObject effect = null;

            var posEffect = UnitSizeConstants.GetUnitPosEffect(MySize);

            if (damageType == DamageType.Magical)
            {
                effect = ResourceUtils.GetVfx("Effects", "fx_common_magic_basic_attack", posEffect,
                    Quaternion.identity, transform,10);
            }
            else
            {
                effect = ResourceUtils.GetVfx("Effects", "fx_common_physic_basic_attack", posEffect,
                    Quaternion.identity, transform,60);
            }

            if (effect)
            {
                var sizeEffect = UnitSizeConstants.GetUnitSize(MySize);

                effect.transform.localScale = new Vector3(sizeEffect, sizeEffect, sizeEffect);

                effect.SetActive(true);
            }

            effect.transform.parent = null;
        }

        public virtual void SetColdOverrideMaterial()
        {
            
        }

        public virtual void RemoveColdOverrideMaterial()
        {
            
        }
    }
}