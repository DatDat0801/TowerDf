using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EW2
{
    public class Skill1Enemy3008 : SkillEnemy
    {
        public EnemyData3008.EnemyData3008Skill1 Skill1Data { get; private set; }
        private Enemy3008 enemy;

        public override void Init(EnemyBase e)
        {
            base.Init(e);
            Skill1Data = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3008>().GetSkill1ByLevel(e.Level);
            enemy = (Enemy3008)e;
            timeCooldown = Skill1Data.cooldown;
            lastTimeCastSkill = Time.time;
        }

        public override void CastSkill()
        {
            base.CastSkill();

            enemy.UnitState.Set(ActionState.Skill1);
            SpawnEnemies();
        }

        private int GetEnemySummonId()
        {
            var index = RandomFromDistribution.RandomChoiceFollowingDistribution(Skill1Data.chanceSummon.ToList());
            return Skill1Data.summonEnemyIds[index];
        }

        public async void SpawnEnemies()
        {
            await UniTask.Delay(850);
            //Sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.EnemySummon());
            EazySoundManager.PlaySound(audioClip);

            var ownPos = enemy.transform.position;
            var paths = enemyBase.LineController.CalculateRemainPathWayPoints(ownPos);
            if (!paths.Contains(ownPos))
            {
                paths.Insert(0, ownPos);
            }

            var spawnPositions = new List<Vector2>();
            // {
            //     new Vector2(ownPos.x + 0.3f, ownPos.y), new Vector2(ownPos.x - 0.3f, ownPos.y - 0.3f),
            //     new Vector2(ownPos.x, ownPos.y + 0.3f),
            // }

            for (var i = 0; i < Skill1Data.numberSummon; i++)
            {
                var randomPos = RandomPointInAnnulus(ownPos, 0.42f, 0.42f);
                spawnPositions.Add(randomPos);
            }

            for (int i = 0, length = Skill1Data.numberSummon; i < length; i++)
            {
                var bullet = ResourceUtils.GetVfx("Enemy", "3016_skill_1_smoke");
                bullet.transform.position = enemy.enemySpawnTransform.position;
                //Vector3 destination = new Vector3(ownPos.x + 1f, ownPos.y + 1f, ownPos.z);

                var bullet3016 = bullet.GetComponent<Bullet3016>();
                var i1 = i;
                bullet3016.InitBullet3016(ownPos, spawnPositions[i], 0.5f, async () =>
                {
                    var appearVfx = ResourceUtils.GetVfx("Enemy", "3016_skill_1_explosion");
                    appearVfx.transform.position = spawnPositions[i1];
                    await DoDelayEnemy(spawnPositions[i1], paths);
                });
            }


            await UniTask.Delay(1000);
        }

        async UniTask DoDelayEnemy(Vector3 pos, List<Vector3> paths)
        {
            await UniTask.Delay(500);
            if (enemy.IsAlive)
            {
                var goEnemy = GamePlayController.Instance.SpawnController.SpawnEnemy(GetEnemySummonId(), pos, paths);
                goEnemy.LineController = this.enemy.LineController;
                if (goEnemy)
                    goEnemy.EnableColliderImmediate();
            }
        }

        public Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
        {
            var randomDirection = (Random.insideUnitCircle * origin).normalized;

            var randomDistance = Random.Range(minRadius, maxRadius);

            var point = origin + randomDirection * randomDistance;

            return point;
        }
    }
}
