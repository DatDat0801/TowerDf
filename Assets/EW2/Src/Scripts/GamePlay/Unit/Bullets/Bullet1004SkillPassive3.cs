using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet1004SkillPassive3 : BulletTrail
    {
        private Pet1004 _pet1004;
        private HeroBase _heroTarget;

        public void InitBullet1004ActiveSkill(Unit shooter, Unit unitTarget, float timeFly)
        {
            this._pet1004 = (Pet1004)shooter;
            this._heroTarget = (HeroBase)unitTarget;
            myRigidbody.simulated = true;
            flying = true;
            var posTarget = new Vector3(unitTarget.transform.position.x, unitTarget.transform.position.y + 0.3f,
                unitTarget.transform.position.z);
            AddForceBullet(transform, myRigidbody, transform.position, posTarget, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly);
        }

        private void StopMotion()
        {
            if (transform == null)
                return;

            this._pet1004.SpawnPassive3Impact(transform.position, this._heroTarget);

            flying = false;
            des = transform.position;
            des.z = des.y / 10;
            transform.position = des;
            myRigidbody.simulated = false;

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