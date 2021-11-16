using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Invoke;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EW2
{
    public class Bullet2003 : BaseBullet
    {
        private Tower2003 ownerControl;

        [SerializeField] private GameObject boostedBullet;

        private bool isBoostedBullet;

        public override DamageInfo GetDamage(Unit target)
        {
            //set damageInfo
            var level = ownerControl.towerData.BonusStat.level;
            if (level >= 3)
            {
                var fireBulletRatio = ownerControl.towerData.BonusStat.level3Stat.fireBulletRatio;
                var index = RandomFromDistribution.RandomChoiceFollowingDistribution(new List<float>()
                    {1 - fireBulletRatio, fireBulletRatio});

                damageInfo.value = ownerControl.towerData.BonusStat.level3Stat.fireBulletDamage;
                myRender.gameObject.SetActive(false);
                boostedBullet.SetActive(true);
            }
            else
            {
                myRender.gameObject.SetActive(true);
                boostedBullet.SetActive(false);
            }

            return base.GetDamage(target);
        }

        public void InitBullet2003(Tower2003 shooter, Unit target, float timeFly, Action<Unit> onFlyFinish = null)
        {
            ownerControl = shooter;

            if (target is EnemyBase)
            {
                des = ((EnemyBase) target).GetFuturePosition(timeFly);
            }
            else
            {
                des = target.transform.position;
            }

            this.onFlyFinish = onFlyFinish;

            SetSprite();

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

        protected void StopMotion()
        {
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

            SpawnImpact();
        }

        private async void SpawnImpact()
        {
            var idSoundRandom = Random.Range(1, 4);
            var soundName = string.Format(SoundConstant.Tower2003Impact, idSoundRandom);
            EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(this.ownerControl.Id, soundName),
                EazySoundManager.GlobalSoundsVolume);

            var posEffect = transform.position;

            posEffect.y = posEffect.y + 0.3f;

            if (!isBoostedBullet)
            {
                //normal impact
                var goImpact = ResourceUtils.GetVfxTower("2003_attack_range_impact", posEffect, Quaternion.identity);

                if (goImpact)
                {
                    var control = goImpact.GetComponent<Tower2003AtkRangeImpact>();

                    if (control)
                    {
                        control.InitAOE(ownerControl);
                    }
                }
                
            }
            else
            {
                //boosted 
                var goImpact = ResourceUtils.GetVfxTower("2003_skill3_projectile_impact", posEffect, Quaternion.identity);
                
                if (goImpact)
                {
                    var control = goImpact.GetComponent<BoostedBulletImpact>();

                    if (control)
                    {
                        control.InitAOE(ownerControl);
                    }
                    //await UniTask.Delay(500);
                    var poisonousFire = goImpact.GetComponentInChildren<PoisonousFireImpact>();
                    poisonousFire.InitAOE(ownerControl);
                }
            }
            Despawn();
        }
        
        
        private void SetSprite()
        {
            var level = ownerControl.towerData.BonusStat.level;
            if (level >= 3)
            {
                var fireBulletRatio = ownerControl.towerData.BonusStat.level3Stat.fireBulletRatio;
                var index = RandomFromDistribution.RandomChoiceFollowingDistribution(new List<float>()
                    {1 - fireBulletRatio, fireBulletRatio});
                if (index == 1)
                {
                    myRender.gameObject.SetActive(false);
                    boostedBullet.SetActive(true);
                    isBoostedBullet = true;
                    return;
                }
            }

            myRender.sprite = ResourceUtils.GetSpriteAtlas("bullets", $"bullet_2003_{ownerControl.Level - 1}");
            myRender.gameObject.SetActive(true);
            boostedBullet.SetActive(false);
            isBoostedBullet = false;
        }
    }
}