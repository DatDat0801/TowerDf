using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class GamePlayController : GamePlayControllerBase
    {
        public static bool IsTrialCampaign { get; set; }
        
        private readonly List<Building> buildings = new List<Building>();
        
        // heroId - originExpPercentage - currentExpPercentage - numberLevelUp
        private Dictionary<int, (float, float, int)> infoAddHeroExp;
        
        private int EarlyWave { get; set; }
        
        private Dictionary<int, int> listSpellUse = new Dictionary<int, int>();

        private void Awake()
        {
            infoAddHeroExp = new Dictionary<int, (float, float, int)>();
            Speed = 1;

            (WorldId, MapId, ModeId) = MapCampaignInfo.GetWorldMapModeId(CampaignId);

            playMode = PlayMode.None;
        }

        private void Start()
        {
            UIFrame.Instance.SwitchCamera(true);
            SpawnGamePlayUi();
            EventManager.StartListening(GamePlayEvent.OnEndGame, OnEndGame);
            EventManager.StartListening(GamePlayEvent.OnSubMoney(ResourceType.MoneyInGame, MoneyInGameType.LifePoint),
                OnLifePointChange);
        }

        public override void SpawnGamePlayUi()
        {
            if (gameMode == GameMode.CampaignMode)
            {
                UIFrame.Instance.OpenWindow(ScreenIds.game_play, new GamePlayWindowProperties(CampaignId));
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventManager.StopListening(GamePlayEvent.OnEndGame, OnEndGame);
            EventManager.StopListening(GamePlayEvent.OnSubMoney(ResourceType.MoneyInGame, MoneyInGameType.LifePoint),
                OnLifePointChange);

            ResetResourceInGame();
        }



        private void OnLifePointChange()
        {
            var lifePoint = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint);

            if (lifePoint <= 0)
            {
                OnEndGame();
            }
        }

        private void OnEndGame()
        {
            var (isWin, star) = CheckResult();

            Debug.Log("Result battle: " + isWin + " => " + star);

            ShowEndGame(isWin, star);
        }

        private void ResetResourceInGame()
        {
            TotalUseSpellInWave = 0;
            TotalUseEnv = 0;
            listSpellUse.Clear();
            GamePlayData.Instance.ClearResourceInGame();
        }

        public override async void ShowEndGame(bool isWin, int star)
        {
            var mapIdCheck = GameContainer.Instance.Get<ShopDataBase>().Get<StarterPackData>().packConditions[0]
                .mapUnlock;
            var mapIdConvert = MapCampaignInfo.GetCampaignId(0, mapIdCheck, 0);
            if (CampaignId == mapIdConvert)
            {
                UserData.Instance.CampaignData.isCanShowStarterPack = true;
            }

            State = GamePlayState.End;


            if (isWin)
            {
                EventManager.EmitEventData(GamePlayEvent.OnMapComplete, star);

                UIFrame.Instance.SetEventSystem(false);

                if (ModeId == CampaignMode.Normal)
                {
                    try
                    {
                        FirebaseLogic.Instance.SetMaxCampaign(CampaignId);
                        if (UserData.Instance.SettingData.CheckCampaignFirstUnlock(CampaignId))
                            AppsflyerUtils.Instance.CompleteStageEvent(CampaignId);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        throw;
                    }
                }


                await UniTask.Delay(2000, true);

                UIFrame.Instance.SetEventSystem(true);
                //pause
                Speed = 0;

                int previousStar = UserData.Instance.CampaignData.GetStar(CampaignId);

                Reward[] rewards;

                var campaignReward = GameContainer.Instance.Get<MapDataBase>().GetAllReward()
                    .GetRewardByCampaignId(CampaignId);

                if (star <= previousStar)
                {
                    rewards = campaignReward.GetWinReward();
                }
                else
                {
                    rewards = campaignReward.GetStarReward(previousStar, star);
                }

                foreach (var reward in rewards)
                {
                    if (reward.type == ResourceType.Money && reward.id == MoneyType.Exp)
                    {
                        AddExpToHeroes(reward.number);
                    }
                }

                AddStarReward(previousStar, star, false);

                Reward.AddToUserData(rewards, AnalyticsConstants.SourceCampaign);

                if (star > previousStar)
                    UserData.Instance.SetCampaignStar(CampaignId, star);

                UIFrame.Instance.OpenWindow(ScreenIds.game_victory,
                    new GameVictoryWindowProperties(ModeId, WorldId, MapId, star, rewards, infoAddHeroExp));
                //Sfx
                var audioClip1 = ResourceUtils.LoadSound(SoundConstant.VICTORY);
                EazySoundManager.PlaySound(audioClip1);

                //check show rating
                if (RatingController.Instance.CheckCanShowRating(CampaignId))
                {
                    UserData.Instance.AccountData.idMapShowRating = CampaignId;
                    UserData.Instance.Save();
                }
            }
            else
            {
                UserData.Instance.SetCampaignStar(CampaignId, 0);
                Speed = 0;
                UIFrame.Instance.OpenWindow(ScreenIds.game_defeat,
                    new GameDefeatWindowProperties(ModeId, WorldId, MapId));
                //Sfx
                var audioClip1 = ResourceUtils.LoadSound(SoundConstant.DEFEAT);
                EazySoundManager.PlaySound(audioClip1);
            }

            //infoAddHeroExp.Clear();

            CloseALlUiInGame();

            //tracking
            try
            {
                FirebaseLogic.Instance.SetStageEnd(
                    CampaignId, isWin, CallWave.Instance.waveChanged.Value,
                    GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint),
                    star, GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold), TotalUseEnv, GetTotalSpellUse());

                foreach (var spell in listSpellUse)
                {
                    var spellData = (SpellItem)UserData.Instance.GetInventory(InventoryType.Spell, spell.Key);
                    if (spellData != null)
                        FirebaseLogic.Instance.StageSpell(CampaignId, isWin ? 1 : 0, spell.Key, spellData.Level,
                            spell.Value,
                            spellData.HeroIdEquip);
                }

                foreach (var building in buildings)
                {
                    var skill1Lvl = building.GetSkill(BranchType.Skill1) != null
                        ? building.GetSkill(BranchType.Skill1).Level
                        : 0;

                    var skill2Lvl = building.GetSkill(BranchType.Skill2) != null
                        ? building.GetSkill(BranchType.Skill2).Level
                        : 0;

                    FirebaseLogic.Instance.SetStageTower(
                        CampaignId, isWin, building.Id, building.Level.Value, skill1Lvl, skill2Lvl);
                }

                foreach (var heroBase in heroes)
                {
                    FirebaseLogic.Instance.SetStageHero(CampaignId, isWin, heroBase.Id,
                        heroBase.Tracking.skills, heroBase.Tracking.revive, heroBase.Tracking.move);
                }

                FirebaseLogic.Instance.SetCallWave(CampaignId, EarlyWave);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void CloseALlUiInGame()
        {
            GamePlayUIManager.Instance.CloseCurrentUI(true);

            GamePlayUIManager.Instance.CloseAllUI();
        }

        private void AddExpToHeroes(int exp)
        {
            var numberHeroes = heroList.Count;
            //int expForHero = numberHeroes > 0 ? exp / numberHeroes : 0;
            foreach (var heroId in heroList)
            {
                var hero = UserData.Instance.UserHeroData.GetHeroById(heroId);

                if (hero == null) continue;

                var heroDataBase = GameContainer.Instance.GetHeroData(heroId);

                float originExpPercentage = hero.level <= heroDataBase.stats.Length - 1
                    ? hero.exp / heroDataBase.stats[hero.level].maxExp
                    : 1;
                int originLevel = hero.level;

                AddExpToUserHeroData(heroId, exp);

                float currentExpPercentage = hero.level <= heroDataBase.stats.Length - 1
                    ? hero.exp / heroDataBase.stats[hero.level].maxExp
                    : 1;
                int currentLevel = hero.level;

                infoAddHeroExp.Add(heroId, (originExpPercentage, currentExpPercentage, currentLevel - originLevel));
                Debug.Log($"InfoAddExp heroId: {heroId}, originExpPercent: {originExpPercentage}, " +
                          $"currentExpPercentage: {currentExpPercentage}, numberUpLevel: {currentLevel - originLevel}");
            }
        }

        private void AddExpToUserHeroData(int heroId, int exp)
        {
            var currentExp = UserData.Instance.UserHeroData.GetHeroById(heroId).exp;
            UserData.Instance.UserHeroData.GetHeroById(heroId).SetExp(currentExp + exp);
        }

        private void AddStarReward(int previousStar, int star, bool isSave)
        {
            var starReward = new Reward();
            starReward.type = ResourceType.Money;
            switch (ModeId)
            {
                case CampaignMode.Normal:
                    starReward.id = MoneyType.SliverStar;
                    break;
                case CampaignMode.Nightmare:
                    starReward.id = MoneyType.GoldStar;
                    break;
            }

            var numberStarAdd = star - previousStar;

            if (numberStarAdd > 0)
            {
                starReward.number = numberStarAdd;
                starReward.AddToUserData(isSave, AnalyticsConstants.SourceCampaign);
                EventManager.EmitEventData(GamePlayEvent.OnEarnStarCampaign, new int[] {starReward.id, numberStarAdd});
            }
        }

        private (bool, int) CheckResult()
        {
            var lifePoint = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint);

            if (lifePoint <= 0)
            {
                return (false, 0);
            }

            return (true, GamePlayData.Instance.CurrentMapCampaign.GetMapStatBase().GetStar((int)lifePoint));
        }

        public void HideStartWaveBtn()
        {
            startWaveButtonController.HideButtonCallWave();
        }

        public void ShowStartWaveBtn()
        {
            startWaveButtonController.ShowButtonCallWave();
        }

        public Transform GetStartWaveBtnController() => startWaveButtonController.GetFirstBtnCallWave().transform;

        
        public override void AddBuilding(Building building)
        {
            if (buildings.Contains(building) == false)
            {
                buildings.Add(building);
            }
            else
            {
                throw new Exception("building is exist");
            }
        }

        public override void RemoveBuilding(Building building)
        {
            if (buildings.Contains(building))
            {
                buildings.Remove(building);
            }
            else
            {
                throw new Exception("building is not exist");
            }
        }

        public override void AddHero(HeroBase hero)
        {
            if (heroes.Contains(hero) == false)
            {
                heroes.Add(hero);
            }
            else
            {
                throw new Exception("hero is exist");
            }
        }

        public override List<HeroBase> GetHeroes()
        {
            return this.heroes;
        }

        public override HeroBase GetHeroes(int heroId)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].Id == heroId)
                {
                    return heroes[i];
                }
            }

            return null;
        }

        public override void CallEarlyWave()
        {
            EarlyWave += 1;
        }

        public override void CountUseSpell(int spellId)
        {
            EventManager.EmitEvent(GamePlayEvent.OnUseSpell);

            if (!listSpellUse.ContainsKey(spellId))
            {
                listSpellUse.Add(spellId, 1);
            }
            else
            {
                listSpellUse[spellId]++;
            }
        }

        private int GetTotalSpellUse()
        {
            var total = 0;
            foreach (var spell in listSpellUse)
            {
                total += spell.Value;
            }

            return total;
        }
    }
}