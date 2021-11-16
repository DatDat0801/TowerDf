using System;
using Constants;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class BulletMovingCurve : BaseBullet
    {
        public Sprite[] state;


        public void InitBullet(Unit shooter, Unit target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyDamageBox);
            myRigidbody.simulated = true;
            Init(shooter, onFlyFinish);
            this.target = target;
            myRender.sprite = state[0];
            if (target is EnemyBase)
            {
                des = ((EnemyBase) target).GetFuturePosition(timeFly);
            }
            else
            {
                des = target.transform.position;
            }

            flying = true;
            AddForceBullet(transform, myRigidbody, transform.position, des, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (flying)
            {
                float angle = (float) (Math.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
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
            myRender.sprite = state[1];

            if (this.onFlyFinish != null)
            {
                this.onFlyFinish(null);
                this.onFlyFinish = null;
            }

            InvokeProxy.Iinvoke.Invoke(this, Despawn, 2f);
        }
    }
}