using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Enemy3020Skill3 : SkillEnemy
    {
        private Enemy3020 enemy;
        private EnemyData3020.EnemyData3020Skill3 skillData;
        private Coroutine spawnAlly;
        
        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);

            enemy = (Enemy3020) enemyBase;
            if (skillData == null)
                skillData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>().GetSkill3(enemyBase.Level);
            timeCooldown = skillData.cooldown;
        }

        public override void CastSkill()
        {
            base.CastSkill();

            if(spawnAlly != null)
                CoroutineUtils.Instance.StopCoroutine(spawnAlly);
            
            spawnAlly = CoroutineUtils.Instance.StartCoroutine(SpawnEnemies());
        }

        public override bool CanCastSkill()
        {
            return base.CanCastSkill() && enemy.IsAlive;
        }

        private int GetEnemySummonId()
        {
            var index = Random.Range(0, skillData.enemySummon.Length);
            return skillData.enemySummon[index];
        }

        public IEnumerator SpawnEnemies()
        {
            yield return new WaitForSeconds(1.67f);

            var ownPos = enemy.transform.position;
            var paths = enemy.LineController.CalculateRemainPathWayPoints(ownPos);

            var spawnPositions = new List<Vector2>();

            for (var i = 0; i < skillData.numberSummon; i++)
            {
                var randomPos = RandomPointInAnnulus(ownPos, 0.6f, 0.6f);
                spawnPositions.Add(randomPos);
            }

            for (int i = 0, length = skillData.numberSummon; i < length; i++)
            {
                var vfx = ResourceUtils.GetVfx("Enemy", "3020_skill_3_spawn");
                vfx.transform.position = spawnPositions[i];
                DoSpawnEnemy(spawnPositions[i], paths);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void DoSpawnEnemy(Vector3 pos, List<Vector3> paths)
        {
            var goEnemy = GamePlayController.Instance.SpawnController.SpawnEnemy(GetEnemySummonId(), pos, paths);
            goEnemy.LineController = enemy.LineController;
            
            if (goEnemy)
                goEnemy.EnableColliderImmediate();
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