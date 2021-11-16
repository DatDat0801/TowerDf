using System.Collections.Generic;
using Newtonsoft.Json;
using TigerForge;

namespace EW2
{
    public class GamePlayData
    {
        private static GamePlayData _instance;

        public static GamePlayData Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GamePlayData();
                return _instance;
            }
        }

        private GamePlayData()
        {
            GamePlayResources = new ResourceController();
        }

        private Dictionary<int, int> dictEnemyDropGold = new Dictionary<int, int>();

        private int goldRemainAfterWave;

        private List<int> goldDropInfo;

        public MapCampaignInfo CurrentMapCampaign { get; set; }
        
        //Tournament Map
        public TournamentMapData TournamentMapData { get; set; }

        private ResourceController GamePlayResources { get; }

        public long GetMoneyInGame(int moneyInGameType)
        {
            return GamePlayResources.GetMoneyInGame(moneyInGameType);
        }

        public void AddMoneyInGame(int moneyInGameType, long value)
        {
            GamePlayResources.Add(ResourceType.MoneyInGame, moneyInGameType, value);
        }

        public void SubMoneyInGame(int moneyInGameType, long value)
        {
            GamePlayResources.Sub(ResourceType.MoneyInGame, moneyInGameType, value);
        }

        public void ClearResourceInGame()
        {
            GamePlayResources.Clear(ResourceType.MoneyInGame);
        }

        public void SetCampaignMapData()
        {
            var dataStatBase = CurrentMapCampaign.GetMapStatBase();

            AddMoneyInGame(MoneyInGameType.Gold, dataStatBase.gold);

            AddMoneyInGame(MoneyInGameType.LifePoint, dataStatBase.lifePoint);

            var worldMapGoldDrop =
                GameContainer.Instance.GetWorldMapGoldDrop(CurrentMapCampaign.worldId, CurrentMapCampaign.modeId);

            goldDropInfo = worldMapGoldDrop.GetGoldDropByMapId(CurrentMapCampaign.stageId);
        }

        public void SetMapDefenseModeData()
        {
            var currDfpId = UserData.Instance.UserHeroDefenseData.defensePointId;
            var dataStatBase = GameContainer.Instance.Get<UnitDataBase>().GetDefensivePointData(currDfpId);
            var goldInWave = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>().GetDataConfig().goldDrop;
            AddMoneyInGame(MoneyInGameType.Gold, goldInWave);
            EventManager.EmitEventData(GamePlayEvent.OnHealthPointDFPUpdate, dataStatBase.stats[0].hp);

        }

        public void SetMapTournamentData()
        {
            var dataStatBase = this.TournamentMapData.GetTournamentMapConfig();

            AddMoneyInGame(MoneyInGameType.Gold, dataStatBase.goldBase);

            AddMoneyInGame(MoneyInGameType.LifePoint, dataStatBase.lifePoint);

            var tournamentGoldDropData = GameContainer.Instance.GetTournamentGoldDropData( this.TournamentMapData.tournamentMapId);
            //GameContainer.Instance.GetWorldMapGoldDrop(this.TournamentMapData.worldId, this.TournamentMapData.modeId);

            goldDropInfo = tournamentGoldDropData.GetGoldDrops();
        }

        public void CalculateGoldDropInWave(int wave, List<WaveInfo> waveInfo)
        {
            dictEnemyDropGold.Clear();


            if (goldDropInfo != null)
            {
                var goldInWave = GetGoldDropInWave(wave);

                goldRemainAfterWave = goldInWave;

                var dictEnemyInWave = new Dictionary<int, int>();

                foreach (var subWave in waveInfo)
                {
                    if (!dictEnemyInWave.ContainsKey(subWave.enemyId))
                    {
                        dictEnemyInWave.Add(subWave.enemyId, subWave.amount);
                    }
                    else
                    {
                        dictEnemyInWave[subWave.enemyId] += subWave.amount;
                    }
                }


                // finish calculate gold drop
                var totalWeight = CaculateTotalWeight(dictEnemyInWave);

                foreach (var enemyInWave in dictEnemyInWave)
                {
                    if (!dictEnemyDropGold.ContainsKey(enemyInWave.Key))
                    {
                        var enemyId = enemyInWave.Key;

                        var numberEnemyInWave = enemyInWave.Value;

                        // hardcore -- GD confirm be the same for all levels
                        var weight = GameContainer.Instance.Get<UnitDataBase>().GetEnemyById(enemyId).GetStats(1)
                            .weight;

                        var goldForEnemy = GoldDropOfEnemy(goldInWave, totalWeight, weight, numberEnemyInWave);

                        goldRemainAfterWave -= numberEnemyInWave * goldForEnemy;

                        dictEnemyDropGold.Add(enemyInWave.Key, goldForEnemy);
                    }
                }
            }
        }

        public void CalculateGoldDropHeroDefenseInWave(List<WaveInfo> waveInfo)
        {
            dictEnemyDropGold.Clear();


            var goldInWave = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>().GetDataConfig().goldDrop;

            goldRemainAfterWave = goldInWave;

            var dictEnemyInWave = new Dictionary<int, int>();

            foreach (var subWave in waveInfo)
            {
                if (!dictEnemyInWave.ContainsKey(subWave.enemyId))
                {
                    dictEnemyInWave.Add(subWave.enemyId, subWave.amount);
                }
                else
                {
                    dictEnemyInWave[subWave.enemyId] += subWave.amount;
                }
            }


            // finish calculate gold drop
            var totalWeight = CaculateTotalWeight(dictEnemyInWave);

            foreach (var enemyInWave in dictEnemyInWave)
            {
                if (!dictEnemyDropGold.ContainsKey(enemyInWave.Key))
                {
                    var enemyId = enemyInWave.Key;

                    var numberEnemyInWave = enemyInWave.Value;

                    // hardcore -- GD confirm be the same for all levels
                    var weight = GameContainer.Instance.Get<UnitDataBase>().GetEnemyById(enemyId).GetStats(1)
                        .weight;

                    var goldForEnemy = GoldDropOfEnemy(goldInWave, totalWeight, weight, numberEnemyInWave);

                    goldRemainAfterWave -= numberEnemyInWave * goldForEnemy;

                    dictEnemyDropGold.Add(enemyInWave.Key, goldForEnemy);
                }
            }
        }
        public void CalculateGoldDropTournamentInWave(List<WaveInfo> waveInfo, int tournamentMapId,int currentWave)
        {
            dictEnemyDropGold.Clear();


            var goldInWave = GameContainer.Instance.GetTournamentGoldDropData(tournamentMapId)
                .GetGoldDropByWaveId(currentWave);

            goldRemainAfterWave = goldInWave;

            var dictEnemyInWave = new Dictionary<int, int>();

            foreach (var subWave in waveInfo)
            {
                if (!dictEnemyInWave.ContainsKey(subWave.enemyId))
                {
                    dictEnemyInWave.Add(subWave.enemyId, subWave.amount);
                }
                else
                {
                    dictEnemyInWave[subWave.enemyId] += subWave.amount;
                }
            }


            // finish calculate gold drop
            var totalWeight = CaculateTotalWeight(dictEnemyInWave);

            foreach (var enemyInWave in dictEnemyInWave)
            {
                if (!dictEnemyDropGold.ContainsKey(enemyInWave.Key))
                {
                    var enemyId = enemyInWave.Key;

                    var numberEnemyInWave = enemyInWave.Value;

                    // hardcore -- GD confirm be the same for all levels
                    var weight = GameContainer.Instance.Get<UnitDataBase>().GetEnemyById(enemyId).GetStats(1)
                        .weight;

                    var goldForEnemy = GoldDropOfEnemy(goldInWave, totalWeight, weight, numberEnemyInWave);

                    goldRemainAfterWave -= numberEnemyInWave * goldForEnemy;

                    dictEnemyDropGold.Add(enemyInWave.Key, goldForEnemy);
                }
            }
        }

        private int CaculateTotalWeight(Dictionary<int, int> dictEnemy)
        {
            var totalWeight = 0;

            foreach (var enemyId in dictEnemy.Keys)
            {
                // hardcore -- GD confirm be the same for all levels
                var enemyData = GameContainer.Instance.Get<UnitDataBase>().GetEnemyById(enemyId).GetStats(1);

                totalWeight += enemyData.weight;
            }

            return totalWeight;
        }

        private int GoldDropOfEnemy(int totalGoldInWave, int totalWeight, int weight, int numberEnemyInWave)
        {
            var goldDrop = 0;

            goldDrop = (totalGoldInWave * weight) / (totalWeight * numberEnemyInWave);

            return goldDrop;
        }

        private int GetGoldDropInWave(int wave)
        {
            if (this.goldDropInfo.Count > 0)
            {
                return this.goldDropInfo[wave];
            }

            return 0;
        }

        public int GetGoldReceiveKillEnemy(int enemyId)
        {
            return dictEnemyDropGold.ContainsKey(enemyId) ? dictEnemyDropGold[enemyId] : 0;
        }

        public int GetGoldRemainAfterWave()
        {
            return goldRemainAfterWave;
        }
    }
}