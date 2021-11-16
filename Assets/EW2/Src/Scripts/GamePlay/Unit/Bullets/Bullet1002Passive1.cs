using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet1002Passive1 : BulletTrail
    {
        private Sprite arrowNormal, arrowCut;

        private void Awake()
        {
            arrowNormal = ResourceUtils.GetSpriteAtlas("bullets", "bullet_1002_0");
            arrowCut = ResourceUtils.GetSpriteAtlas("bullets", "bullet_1002_0_cut");
        }

        public void InitBullet1002Passive1(Unit shooter, Unit target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.AllyDamageBox);
            myRigidbody.simulated = true;
            Init(shooter, target, onFlyFinish);
            myRender.sprite = arrowNormal;
            if (target is EnemyBase)
            {
                var enemy = (EnemyBase)target;
                des = enemy.GetFuturePosition(timeFly) + new Vector2(0, 0.23f);
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
                float angle = (float)(Math.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy == target)
                {
                    //effect
                    var posEffect = enemy.transform.position;

                    posEffect.y = posEffect.y + 0.2f;

                    var goEffect = ResourceUtils.GetVfxHero("1002_skill_passive_1_impact", Vector3.zero, Quaternion.identity, enemy.Transform);

                    if (goEffect != null)
                        goEffect.transform.position = posEffect;

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

        protected void StopMotion()
        {
            if (transform == null)
                return;

            flying = false;
            des = transform.position;
            des.z = des.y / 10;
            transform.position = des;
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);
            myRigidbody.simulated = false;
            myRender.sprite = arrowCut;

            if (this.onFlyFinish != null)
            {
                this.onFlyFinish(null);
                this.onFlyFinish = null;
            }

            InvokeProxy.Iinvoke.Invoke(this, Despawn, 2f);
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