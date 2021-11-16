using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class TournamentCallWave : CallWave
    {
        public override void Execute()
        {
            StartWave();
        }

        public override void LoadTournamentMapData(int tournamentMapId)
        {
            this._currWave = -1;
            this._dictMapData.Clear();
            this._listEnemyUnlock.Clear();
            this._dictMapLoopData.Clear();
            this._countLoop = 0;
            var mapData = GameContainer.Instance.GetTournamentMapData(GamePlayControllerBase.CampaignId);
            if (mapData != null)
            {
                this._dictMapData = mapData.GetMapData();
                this._dictMapLoopData = mapData.GetMapLoopData();
            }

            waveChanged.Value = 1;
        }

        protected override IEnumerator DelaySpawnEnemy(WaveInfo waveInfo)
        {
            for (int i = 0; i < waveInfo.amount; i++)
            {
                TournamentGamePlayController.Instance.SpawnController.SpawnEnemy(waveInfo.enemyId, waveInfo.line);
                
                if (waveInfo.spacing > 0)
                    yield return new WaitForSeconds(waveInfo.spacing);
            }
        }

        protected override void SpawnWave()
        {
            //Debug.LogAssertion($"Start wave {this._currWave + 1}");

            startSpawnWave?.Invoke(this._currWave + 1);

            var listWave = this._dictMapData[this._currWave];

            CheckShowNewEnemy(listWave);

            this._dictLineData.Clear();

            //filter line
            for (int i = 0; i < listWave.Count; i++)
            {
                var waveInfo = listWave[i];

                if (!this._dictLineData.ContainsKey(waveInfo.line))
                {
                    var listWaveInLine = new List<WaveInfo>();

                    listWaveInLine.Add(waveInfo);

                    this._dictLineData.Add(waveInfo.line, listWaveInLine);
                }
                else
                {
                    this._dictLineData[waveInfo.line].Add(waveInfo);
                }
            }

            // spawn enemy in line
            this._numberLineSpawnEnemy = this._dictLineData.Count;

            foreach (var listEnemyInLine in this._dictLineData.Values)
            {
                var coroutine = CoroutineUtils.Instance.StartCoroutine(SpawnEnemyInLine(listEnemyInLine, () => {
                    this._numberLineSpawnEnemy--;

                    if (this._numberLineSpawnEnemy <= 0)
                    {
                        if (this._currWave < this._dictMapData.Count - 1)
                        {
                            WaitToNextWave();
                        }
                        else
                        {
                            if (this._dictMapLoopData.Count > 0)
                            {
                                AddLoopWave();
                                WaitToNextWave();
                            }
                            else
                            {
                                EventManager.EmitEvent(GamePlayEvent.OnEndSpawn);
                            }
                        }
                    }
                }));

                if (coroutine != null)
                    this._coroutineManager.Add(coroutine);
            }
        }
    }
}