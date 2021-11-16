using System.Collections.Generic;
using Map;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public class SpawnControllerBase : MonoBehaviour
    {
#if UNITY_EDITOR
        public List<int> debugSpawnedEnemies;
        public int debugEnemyCount;
#endif
        public MapController MapController { get; set; }
        public static readonly List<int> spawnedEnemies = new List<int>();
        public int KilledEnemy { get; set; }
        //IObservableProperty<int> KilledEnemy

        protected List<HeroBase> Heroes { get; set; }
        protected int CountEnemy { get; set; }
        
        protected readonly Dictionary<int, int> _mapCheckEnemy = new Dictionary<int, int>();


        public HeroBase SpawnHero(HeroButton heroButton, SkillButton skillButton, string heroId, int indexHero)
        {
            if (string.IsNullOrEmpty(heroId))
                heroId = "1001";

            var posSpawn = Vector3.zero;

            HeroBase heroControl = null;


            if (indexHero < MapController.pointSpawnHero.Count)
            {
                posSpawn = MapController.pointSpawnHero[indexHero].position;
            }

            var heroClone = ResourceUtils.GetUnit(heroId, posSpawn, Quaternion.identity, null, false);

            if (heroClone != null)
            {
                heroControl = heroClone.GetComponent<HeroBase>();

                heroControl.InitUI(heroButton, skillButton);

                heroControl.transform.position = posSpawn;
                if (Heroes == null)
                {
                    Heroes = new List<HeroBase>();
                }

                Heroes.Add(heroControl);
            }

            return heroControl;
        }

        public HeroBase GetHeroUnitById(int heroId)
        {
            int index = Heroes.FindIndex(o => o.Id == heroId);
            if (index == -1) return null;
            return Heroes[index];
        }

        public EnemyBase SpawnEnemy(int enemyId, Vector2 spawnPoint, List<Vector3> paths)
        {
            CountEnemy++;
            spawnedEnemies.Add(enemyId);
            //Debug.Log("Spawn Enemy: " + CountEnemy);

            var numbPreLoad = GetNumbPreloadEnemys(enemyId);
            var enemyClone = ResourceUtils.GetUnit(enemyId.ToString(), spawnPoint, Quaternion.identity, null, true,
                numbPreLoad);

            AddEnemyToMapCheck(enemyId);

            var enemyControl = enemyClone.GetComponent<EnemyBase>();
            if (enemyControl.LineController == null)
            {
                var indexRand = Random.Range(0, MapController.lines.Count);
                LineController line = MapController.lines[indexRand];
                enemyControl.LineController = line;
            }

            // Set level follow campaign mode, need update when it's not campaign
            enemyControl.SetInfo(enemyId, GamePlayControllerBase.Instance.ModeId + 1);
            //Debug.Log(
            //    $"<color=orange>Spawned enemy(in path) {enemyId}, init enemy action, current state {enemyControl.UnitState.Current}, init health {enemyControl.Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}</color>");

            enemyControl.GoldDrop = GamePlayData.Instance.GetGoldReceiveKillEnemy(enemyId);

            if (paths != null && paths.Count > 0)
            {
                enemyControl.MoveToEndPoint(paths);
            }

            return enemyControl;
        }

        public EnemyBase SpawnEnemy(int enemyId, Vector2 spawnPoint)
        {
            CountEnemy++;
            spawnedEnemies.Add(enemyId);

            //Debug.Log("Spawn Enemy: " + CountEnemy);

            var enemyClone = ResourceUtils.GetUnit(enemyId.ToString(), spawnPoint, Quaternion.identity);

            AddEnemyToMapCheck(enemyId);

            var enemyControl = enemyClone.GetComponent<EnemyBase>();

            // Set level follow campaign mode, need update when it's not campaign
            enemyControl.SetInfo(enemyId, GamePlayController.Instance.ModeId + 1);
            //Debug.Log(
            //    $"<color=orange>Spawned enemy {enemyId}, init enemy action, current state {enemyControl.UnitState.Current}, init health {enemyControl.Stats.GetStat<HealthPoint>(RPGStatType.Health)}</color>");
            enemyControl.GoldDrop = GamePlayData.Instance.GetGoldReceiveKillEnemy(enemyId);

            return enemyControl;
        }

        public EnemyBase SpawnEnemyRandomLand(string enemyId)
        {
            if (string.IsNullOrEmpty(enemyId))
                enemyId = "3001";

            EnemyBase enemyControl = null;
            var indexRand = Random.Range(0, MapController.lines.Count);
            LineController line = MapController.lines[indexRand];

            if (line != null)
            {
                enemyControl = SpawnEnemy(int.Parse(enemyId), line.GetSpawnPoint(), line.GetPathWaypoints());
                enemyControl.LineController = line;
            }

            return enemyControl;
        }

        public EnemyBase SpawnEnemyLine0(string enemyId)
        {
            if (string.IsNullOrEmpty(enemyId))
                enemyId = "3001";

            EnemyBase enemyControl = null;

            LineController line = MapController.lines[0];

            if (line != null)
            {
                enemyControl = SpawnEnemy(int.Parse(enemyId), line.GetSpawnPoint(), line.GetPathWaypoints());
                enemyControl.LineController = line;
            }

            return enemyControl;
        }

        public EnemyBase SpawnDummy(string dummyId)
        {
            if (string.IsNullOrEmpty(dummyId))
                dummyId = "3001";

            //GameObject enemyClone = ResourceUtils.GetUnit(dummyId);
            EnemyBase enemyBase = SpawnEnemy(int.Parse(dummyId), Vector2.zero, null);
            if (enemyBase != null)
            {
                //var enemyControl = enemyClone.GetComponent<EnemyBase>();

                return enemyBase;
            }

            return null;
        }

        public Building SpawnTower(int towerId, Vector3 position)
        {
            GameObject tower = ResourceUtils.GetUnit($"tower_{towerId}", position, Quaternion.identity);

            if (tower != null)
            {
                var building = tower.GetComponent<Building>();

                return building;
            }

            return null;
        }

        public void AddCountEnemy()
        {
            CountEnemy++;
        }

        private void AddEnemyToMapCheck(int enemyId)
        {
            if (!this._mapCheckEnemy.ContainsKey(enemyId))
            {
                this._mapCheckEnemy.Add(enemyId, 1);
            }
            else
            {
                this._mapCheckEnemy[enemyId]++;
            }
        }

        protected void RemoveEnemyToMapCheck(int enemyId)
        {
            if (this._mapCheckEnemy.ContainsKey(enemyId))
            {
                this._mapCheckEnemy[enemyId]--;
            }
        }

        #region Game Play

        public virtual void SpawnEnemy(int enemyId, int lineId)
        {
            if (enemyId == 0) return;

            EnemyBase enemyControl;

            var line = MapController.lines[lineId];

            if (line != null)
            {
                enemyControl = SpawnEnemy(enemyId, line.GetSpawnPoint(), line.GetPathWaypoints());
                enemyControl.LineController = line;
            }
        }

        private int GetNumbPreloadEnemys(int enemyId)
        {
            if (this._mapCheckEnemy.ContainsKey(enemyId)) return 0;

            var dictEnemy = new Dictionary<int, int>();

            if (GamePlayControllerBase.gameMode == GameMode.CampaignMode)
            {
                dictEnemy = GamePlayData.Instance.CurrentMapCampaign.GetTotalEnemyInMap();
            }
            else if (GamePlayControllerBase.gameMode == GameMode.CampaignMode)
            {
                dictEnemy = GameContainer.Instance.GetMapDefensiveData(GamePlayControllerBase.CampaignId)
                    .GetTotalEnemyInMap();
            }

            foreach (var enemyInMap in dictEnemy)
            {
                if (enemyId == enemyInMap.Key)
                {
                    return Mathf.RoundToInt((0.5f * enemyInMap.Value * 1f));
                }
            }

            return 0;
        }

        #endregion
#if UNITY_EDITOR
        private void Update()
        {

            debugEnemyCount = CountEnemy;
            debugSpawnedEnemies = spawnedEnemies;

        }
#endif
    }
}