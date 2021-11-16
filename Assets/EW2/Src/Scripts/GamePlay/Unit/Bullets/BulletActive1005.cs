using System;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class BulletActive1005 : BulletTrail
    {
        private Hero1005 ownerControl;
        private int _enemyDeadByActive;

        public void InitBullet1005ActiveSkill(Unit shooter, Vector3 targetPosition, float timeFly)
        {
            ownerControl = (Hero1005)shooter;
            myRigidbody.simulated = true;
            flying = true;
            AddForceBullet(transform, myRigidbody, transform.position, targetPosition, timeFly);
            InvokeProxy.Iinvoke.Invoke(this, StopMotion, timeFly - 0.016f);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (flying)
            {
                float angle = (float)(Math.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg);

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
            //start aoe for range attack, Passive 2
            if (this.ownerControl. PassiveSkill2.Level > 0)
                this.ownerControl. AoeCountTime.Start();
            
            SpawnImpact();
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.HERO_1005_ACTIVE_SKILL_IMPACT);
            EazySoundManager.PlaySound(audioClip1);
        }

        private async void SpawnImpact()
        {
            // var idSoundRandom = Random.Range(1, 4);
            // var soundName = string.Format(SoundConstant.Tower2003Impact, idSoundRandom);
            // EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(this.ownerControl.Id, soundName),
            //     EazySoundManager.GlobalSoundsVolume);
            //TODO Sound here

            var posEffect = transform.position;

            posEffect.y = posEffect.y + 0.3f;

            //boosted 
            var goImpact = ResourceUtils.GetVfxHero("1005_active_impact", posEffect, Quaternion.identity);

            if (goImpact)
            {
                var control = goImpact.GetComponent<Hero1005ActiveImpact>();

                if (control)
                {
                    control.InitAOE(ownerControl);
                }

                Despawn();

                //wait for collision done
                await UniTask.Delay(TimeSpan.FromMilliseconds(200));
                CountEnemyDeadByActive(control);
                Debug.Log("Killed " + this._enemyDeadByActive + " by active skill 1005");
                //this.ownerControl.KilledEnemyByActiveSkill = this._enemyDeadByActive;
                if (this._enemyDeadByActive > 0)
                {
                    
                    this.ownerControl.DecreaseCooldown(this._enemyDeadByActive *
                                                       this.ownerControl.ActiveSkill.SkillData.cooldownDecrease);
                    this._enemyDeadByActive = 0;
                }


                control.ResetEnemyGetDamageByThisActive();
            }
        }

        private void CountEnemyDeadByActive(Hero1005ActiveImpact impact)
        {
            foreach (var p in impact.EnemiesGetHurt)
            {
                if (p.IsAlive == false && impact.DamageInfo.creator == this.ownerControl)
                {
                    this._enemyDeadByActive++;
                }
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