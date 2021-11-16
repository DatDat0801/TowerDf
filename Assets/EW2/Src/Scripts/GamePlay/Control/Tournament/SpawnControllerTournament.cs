using Invoke;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class SpawnControllerTournament : SpawnControllerBase
    {
        private void OnEnable()
        {
            KilledEnemy = 0;
        }

        private void Start()
        {
            CountEnemy = 0;
            EventManager.StartListening(GamePlayEvent.OnEnemyDie, OnEnemyDie);
        }

        protected void OnDestroy()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
            EventManager.StopListening(GamePlayEvent.OnEnemyDie, OnEnemyDie);

        }

        private void OnEnemyDie()
        {
            CountEnemy -= 1;
            var enemyId = EventManager.GetInt(GamePlayEvent.OnEnemyDie);
            RemoveEnemyToMapCheck(enemyId);

        }
        public override void SpawnEnemy(int enemyId, int lineId)
        {
            if (enemyId == 0) return;

            var line = MapController.lines[lineId];

            if (line != null)
            {
                EnemyBase enemy = SpawnEnemy(enemyId, line.GetSpawnPoint(), line.GetPathWaypoints());
                enemy.LineController = line;
                
                var mapData = GameContainer.Instance.GetTournamentMapData(GamePlayControllerBase.CampaignId);
                enemy.BuffEnemyTournamentGrowth(mapData.GetTournamentEnemyConfig(), CallWave.Instance.CountLoop);
                //Debug.LogAssertion($"Count loop: " + CallWave.Instance.CountLoop);
            }
        }
        
    }
}