using System;
using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Lean.Pool;
using Spine.Unity;
using UnityEngine;

namespace EW2
{
    [Serializable]
    public class Skill2Enemy3016 : SkillEnemy
    {
        [SerializeField] private TowerTargetCollection3016 targetSeeker;
        public EnemyData3016.EnemyData3016Skill2 Skill2Data { get; private set; }
        private Enemy3016 enemy;
        private BoneFollower boneFollower;

        public override void Init(EnemyBase e)
        {
            base.Init(e);
            Skill2Data = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3016>().GetSkill2ByLevel(e.Level);
            enemy = (Enemy3016) e;
            timeCooldown = Skill2Data.cooldown;
            targetSeeker.SetRadius(Skill2Data.radius);
            targetSeeker.Targets.CollectionChanged += UpdateTargetList;
            lastTimeCastSkill = Time.time;
        }

        private void UpdateTargetList(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (var t in e.OldItems)
                    {
                        var targetRemove = t as Dummy;
                        if (enemy.target == targetRemove)
                        {
                            enemy.target = null;
                            break;
                        }
                    }

                    break;
            }
        }

        public bool HasTarget()
        {
            var targets = targetSeeker.SelectTarget(Skill2Data.numberOfTower);
            if (targets.Count <= 0) return false;
            return true;
        }

        public override void CastSkill()
        {
            base.CastSkill();
            enemy.UnitState.Set(ActionState.Skill2);
            DoDelaySkill();
        }

        void DoDelaySkill()
        {
            var targets = targetSeeker.SelectTarget(Skill2Data.numberOfTower);
            if (targets.Count <= 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                SpawnEffectAsync(targets[i]);
            }
        }

        async void SpawnEffectAsync(Building target)
        {
            //Close tower option if are opening
            if (TowerOption.Instance.TowerPointSelected.myTower != null)
            {
                if (TowerOption.Instance.TowerPointSelected.myTower.Equals(target))
                {
                    TowerOption.Instance.Close();
                }
            }
            //Sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3016_TOWER_CONTROL);
            EazySoundManager.PlaySound(audioClip);
            
            //set bone
            var prepareSpine =
                ResourceUtils.GetVfx("Enemy", "3016_fx_soul", enemy.transform.position, Quaternion.identity);
            
            var animation = prepareSpine.GetComponent<SkeletonAnimation>();
            animation.AnimationState.TimeScale = 1f;
            animation.timeScale = 1f;
            var myBone = animation.Skeleton.FindBone("soul_target_to_tower");

            myBone.SetPositionSkeletonSpace(prepareSpine.transform.InverseTransformPoint(target.transform.position));

            await UniTask.Delay(1300);
            var explosion = ResourceUtils.GetVfx("Enemy", "3016_skill_2_explosion_fire");
            explosion.transform.position = target.transform.position;
            
            await UniTask.Delay(200);
            
            target.SetStunInSecond(Skill2Data.inSecond);
            //Close tower option if are opening
            if (TowerOption.Instance.TowerPointSelected.myTower != null)
            {
                if (TowerOption.Instance.TowerPointSelected.myTower.Equals(target))
                {
                    TowerOption.Instance.Close();
                }
            }

            var loopFire = ResourceUtils.GetVfx("Enemy", "3016_skill_2_fire_loop", target.transform.position, Quaternion.identity);
            
            //loopFire.AddComponent<DestroyMe>().deathtimer = Skill2Data.inSecond;
            //LeanPool.Despawn(loopFire, Skill2Data.inSecond);
            target.TowerPointController.SetStunPowerPoint(Skill2Data.inSecond);
            //await UniTask.Delay(TimeSpan.FromSeconds(Skill2Data.inSecond));
            LeanPool.Despawn(loopFire, Skill2Data.inSecond);
        }
    }
}