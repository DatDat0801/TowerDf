namespace EW2
{
    public class SpawnControllerDefenseMode :  SpawnControllerBase
    {
        public override void SpawnEnemy(int enemyId, int lineId)
        {
            if (enemyId == 0) return;

            var line = MapController.lines[lineId];

            if (line != null)
            {
                EnemyBase enemy = SpawnEnemy(enemyId, line.GetSpawnPoint(), line.GetPathWaypoints());
                enemy.LineController = line;
                
                var mapData = GameContainer.Instance.GetMapDefensiveData(GamePlayControllerBase.CampaignId);
                enemy.BuffEnemyDefensiveMode(mapData.GetDefensiveEnemyConfig(), CallWave.Instance.CountLoop);
                //Debug.LogAssertion($"Count loop: " + CallWave.Instance.CountLoop);
            }
        }
    }
}