using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class BulletRange1005 : BulletTrail
    {
        private Hero1005 _ownerControl;

        public override DamageInfo GetDamage(Unit targetUnit)
        {
            if (targetUnit is EnemyBase enemyBase)
                this._ownerControl.AddEnemyToTrack(enemyBase);
            return base.GetDamage(targetUnit);
        }

        public void InitBullet1005(Unit shooter, Unit target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            this._ownerControl = (Hero1005)shooter;
            HandlePassive2();

            gameObject.layer = LayerMask.NameToLayer(LayerConstants.AllyDamageBox);
            myRigidbody.simulated = true;
            Init(shooter, target, onFlyFinish);
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

        private void HandlePassive2()
        {
            if (this._ownerControl.PassiveSkill2.Level <= 0) return;
            //handle passive 2
            if (this._ownerControl.AoeCountTime.TimeOut)
            {
                this.isAoe = false;
            }
            else
            {
                this.isAoe = true;
            }
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

                    GameObject goEffect = null;
                    if (this._ownerControl.IsInPassive2())
                    {
                        goEffect =  ResourceUtils.GetVfxHero("1005_passive_2_impact", enemy.transform.position,
                            Quaternion.identity);
                    }
                    else
                    {
                      goEffect =  ResourceUtils.GetVfxHero("1005_attack_range_impact", Vector3.zero,
                            Quaternion.identity, enemy.Transform);
                    }

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