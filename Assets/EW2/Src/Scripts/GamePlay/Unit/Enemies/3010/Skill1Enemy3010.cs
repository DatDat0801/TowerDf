using System.Collections;
using System.Collections.Generic;
using Hellmade.Sound;
using Lean.Pool;
using Map;
using Spine;
using Spine.Unity;
using TigerForge;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3010 : SkillEnemy
    {
        [SerializeField] private float secondDelaySpawnEnemy;

        private EnemyData3010.EnemyData3010Skill1 skill1Data;
        private Enemy3010 enemy;


        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            skill1Data = skill1Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3010>()
                .GetSkill1(enemyBase.Level);
            enemy = (Enemy3010)enemyBase;
        }

        public IEnumerator SpawnEnemies()
        {
            //Sfx break barrel
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3010_BREAK);
            EazySoundManager.PlaySound(audioClip);

            var paths = enemyBase.LineController.CalculateRemainPathWayPoints(enemyBase.transform.position);

            for (int i = 0, length = skill1Data.numberSummon; i < length; i++)
            {
                if (this.enemy == null)
                {
                    yield return null;
                }

                var goEnemy = GamePlayController.Instance.SpawnController.SpawnEnemy(CalculateEnemySummonId(),
                    enemy.transform.position, paths);

                if (goEnemy)
                    goEnemy.EnableColliderImmediate();

                yield return new WaitForSeconds(secondDelaySpawnEnemy);
            }

            EventManager.EmitEventData(GamePlayEvent.OnEnemyDie, this.enemy.Id);
            LeanPool.Despawn(enemyBase.gameObject);
        }

        private int CalculateEnemySummonId()
        {
            for (int i = 0, length = skill1Data.chanceSummon.Length; i < length; i++)
            {
                var random = Random.Range(0, 1f);
                if (random < skill1Data.chanceSummon[i])
                {
                    return skill1Data.summonEnemyIds[i];
                }

                if (i == length - 1)
                {
                    return skill1Data.summonEnemyIds[length - 1];
                }
            }

            return -1;
        }
    }
}