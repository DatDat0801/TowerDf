using System;
using Constants;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet1002SkillActive : BaseBullet
    {
        public void InitBullet1002Skill(Unit shooter, Vector3 target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            Init(shooter, onFlyFinish);
            des = target;
            SetDefaultBullet();

            AddForceBullet(transform, myRigidbody, transform.position, des, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly / 2);
        }

        public void InitBullet1002Passive(Unit shooter, Vector3 target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            Init(shooter, onFlyFinish);
            des = target;
            SetDefaultBullet();

            AddForceBullet(transform, myRigidbody, transform.position, des, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly);
        }

        public void InitBullet1002SkillClone(Vector3 target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            des = target;
            this.onFlyFinish = onFlyFinish;
            SetDefaultBullet();

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

        private void SetDefaultBullet()
        {
            myRigidbody.simulated = true;
            flying = true;
        }

        protected void StopMotion()
        {
            flying = false;
            des = transform.position;
            des.z = des.y / 10;
            transform.position = des;
            myRigidbody.simulated = false;

            if (this.onFlyFinish != null)
            {
                this.onFlyFinish(null);
                this.onFlyFinish = null;
            }

            Despawn();
        }
    }
}