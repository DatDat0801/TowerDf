using System;
using Invoke;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;
using Zitga.Update;

namespace EW2
{
    public class Bullet3016 : MonoBehaviour, IUpdateSystem
    {
        [SerializeField] protected Rigidbody2D myRigidbody;
        
        private bool flying;
        private Vector3 destination;
        private UnityAction onFlyFinish;

        void Init(Vector3 shooter, Vector3 target, UnityAction onFlyFinish = null)
        {
            this.onFlyFinish = onFlyFinish;
            this.destination = target;
            if (myRigidbody)
            {
                myRigidbody.simulated = true;
            }

            flying = true;
        }
        public void InitBullet3016(Vector3 shooter, Vector3 target, float timeFly, UnityAction onFlyFinish = null)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.AllyDamageBox);
            Init(shooter, target, onFlyFinish);

            AddForceBullet(transform, myRigidbody, transform.position, target, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly);
        }

        protected void StopMotion()
        {
            if (gameObject == null)
                return;

            flying = false;
            destination = transform.position;
            destination.z = destination.y / 10;
            transform.position = destination;
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);
            myRigidbody.simulated = false;

            if (this.onFlyFinish != null)
            {
                this.onFlyFinish();
                this.onFlyFinish = null;
            }

            InvokeProxy.Iinvoke.Invoke(this, Despawn, 0.01f);
        }
        public virtual void Despawn()
        {
            InvokeProxy.Iinvoke.CancelInvoke(this);

            LeanPool.Despawn(gameObject);
        }
        public void OnUpdate(float deltaTime)
        {
            // if (flying)
            // {
            //     float angle = (float) (Math.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg);
            //     transform.rotation = Quaternion.Euler(0, 0, angle);
            // }
        }
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
    }
}