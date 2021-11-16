using System;
using System.Collections.Generic;
using Invoke;
using Lean.Pool;
using TigerForge;

namespace EW2
{
    public class DefenseModeGamePlayWindow : GamePlayWindowCommon
    {
        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            lbWave.text = "0";

            SpawnMap();

            InvokeProxy.Iinvoke.Invoke(this, SpawnHeroList, 1f);

            GamePlayController.Instance.State = GamePlayState.Init;

            PlayAmbienceMusic(Properties.campaignId);
        }

        #region Control

        protected void SpawnMap()
        {
            var mapId = Properties.campaignId;

            if (GamePlayController.Instance == null) return;

            if (GamePlayController.Instance.SpawnController.MapController != null)
            {
                if (GamePlayController.Instance.SpawnController.MapController.MapId == mapId)
                {
                    return;
                }

                LeanPool.Despawn(GamePlayController.Instance.SpawnController.MapController.gameObject);
            }
            else if (GamePlayController.Instance.SpawnController.MapController == null)
            {
                GamePlayController.Instance.SpawnController.MapController =
                    ResourceUtils.GetMapDefenseModePrefab(mapId);
            }

            GamePlayController.Instance.SpawnController.MapController.MapId = mapId;

            // setup camera
            SetUpCamera();

            InitListenChange();

            // set map data

            GamePlayData.Instance.SetMapDefenseModeData();

            CallWave.Instance.LoadMapDefenseModeData();

            StartWaveButtonController.Instance.InitButtonCallWave(GamePlayController.Instance.SpawnController
                .MapController.PointButtonSpawn);
        }

        private void InitListenChange()
        {
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                OnGoldChange);

            EventManager.StartListening(GamePlayEvent.OnHealthPointDFPUpdate, HandleLifePointChange);

            CallWave.Instance.waveChanged.ValueChanged += HandleWaveChange;

            CallWave.Instance.onShowNewEnemy = ShowNewEnemy;
        }

        private void HandleLifePointChange()
        {
            var bigDisplay =
                new BigNumberDisplay() {Value = EventManager.GetData<int>(GamePlayEvent.OnHealthPointDFPUpdate)};
            lbHealth.text = $"{bigDisplay.ConvertToString()}";
        }

        private void ShowNewEnemy(List<int> listNewEnemy)
        {
            if (btnNewEnemy)
            {
                btnNewEnemy.ShowButton(listNewEnemy);
            }
        }

        private void HandleWaveChange(object sender, EventArgs e)
        {
            lbWave.text = $"{CallWave.Instance.waveChanged.Value.ToString()}";
        }

        private void OnGoldChange()
        {
            lbGold.text = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold).ToString();
        }

        #endregion
    }
}