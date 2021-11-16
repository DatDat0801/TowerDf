using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invoke;
using TigerForge;
using UnityEngine;
using Zitga.Observables;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class CallWave : Singleton<CallWave>
    {
        public ObservableProperty<int> waveChanged = new ObservableProperty<int>();

        protected Dictionary<int, List<WaveInfo>> _dictMapData = new Dictionary<int, List<WaveInfo>>();

        protected Dictionary<int, List<WaveInfo>> _dictMapLoopData = new Dictionary<int, List<WaveInfo>>();

        protected Dictionary<int, List<WaveInfo>> _dictLineData = new Dictionary<int, List<WaveInfo>>();

        protected List<int> _listEnemyUnlock = new List<int>();

        protected int _currWave;
        public int CurrWave => this._currWave;

        protected int _numberLineSpawnEnemy;

        protected Coroutine _coroutineWaitShowButton;

        protected List<Coroutine> _coroutineManager = new List<Coroutine>();

        public Action<int> startSpawnWave;

        public Action onFinalWave;

        public Action<List<int>> onShowNewEnemy;

        protected int _unitId;

        protected int _countLoop;

        public int CountLoop => this._countLoop;

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

        protected void StartWave()
        {
            if (this._currWave < this._dictMapData.Count - 1)
            {
                this._currWave++;

                if (this._currWave == 0)
                {
                    Debug.LogWarning($"First Wave");

                    EventManager.EmitEvent(GamePlayEvent.OnStartGame);

                    GamePlayController.Instance.State = GamePlayState.Called;
                }

                if (this._currWave == this._dictMapData.Count - 1)
                {
                    Debug.LogWarning($"Final Wave");

                    onFinalWave?.Invoke();
                }
            }
            else
            {
                Debug.LogWarning($"End wave");
            }

            CancelAllDelay();

            waveChanged.Value = this._currWave + 1;

            if (GamePlayController.Instance.SpawnController.MapController)
            {
                if (GamePlayControllerBase.gameMode == GameMode.CampaignMode)
                {
                    GamePlayData.Instance.CalculateGoldDropInWave(this._currWave, this._dictMapData[this._currWave]);
                }
                else if (GamePlayControllerBase.gameMode == GameMode.DefenseMode)
                {
                    GamePlayData.Instance.CalculateGoldDropHeroDefenseInWave(this._dictMapData[this._currWave]);
                }else if (GamePlayControllerBase.gameMode == GameMode.TournamentMode)
                {
                    GamePlayData.Instance.CalculateGoldDropTournamentInWave(this._dictMapData[this._currWave], GamePlayControllerBase.CampaignId, this._currWave);
                }

                SpawnWave();
                TrackingWaveEnd();
            }
        }

        void TrackingWaveEnd()
        {
            FirebaseLogic.Instance.WaveEnd(GamePlayControllerBase.CampaignId, CallWave.Instance.CurrWave + 1,
                (int)GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint),
                GamePlayController.Instance.TotalUseSpellInWave);
            GamePlayController.Instance.TotalUseSpellInWave = 0;
        }

        public void LoadMapData()
        {
            this._currWave = -1;

            this._dictMapData.Clear();

            this._listEnemyUnlock.Clear();

            this._dictMapLoopData.Clear();

            if (GamePlayData.Instance.CurrentMapCampaign != null)
            {
                this._dictMapData = GamePlayData.Instance.CurrentMapCampaign.GetMapData();

                this._listEnemyUnlock = GamePlayData.Instance.CurrentMapCampaign.GetEnemyUnlock();
            }

            waveChanged.Value = 1;
        }

        public virtual void LoadTournamentMapData(int tournamentMapId)
        {
            throw new NotImplementedException();
        }

        public virtual void LoadMapDefenseModeData()
        {
            throw new NotImplementedException();
        }

        protected virtual void SpawnWave()
        {
            throw new NotImplementedException();
        }

        protected IEnumerator SpawnEnemyInLine(List<WaveInfo> listEnemyInLine, Action onComplete)
        {
            for (int i = 0; i < listEnemyInLine.Count; i++)
            {
                var waveInfo = listEnemyInLine[i];

                if (waveInfo.delaySpawn > 0)
                    yield return new WaitForSeconds(waveInfo.delaySpawn);

                if (waveInfo.spacing > 0)
                    yield return new WaitForSeconds(waveInfo.spacing);

                var coroutine = CoroutineUtils.Instance.StartCoroutine(DelaySpawnEnemy(waveInfo));

                if (coroutine != null)
                    this._coroutineManager.Add(coroutine);

                yield return coroutine;
            }

            onComplete?.Invoke();

            yield return null;
        }

        protected virtual IEnumerator DelaySpawnEnemy(WaveInfo waveInfo)
        {
            throw new NotImplementedException();
        }

        protected void WaitToNextWave()
        {
            var goldRemain = GamePlayData.Instance.GetGoldRemainAfterWave();

            GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.Gold, goldRemain);

            this._coroutineWaitShowButton = CoroutineUtils.Instance.StartCoroutine(WaitToShowButtonCallWave());

            InvokeProxy.Iinvoke.Invoke(this, Execute, GameConfig.TimeDelayStartWave);
        }

        protected IEnumerator WaitToShowButtonCallWave()
        {
            yield return new WaitForSeconds(GameConfig.TimeDelayShowCallWave);

            StartWaveButtonController.Instance.ShowButtonCallWave();
        }

        protected void CancelAllDelay()
        {
            if (CoroutineUtils.Instance != null)
            {
                if (this._coroutineWaitShowButton != null)
                    CoroutineUtils.Instance.StopCoroutine(this._coroutineWaitShowButton);

                foreach (var coroutine in this._coroutineManager)
                {
                    CoroutineUtils.Instance.StopCoroutine(coroutine);
                }
            }

            this._coroutineManager.Clear();

            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this, Execute);
        }

        public int GetMaxWave()
        {
            return this._dictMapData.Count;
        }

        public Dictionary<int, int> GetListEnemyInLane(int laneId)
        {
            var listWave = this._dictMapData[this._currWave + 1];

            var listResult = new Dictionary<int, int>();

            foreach (var waveInfo in listWave)
            {
                if (waveInfo.gateSpawnId == laneId)
                {
                    if (!listResult.ContainsKey(waveInfo.enemyId))
                    {
                        listResult.Add(waveInfo.enemyId, waveInfo.amount);
                    }
                    else
                    {
                        listResult[waveInfo.enemyId] += waveInfo.amount;
                    }
                }
            }

            return listResult;
        }

        protected void CheckShowNewEnemy(List<WaveInfo> listWave)
        {
            var listFillter = new List<int>();

            foreach (var waveInfo in listWave.Where(waveInfo =>
                !listFillter.Contains(waveInfo.enemyId) && this._listEnemyUnlock.Contains(waveInfo.enemyId) &&
                !UserData.Instance.CampaignData.CheckEnemyUnlocked(waveInfo.enemyId)))
            {
                listFillter.Add(waveInfo.enemyId);

                this._listEnemyUnlock.Remove(waveInfo.enemyId);

                UserData.Instance.CampaignData.AddEnemyUnlocked(waveInfo.enemyId);
            }

            if (listFillter.Count > 0)
            {
                onShowNewEnemy?.Invoke(listFillter);
            }
        }

        private void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);

            CancelAllDelay();
        }

        protected void AddLoopWave()
        {
            this._countLoop++;
            
            var wave = this._currWave;

            foreach (var waveInfo in this._dictMapLoopData.Values)
            {
                wave++;
                this._dictMapData.Add(wave, waveInfo);
            }
        }
    }
}