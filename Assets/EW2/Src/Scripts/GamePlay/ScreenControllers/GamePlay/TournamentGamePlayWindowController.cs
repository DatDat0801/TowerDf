using System;
using System.Collections.Generic;
using Invoke;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class TournamentGamePlayWindowController : GamePlayWindowCommon
    {
        [SerializeField] private Text scoreLabel;
        [SerializeField] private Text scoreTxt;
        public HeroButton GetSecondHeroBtn() => listButtonHero[1];
        public SkillButton GetFirstHeroSkillBtn() => listButtonSkill[0];
        public SkillButton GetSecondHeroSkillBtn() => listButtonSkill[1];

        protected override void Awake()
        {
            base.Awake();
        }

        public override void SetLocalization()
        {
            base.SetLocalization();
            this.scoreLabel.text = L.gameplay.score;
            if (this.scoreTxt) this.scoreTxt.text = 0.ToString();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SpawnMap(Properties.campaignId);

            InvokeProxy.Iinvoke.Invoke(this, SpawnHeroList, 1f);

            GamePlayController.Instance.State = GamePlayState.Init;

            PlayAmbienceMusic(Properties.campaignId);
            SetLocalization();
        }

        private void OnEnable()
        {
            //Disable setting button on tutorial map 0
            //btnSetting.gameObject.SetActive(!TutorialManager.Instance.IsLockUpgradeTower);
            EventManager.StartListening(GamePlayEvent.OnEnemyKill, OnEnemyKilled);
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                OnGoldChange);
            EventManager.StartListening(
                GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.LifePoint), HandleEventEnemyPass);
        }


        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnEnemyKill, OnEnemyKilled);
            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.Gold),
                OnGoldChange);
            EventManager.StopListening(GamePlayEvent.OnMoneyChange(ResourceType.MoneyInGame, MoneyInGameType.LifePoint),
                HandleEventEnemyPass);
            DisableButtons();
        }

        protected override void SpawnHeroList()
        {
            base.SpawnHeroList();
            var heroes = TournamentGamePlayController.Instance.heroes;
            foreach (HeroBase heroBase in heroes)
            {
                SetBuffNerf(heroBase);
                EventManager.StartListening(GamePlayEvent.onHeroRevive, OnHeroRevive);
            }
        }

        private void OnHeroRevive()
        {
            HeroBase hero = EventManager.GetData<HeroBase>(GamePlayEvent.onHeroRevive);
            SetBuffNerf(hero);
        }

        private void OnEnemyKilled()
        {
            GamePlayControllerBase.Instance.SpawnController.KilledEnemy += 1;
            if (this.scoreTxt)
            {
                this.scoreTxt.text = GamePlayControllerBase.Instance.SpawnController.KilledEnemy.ToString();
            }
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
            var campaignId = GameContainer.Instance.GetTournamentMapData(mapId).mapConfigs.campaignMapId;
            if (TournamentGamePlayController.Instance == null) return;

            if (TournamentGamePlayController.Instance.SpawnController.MapController != null)
            {
                if (TournamentGamePlayController.Instance.SpawnController.MapController.MapId == mapId)
                {
                    return;
                }

                LeanPool.Despawn(TournamentGamePlayController.Instance.SpawnController.MapController.gameObject);
            }
            else if (TournamentGamePlayController.Instance.SpawnController.MapController == null)
            {
                TournamentGamePlayController.Instance.SpawnController.MapController =
                    ResourceUtils.GetMapPrefab(mapInfo.Item1, campaignId);
            }

            TournamentGamePlayController.Instance.SpawnController.MapController.MapId = mapId;

            // setup camera
            SetUpCamera();

            InitListenChange();

            // set map data
            //TODO Get tournament map id for this season
            GamePlayData.Instance.TournamentMapData =
                GameContainer.Instance.GetTournamentMapData(Properties.campaignId);
            //GetMapData(mapInfo.Item1, mapInfo.Item2, mapInfo.Item3);

            GamePlayData.Instance.SetMapTournamentData();

            if (GamePlayData.Instance.TournamentMapData != null)
            {
                TournamentCallWave.Instance.LoadTournamentMapData(Properties.campaignId);

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
            //lbWave.text = $"{CallWave.Instance.waveChanged.Value.ToString()}/{CallWave.Instance.GetMaxWave().ToString()}";
            lbWave.text = $"{CallWave.Instance.waveChanged.Value.ToString()}";
        }

        private void HandleEventEnemyPass()
        {
            lbHealth.text = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint).ToString();
        }

        private void OnGoldChange()
        {
            lbGold.text = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold).ToString();
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

        /// <summary>
        /// Buff in tournament
        /// </summary>
        /// <param name="heroBase"></param>
        private void BuffHero(HeroBase heroBase)
        {
            var buffData = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentBuffConfig>();
            var userBuffData = UserData.Instance.TournamentData;
            var buff = buffData.GetBuffDataById(userBuffData.buffStatId);
            var statType = buff.statBonus;
            //var stat = heroBase.Stats.GetStat(statType);
            RPGStatModifier statModifier;
            var modifiable = Ultilities.GetStatModifiable(statType);
            if (buff.statBonus == RPGStatType.CooldownReduction)
            {
                statModifier = new RPGStatModifier(modifiable, ModifierType.TotalAdd, buff.ratioBonus, false);
            }
            else
            {
                statModifier = buff.valueBonus > 0
                    ? new RPGStatModifier(modifiable, ModifierType.TotalAdd, buff.valueBonus, false)
                    : new RPGStatModifier(modifiable, ModifierType.TotalPercent, buff.ratioBonus, false);
            }

            heroBase.Stats.AddStatModifier(statType, statModifier);

            //Debug.LogAssertion($"Buff stat on {heroBase.Id}: {heroBase.Stats.GetStat(statType).StatValue} {statType}");
        }

        private void NerfHero(HeroBase heroBase)
        {
            var nerfConfig = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentNerfConfig>();
            var userBuffData = UserData.Instance.TournamentData;
            var nerf = nerfConfig.GetNerfDataById(userBuffData.nerfStatId);
            var statType = nerf.statBonus;
            //var stat = heroBase.Stats.GetStat(statType);
            RPGStatModifier statModifier;
            var modifiable = Ultilities.GetStatModifiable(statType);
            if (nerf.ratioBonus > 0)
            {
                statModifier = new RPGStatModifier(modifiable, ModifierType.TotalPercent, -nerf.ratioBonus,
                    false);
            }
            else
            {
                statModifier = new RPGStatModifier(modifiable, ModifierType.TotalAdd, -nerf.valueBonus, false);
            }

            heroBase.Stats.AddStatModifier(statType, statModifier);
        }

        private void SetBuffNerf(HeroBase hero)
        {
            var userData = UserData.Instance.TournamentData;

            if (hero.Id == userData.heroNerfId)
            {
                //nerf
                NerfHero(hero);
            }
            
            if(!userData.CanBuffHeroes()) return;
            
            var heroBuff = userData.listHeroBuff.Find(data => data.heroId == hero.Id);
            if (heroBuff != null)
            {
                //buff
                BuffHero(hero);
            }
        }
    }
}