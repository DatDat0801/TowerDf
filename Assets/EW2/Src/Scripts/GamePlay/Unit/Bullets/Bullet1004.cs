using System;
using Hellmade.Sound;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet1004 : BulletTrail
    {
        private Pet1004 hero1004;
        private float timeLifePoisonStatus;
        private float damagePoisonStatus;

        public void InitBullet1004(Unit shooter, Unit target, float timeFly, float timeLifePoison, float damagePoison)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.AllyDamageBox);
            myRigidbody.simulated = true;
            timeLifePoisonStatus = timeLifePoison;
            damagePoisonStatus = damagePoison;
            Init(shooter, target);
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

                    var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_basic_attack_range_impact");
                    EazySoundManager.PlaySound(audioClip);
                    var goEffect = ResourceUtils.GetVfxHero("1004_attack_range_impact", Vector3.zero,
                        Quaternion.identity, enemy.Transform);

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

            var audioClip = ResourceUtils.LoadSound($"Sounds/Sfx/1004/sfx_1004_basic_attack_range_impact");
            EazySoundManager.PlaySound(audioClip);
            ResourceUtils.GetVfxHero("1004_attack_range_impact", transform.position, Quaternion.identity);

            flying = false;
            des = transform.position;
            des.z = des.y / 10;
            transform.position = des;
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);
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

        public override DamageInfo GetDamage(Unit target)
        {
            if (isAoe == false && (this.target == null || (this.target != null && this.target != target))) return null;

            var poison = new PoisonStatus(new StatusOverTimeConfig()
            {
                creator = this.owner,
                owner = target,
                lifeTime = timeLifePoisonStatus,
                intervalTime = 1,
                baseValue = damagePoisonStatus,
                damageType = owner.DamageType,
                statusType = StatusType.Poison
            });
            poison.Stacks = false;

            target.StatusController.AddStatus(poison);

            return damageInfo;
        }
    }
}