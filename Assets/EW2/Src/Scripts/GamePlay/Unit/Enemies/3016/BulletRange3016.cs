using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class BulletRange3016 : BaseBullet
    {
        public override void OnUpdate(float deltaTime)
        {
        }

        public void InitBullet(Unit shooter, Unit t, float timeFly, Action<Unit> onFlyFinish = null)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyDamageBox);
            myRigidbody.simulated = true;
            Init(shooter, t, onFlyFinish);
            this.target = t;

            des = t.transform.position;

            flying = true;
            //AddForceBullet(transform, myRigidbody, transform.position, des, timeFly);
            AddForceStraight(transform, myRigidbody, transform.position, des, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var collider = other.GetComponent<BodyCollider>();
            if (collider != null)
            {
                var enemy = collider.Owner;
                if (enemy == target)
                {
                    InvokeProxy.Iinvoke.CancelInvoke(this);
                    if (this.onFlyFinish != null)
                    {
                        this.onFlyFinish(target);
                        this.onFlyFinish = null;
                    }

                    Despawn();
                }
            }
        }

        protected virtual void StopMotion()
        {
            flying = false;
            des = transform.position;
            des.z = des.y / 10;
            transform.position = des;
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.Effect);
            myRigidbody.simulated = false;

            if (this.onFlyFinish != null)
            {
                this.onFlyFinish(null);
                this.onFlyFinish = null;
            }

            InvokeProxy.Iinvoke.Invoke(this, Despawn, 0.6f);
        }

        public override DamageInfo GetDamage(Unit t)
        {
            var dummy = (Dummy) t;
            var posEffect = UnitSizeConstants.GetUnitPosEffect(dummy.MySize);
            ResourceUtils.GetVfx("Enemy", "3008_attack_range_impact", posEffect, Quaternion.identity, t.Transform);

            return base.GetDamage(t);
        }
    }
}