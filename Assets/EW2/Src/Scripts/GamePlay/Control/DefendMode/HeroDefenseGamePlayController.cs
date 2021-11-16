using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using ZitgaRankingDefendMode;

namespace EW2
{
    public class HeroDefenseGamePlayController : GamePlayControllerBase
    {
        // heroId - originExpPercentage - currentExpPercentage - numberLevelUp
        private Dictionary<int, (float, float, int)> infoAddHeroExp;


        private int EarlyWave { get; set; }

        private Dictionary<int, int> listSpellUse = new Dictionary<int, int>();

        private void Awake()
        {
            infoAddHeroExp = new Dictionary<int, (float, float, int)>();
            Speed = 1;
            gameMode = GameMode.DefenseMode;
        }

        private void Start()
        {
            UIFrame.Instance.SwitchCamera(true);
            SpawnGamePlayUi();
            //EventManager.StartListening(GamePlayEvent.OnEndGame, OnEndGame);
            EventManager.StartListening(GamePlayEvent.OnHealthPointDFPUpdate, OnDFSPHealthChange);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //EventManager.StopListening(GamePlayEvent.OnEndGame, OnEndGame);
            EventManager.StopListening(GamePlayEvent.OnHealthPointDFPUpdate, OnDFSPHealthChange);
            ResetResourceInGame();
        }

        public override void SpawnGamePlayUi()
        {
            if (gameMode == GameMode.DefenseMode)
            {
                UIFrame.Instance.OpenWindow(ScreenIds.game_play_hero_defense,
                    new GamePlayWindowProperties(CampaignId, gameMode));
            }
        }

        void OnDFSPHealthChange()
        {
            var hp = EventManager.GetData<int>(GamePlayEvent.OnHealthPointDFPUpdate);
            if (hp <= 0)
            {
                OnEndGame();
            }
        }

        private void OnEndGame()
        {
            var currentWave = CallWave.Instance.CurrWave;
            UpdateRanking(currentWave);
        }

        private void ResetResourceInGame()
        {
            TotalUseSpellInWave = 0;
            TotalUseEnv = 0;
            listSpellUse.Clear();
            GamePlayData.Instance.ClearResourceInGame();
        }

        void UpdateRanking(int currentWave)
        {
            var zitgaRankingDefense = new ZitgaRankingDefense();
            var currentPlatform = Application.platform;
            var userId = UserData.Instance.AccountData.tokenId;
            var dfpId = UserData.Instance.UserHeroDefenseData.defensePointId;
            List<HeroUseDefend> heroesInUse = new List<HeroUseDefend>();
            var avatar = UserData.Instance.AccountData.avatarId;
            var playerName = UserData.Instance.AccountData.userName;
            //var avatarFrame = UserData.Instance.
            for (var i = 0; i < this.heroes.Count; i++)
            {
                heroesInUse.Add(new HeroUseDefend() {HeroId = heroes[i].Id, HeroLevel = this.heroes[i].Level});
            }

            AuthProvider authProvider;
            switch (currentPlatform)
            {
                case RuntimePlatform.Android:
                    authProvider = AuthProvider.ANDROID_DEVICE;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    authProvider = AuthProvider.IOS_DEVICE;
                    break;
                default:
                    authProvider = AuthProvider.WINDOWS_DEVICE;
                    break;
            }

            zitgaRankingDefense.UpdateRanking(authProvider, userId,
                new RankingDefenseInbound() {
                    RankingData = new RankingDefense() {
                        WaveCleared = currentWave,
                        DefensivePoint = dfpId,
                        ListOfHeroes = heroesInUse,
                        Avatar = avatar,
                        Name = playerName,
                        AvatarFrame = default
                    }
                });
            zitgaRankingDefense.OnUpdateRanking = ShowEndGame;
        }

        public async void ShowEndGame(bool updateSuccess)
        {
            State = GamePlayState.End;
            var currentWave = CallWave.Instance.CurrWave;
            UserData.Instance.UserHeroDefenseData.SetHighestScore(currentWave);

            UIFrame.Instance.SetEventSystem(false);
            await UniTask.Delay(1000, true);

            UIFrame.Instance.SetEventSystem(true);
            //pause
            Speed = 0;

            var dataBase = GameContainer.Instance.Get<DefendModeDataBase>().Get<DefendModeReward>();

            var rewards = dataBase.GetRewards(currentWave);
            Reward.AddToUserData(rewards, "defend_mode");
            var properties =
                new HeroDefenseResultWindowProperties(MapId, ModeId, currentWave, rewards, this.infoAddHeroExp);
            UIFrame.Instance.OpenWindow(ScreenIds.hero_defense_result, properties);

            //Sfx
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.VICTORY);
            EazySoundManager.PlaySound(audioClip1);

            //check show rating
            if (RatingController.Instance.CheckCanShowRating(CampaignId))
            {
                UserData.Instance.AccountData.idMapShowRating = CampaignId;
                UserData.Instance.Save();
            }

            CloseALlUiInGame();

            //tracking
#if TRACKING_FIREBASE
            try
            {
                FirebaseLogic.Instance.DefenseModeEnd(UserData.Instance.UserHeroDefenseData.battleId, currentWave,
                    GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold));
            }
            catch (Exception e)
            {
                throw;
            }
#endif
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


        public void HideStartWaveBtn()
        {
            startWaveButtonController.HideButtonCallWave();
        }

        public void ShowStartWaveBtn()
        {
            startWaveButtonController.ShowButtonCallWave();
        }

        public Transform GetStartWaveBtnController() => startWaveButtonController.GetFirstBtnCallWave().transform;

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