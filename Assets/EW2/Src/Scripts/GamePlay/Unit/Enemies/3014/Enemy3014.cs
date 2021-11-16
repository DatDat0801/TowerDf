using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Enemy3014 : Enemy3013
    {
        [SerializeField] private Transform pointSpawnAlly;

        private const int EnemyIdSpawn = 3001;

        private Coroutine spawnAllyCoroutune;

        public override void OnDisable()
        {
            base.OnDisable();

            if (spawnAllyCoroutune != null)
                CoroutineUtils.Instance.StopCoroutine(spawnAllyCoroutune);
        }

        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3014>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override void Remove()
        {
            base.Remove();
            SpawnDeadVfx();

            if (IsCompleteEndPoint) return;

            spawnAllyCoroutune = CoroutineUtils.Instance.StartCoroutine(SpawnAlly());
        }

        private IEnumerator SpawnAlly()
        {
            var paths = LineController.CalculateRemainPathWayPoints(transform.position);

            var goEnemy = GamePlayController.Instance.SpawnController.SpawnEnemy(EnemyIdSpawn, pointSpawnAlly.position);

            if (goEnemy)
            {
                goEnemy.transform.DOMoveY(goEnemy.transform.position.y - 0.6f, 0.5f);
            }
            else
            {
                yield return null;
            }

            if (paths != null && paths.Count > 0)
            {
                goEnemy.MoveToEndPoint(paths);
            }

            goEnemy.EnableColliderImmediate();
        }
    }
}