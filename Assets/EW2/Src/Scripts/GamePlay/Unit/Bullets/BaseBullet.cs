using System;
using DG.Tweening;
using Invoke;
using Lean.Pool;
using UnityEngine;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public abstract class BaseBullet : MonoBehaviour, IUpdateSystem, IGetDamage
    {
        [SerializeField] protected bool isNormalAttack;
        [SerializeField] protected bool isAoe;
        [SerializeField] protected Rigidbody2D myRigidbody;
        [SerializeField] protected SpriteRenderer myRender;

        protected Unit owner;
        protected Unit target;
        protected Action<Unit> onFlyFinish;
        protected bool flying;
        protected Vector3 des;

        protected GetDamageCalculation damageCalculation;

        protected DamageInfo damageInfo;

        protected virtual void InitDamageInfo(float damage = 0)
        {
            damageCalculation = new GetDamageCalculation(this.owner);

            damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                showVfxNormalAtk = isNormalAttack, 
                
                target = target
            };

            if (damage <= 0)
            {
                (damageInfo.value, damageInfo.isCritical) = damageCalculation.Calculate(target);
            }
            else
            {
                damageInfo.value = damage;
                damageInfo.isCritical = false;
            }
        }

        public virtual void Init(Unit shooter, Unit target, Action<Unit> onFlyFinish = null)
        {
            Init(shooter, target, 0, onFlyFinish);
        }

        public virtual void Init(Unit shooter, Action<Unit> onFlyFinish = null)
        {
            Init(shooter, null, onFlyFinish);
        }

        public virtual void Init(Unit shooter, Unit target, float damage, Action<Unit> onFlyFinish = null)
        {
            this.owner = shooter;

            this.target = target;

            this.onFlyFinish = onFlyFinish;

            SetDefaultBullet();

            InitDamageInfo(damage);
        }

        public virtual void OnEnable()
        {
            Context.Current.GetService<GlobalUpdateSystem>().Add(this);
        }

        public virtual void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);

            Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        protected virtual void SetDefaultBullet()
        {
            if (myRigidbody)
            {
                myRigidbody.simulated = true;
            }

            flying = true;
        }

        public abstract void OnUpdate(float deltaTime);

        public virtual void Despawn()
        {
            InvokeProxy.Iinvoke.CancelInvoke(this);

            LeanPool.Despawn(gameObject);
        }

        public virtual DamageInfo GetDamage(Unit target)
        {
            if (isAoe == false && (this.target == null || (this.target != null && this.target != target))) return null;

            return damageInfo;
        }
        /// <summary>
        /// Curve bullet
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rigidBody2D"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="t"></param>
        public void AddForceBullet(Transform transform, Rigidbody2D rigidBody2D, Vector3 startPos, Vector3 endPos,
            float t)
        {
            Vector3 target = endPos - startPos;
            if (target.x == 0)
            {
                target.x = 0.01f;
            }

            float tanAlpha = (target.y - Physics2D.gravity.y * rigidBody2D.gravityScale * t * t / 2) / target.x;
            float alpha = (float) Math.Atan(tanAlpha);
            float v0 = target.x / MathUtils.FastCos(alpha) / t;
            alpha = alpha * Mathf.Rad2Deg;

            Vector2 forceDir = Quaternion.Euler(0, 0, alpha) * Vector2.right;
            if (transform != null)
            {
                transform.rotation = Quaternion.Euler(0, 0, alpha);
            }

            rigidBody2D.AddForce(forceDir * v0, ForceMode2D.Impulse);
        }

        public void AddForceStraight(Transform transform, Rigidbody2D rigidBody2D, Vector3 startPos, Vector3 endPos,
            float t)
        {
            rigidBody2D.DOMove(endPos, t).From(startPos).SetEase(Ease.Linear);
        }
    }
}