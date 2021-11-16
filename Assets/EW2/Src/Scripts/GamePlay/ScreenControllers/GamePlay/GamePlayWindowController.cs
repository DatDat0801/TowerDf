using System;
using System.Collections.Generic;
using EW2.Tutorial.General;
using Hellmade.Sound;
using Invoke;
using Lean.Pool;
using TigerForge;
using UnityEngine;


namespace EW2
{
    public class GamePlayWindowController : GamePlayWindowCommon
    {
        [SerializeField] private GameObject objFinalWave;


        public HeroButton GetSecondHeroBtn() => listButtonHero[1];
        public SkillButton GetFirstHeroSkillBtn() => listButtonSkill[0];
        public SkillButton GetSecondHeroSkillBtn() => listButtonSkill[1];

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SpawnMap(Properties.campaignId);

            InvokeProxy.Iinvoke.Invoke(this, SpawnHeroList, 1f);

            GamePlayController.Instance.State = GamePlayState.Init;

            PlayAmbienceMusic(Properties.campaignId);

            if (GamePlayController.IsTrialCampaign && !UserData.Instance.AccountData.isCompleteTutTrial)
            {
                TutorialManager.Instance.ExecuteStepTutorial(AnyTutorialConstants.TRIAL_HERO_DESC);
            }
        }

        private void OnEnable()
        {
            //Disable setting button on tutorial map 0
            btnSetting.gameObject.SetActive(!TutorialManager.Instance.IsLockUpgradeTower);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                OnGoldChange);

            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.LifePoint),
                HandleEventEnemyPass);

            DisableButtons();
        }


        private void DisableButtons()
        {
            _numberHeroInScene = 0;

            foreach (var heroButton in listButtonHero)
            {
                heroButton.gameObject.SetActive(false);
            }

            foreach (var skillButton in listButtonSkill)
            {
                skillButton.gameObject.SetActive(false);
            }

            btnNewEnemy.gameObject.SetActive(false);
        }

        protected void SpawnMap(int mapId)
        {
            var mapInfo = MapCampaignInfo.GetWorldMapModeId(mapId); // world map id, map id, mode id

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
                    ResourceUtils.GetMapPrefab(mapInfo.Item1, mapInfo.Item2);
            }

            GamePlayController.Instance.SpawnController.MapController.MapId = mapId;
            
            // setup camera
            SetUpCamera();

            InitListenChange();

            // set map data
            GamePlayData.Instance.CurrentMapCampaign =
                GameContainer.Instance.GetMapData(mapInfo.Item1, mapInfo.Item2, mapInfo.Item3);

            GamePlayData.Instance.SetCampaignMapData();

            if (GamePlayData.Instance.CurrentMapCampaign != null)
            {
                CallWave.Instance.LoadMapData();

                StartWaveButtonController.Instance.InitButtonCallWave(GamePlayController.Instance.SpawnController
                    .MapController.PointButtonSpawn);
            }
        }

        private void InitListenChange()
        {
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                OnGoldChange);

            EventManager.StartListening(
                GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.LifePoint), HandleEventEnemyPass);

            CallWave.Instance.waveChanged.ValueChanged += HandleWaveChange;

            CallWave.Instance.onFinalWave = ShowWarningFinalWave;

            CallWave.Instance.onShowNewEnemy = ShowNewEnemy;
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
            lbWave.text =
                $"{CallWave.Instance.waveChanged.Value.ToString()}/{CallWave.Instance.GetMaxWave().ToString()}";
        }

        private void HandleEventEnemyPass()
        {
            lbHealth.text = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint).ToString();
        }

        private void OnGoldChange()
        {
            lbGold.text = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold).ToString();
        }

        private void ShowWarningFinalWave()
        {
            if (objFinalWave)
            {
                objFinalWave.SetActive(true);
                AudioClip clip = ResourceUtils.LoadSound(SoundConstant.FINAL_WAVE);
                EazySoundManager.PlaySound(clip);
                InvokeProxy.Iinvoke.Invoke(this, () => { objFinalWave.SetActive(false); }, 2f);
            }
        }

        public void DeselectAllHero()
        {
            foreach (var heroButton in listButtonHero)
            {
                heroButton.Deselect();
            }
        }

        public void DeselectAllButtonSkills()
        {
            foreach (SkillButton skillButton in this.listButtonSkill)
            {
                skillButton.Close();
            }
        }
    }
}