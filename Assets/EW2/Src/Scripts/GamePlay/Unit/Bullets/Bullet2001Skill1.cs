using System;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet2001Skill1 : BulletTrail
    {
        public GameObject effectPower;
        private Sprite arrowLvl2Normal, arrowLvl2Cut;

        private void Awake()
        {
            arrowLvl2Normal = ResourceUtils.GetSpriteAtlas("bullets", "bullet_2001_1");
            arrowLvl2Cut = ResourceUtils.GetSpriteAtlas("bullets", "bullet_2001_1_cut");
        }

        public void InitBulletSkill2001(Unit shooter, Unit target, float timeFly, float damage,
            Action<Unit> onFlyFinish = null)
        {
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.AllyDamageBox);
            myRigidbody.simulated = true;
            Init(shooter, target, damage, onFlyFinish);
            if (target is EnemyBase)
            {
                des = ((EnemyBase) target).GetFuturePosition(timeFly);
            }
            else
            {
                des = target.transform.position;
            }

            flying = true;
            SetSpriteArrowNormal();
            AddForceBullet(transform, myRigidbody, transform.position, des, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly);
        }

        public void InitBulletSkill2001(Vector3 posTarget, float timeFly)
        {
            des = posTarget;
            myRigidbody.simulated = true;
            flying = true;
            SetSpriteArrowNormal();
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
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy != null && enemy == target)
                {
                    //effect

                    var sizeEffect = UnitSizeConstants.GetUnitSize(enemy.MySize);
                    var impact = ResourceUtils.GetVfxTower("2001_archer_impact", Vector3.zero, Quaternion.identity,
                        enemy.Transform);
                    impact.transform.localScale = new Vector3(sizeEffect, sizeEffect, sizeEffect);

                    InvokeProxy.Iinvoke.CancelInvoke(this);
                    if (this.onFlyFinish != null)
                    {
                        this.onFlyFinish(target);
                        this.onFlyFinish = null;
                    }

                    InvokeProxy.Iinvoke.Invoke(this, Despawn, 0.05f);
                }
            }
        }

        protected void StopMotion()
        {
            flying = false;
            des = transform.position;
            des.z = des.y / 10;
            transform.position = des;
            gameObject.layer = LayerMask.NameToLayer(LayerConstants.Default);
            myRigidbody.simulated = false;
            SetSpriteArrowCut();

            if (this.onFlyFinish != null)
            {
                this.onFlyFinish(null);
                this.onFlyFinish = null;
            }

            InvokeProxy.Iinvoke.Invoke(this, Despawn, 2f);
        }

        private void SetSpriteArrowNormal()
        {
            myRender.sprite = arrowLvl2Normal;
            effectPower.SetActive(true);
        }

        private void SetSpriteArrowCut()
        {
            myRender.sprite = arrowLvl2Cut;
            effectPower.SetActive(false);
        }

        protected override void ShowTrail()
        {
            trails[0].SetActive(true);
        }
    }
}