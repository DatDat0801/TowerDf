using System;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using Map;
using UnityEngine;

namespace EW2
{
    public class EnemyAvailableInMapSpawn : MonoBehaviour
    {
        [SerializeField] private int idEnemy;
        [SerializeField] private LineController lineController;

        [SerializeField] private GameObject statue;

        private void OnEnable()
        {
            CallWave.Instance.startSpawnWave = Enemy3017OnPhaseChanged;
        }


        private void Enemy3017OnPhaseChanged(int wave)
        {
            if (wave == 5)
                AwakenTheBoss();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                AwakenTheBoss();
            }
        }

        private void SpawnEnemy()
        {
            var enemyControl =
                GamePlayController.Instance.SpawnController.SpawnEnemy(idEnemy, transform.position, null);
            enemyControl.LineController = lineController;
            enemyControl.UnitSpine.Idle();
        }

        private async void AwakenTheBoss()
        {
            var go = ResourceUtils.GetCutScene();
            await UniTask.Delay(2000);
            statue.SetActive(false);
            await UniTask.Delay(5000);
            LeanPool.Despawn(go);
            SpawnEnemy();
        }
    }
}