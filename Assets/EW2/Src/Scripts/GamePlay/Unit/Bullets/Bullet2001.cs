using System;
using System.Collections.Generic;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet2001 : BulletTrail
    {
        [SerializeField] private GameObject critArrowRenderer;

        private Sprite arrowLvl1Normal, arrowLvl1Cut;
        private Sprite arrowLvl2Normal, arrowLvl2Cut;
        private Soldier2001 ownerControl;

        private bool isCriticalArrow;

        private void Awake()
        {
            arrowLvl1Normal = ResourceUtils.GetSpriteAtlas("bullets", "bullet_2001_0");
            arrowLvl1Cut = ResourceUtils.GetSpriteAtlas("bullets", "bullet_2001_0_cut");

            arrowLvl2Normal = ResourceUtils.GetSpriteAtlas("bullets", "bullet_2001_1");
            arrowLvl2Cut = ResourceUtils.GetSpriteAtlas("bullets", "bullet_2001_1_cut");
        }

        protected override void InitDamageInfo(float damage = 0)
        {
            damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                showVfxNormalAtk = isNormalAttack, target = target
            };
            //tower upgrade system critical
            var criticalRatio = ownerControl.Tower.towerData.BonusStat.level6Stat.criticalRatio;
            var criticalDamage = ownerControl.Tower.towerData.BonusStat.level6Stat.criticalDamage;
            var index = RandomFromDistribution.RandomChoiceFollowingDistribution(new List<float>()
                {1 - criticalRatio, criticalRatio});
            if (index == 1)
            {
                var dmg = ownerControl.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue;
                damageInfo.value = dmg * criticalDamage;
                isCriticalArrow = damageInfo.isCritical = true;
                //Debug.LogAssertion("Arrow critical");
            }
            else
            {
                var dmg = ownerControl.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue;
                damageInfo.value = dmg;
                isCriticalArrow = damageInfo.isCritical = false;
                //Debug.LogAssertion("Normal arrow");
            }
        }

        public void InitBullet2001(Unit shooter, Unit target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            ownerControl = (Soldier2001) shooter;

            gameObject.layer = LayerMask.NameToLayer(LayerConstants.AllyDamageBox);

            myRigidbody.simulated = true;

            Init(shooter, target, onFlyFinish);

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
                if (enemy == target)
                {
                    //effect
                    var posEffect = enemy.transform.position;

                    posEffect.y = posEffect.y + 0.2f;

                    var goEffect = ResourceUtils.GetVfxTower("2001_archer_impact", Vector3.zero,
                        Quaternion.identity, enemy.Transform);
                    
                    if (goEffect != null)
                        goEffect.transform.position = posEffect;

                    InvokeProxy.Iinvoke.CancelInvoke(this);

                    if (this.onFlyFinish != null)
                    {
                        this.onFlyFinish(target);
                        this.onFlyFinish = null;
                    }

                    InvokeProxy.Iinvoke.Invoke(this, Despawn, 0.02f);
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

            var timeDelayToDestroy = 2f;
            if (isCriticalArrow)
                timeDelayToDestroy = 1f;

            InvokeProxy.Iinvoke.Invoke(this, Despawn, timeDelayToDestroy);
        }

        private void SetSpriteArrowNormal()
        {
            if (isCriticalArrow)
            {
                myRender.gameObject.SetActive(false);
                critArrowRenderer.SetActive(true);
            }
            else
            {
                myRender.gameObject.SetActive(true);
                critArrowRenderer.SetActive(false);
                if (ownerControl.Tower.Level < 3)
                {
                    myRender.sprite = arrowLvl1Normal;
                }
                else
                {
                    myRender.sprite = arrowLvl2Normal;
                }
            }
        }

        private void SetSpriteArrowCut()
        {
            if (ownerControl.Tower.Level < 3)
            {
                myRender.sprite = arrowLvl1Cut;
            }
            else
            {
                myRender.sprite = arrowLvl2Cut;
            }
        }

        protected override void ShowTrail()
        {
            if (isCriticalArrow)
            {
                if (trails.Length >= 3)
                {
                    trails[0].SetActive(false);
                    trails[1].SetActive(false);
                    trails[2].SetActive(true);
                    return;
                }
            }

            if (ownerControl.Tower.Level < 3)
            {
                if (trails.Length > 0)
                {
                    trails[0].SetActive(true);
                    trails[1].SetActive(false);
                }
            }
            else
            {
                if (trails.Length > 0)
                {
                    trails[0].SetActive(false);
                    trails[1].SetActive(true);
                }
            }
        }

        protected override void HideTrail()
        {
            base.HideTrail();

            if (trails.Length > 0)
            {
                trails[0].SetActive(false);
                trails[1].SetActive(false);
                trails[2].SetActive(false);
            }
        }
    }
}