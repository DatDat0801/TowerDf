using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet3007 : BulletTrail
    {
        private float lifeTime;
        private float speed;
        private float countTime;

        private Enemy3007 creator;

        private bool IsAlive => countTime < lifeTime;

        public override void OnUpdate(float deltaTime)
        {
            if (IsAlive == false)
            {
                Despawn();
                return;
            }


            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * deltaTime);

            Vector2 direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = rotation;

            countTime += deltaTime;
        }

        public void Init(Enemy3007 creator, Unit target, float lifeTime, float speed)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.EnemyDamageBox);
            Init(creator, target);

            this.countTime = 0;
            this.lifeTime = lifeTime;
            this.speed = speed;
            this.creator = creator;
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

        public override DamageInfo GetDamage(Unit target)
        {
            SpawnImpact(target);
            return base.GetDamage(target);
        }

        private void SpawnImpact(Unit target)
        {
            //ResourceUtils.GetVfxTower("2002_attack_range_1_impact", transform.position, Quaternion.identity);
            if (target == creator.target)
            {
                ResourceUtils.GetUnit("3007_range_impact", target.transform.position, Quaternion.identity);
            }
        }

        protected override void ShowTrail()
        {
            if (trails.Length > 0)
            {
                trails[0].SetActive(true);
            }
        }
    }
}