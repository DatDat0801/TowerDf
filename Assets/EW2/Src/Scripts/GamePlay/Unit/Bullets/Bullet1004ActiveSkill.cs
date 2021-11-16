using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet1004ActiveSkill : BulletTrail
    {
        private Pet1004 hero1004;

        public void InitBullet1004ActiveSkill(Unit shooter, Vector3 target, float timeFly)
        {
            hero1004 = (Pet1004)shooter;
            myRigidbody.simulated = true;
            flying = true;
            AddForceBullet(transform, myRigidbody, transform.position, target, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly - 0.016f);
        }

        protected void StopMotion()
        {
            if (transform == null)
                return;

            hero1004.SpawnActiveImpact(transform.position);

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

        protected override void ShowTrail()
        {
            if (trails.Length > 0)
            {
                trails[0].SetActive(true);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
        }
    }
}