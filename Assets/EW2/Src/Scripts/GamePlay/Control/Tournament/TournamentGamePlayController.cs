using System;
using System.Collections.Generic;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using ZitgaRankingDefendMode;
using ZitgaTournamentMode;
using AuthProvider = ZitgaSaveLoad.AuthProvider;

namespace EW2
{
    public class TournamentGamePlayController : GamePlayControllerBase
    {
        //public static bool IsTrialCampaign { get; set; }

        //private readonly List<Building> buildings = new List<Building>();

        // heroId - originExpPercentage - currentExpPercentage - numberLevelUp
        private Dictionary<int, (float, float, int)> infoAddHeroExp;
        /// <summary>
        /// towerId, count
        /// </summary>
        private readonly Dictionary<int, int> _towerBuilt = new Dictionary<int, int>();
        private int EarlyWave { get; set; }

        private Dictionary<int, int> listSpellUse = new Dictionary<int, int>();
        private bool _newRecord;

        private void Awake()
        {
            infoAddHeroExp = new Dictionary<int, (float, float, int)>();
            Speed = 1;

            (WorldId, MapId, ModeId) = MapCampaignInfo.GetWorldMapModeId(CampaignId);
            //WorldId = 0;
            //MapId = 0;
            ModeId = 0;
            gameMode = GameMode.TournamentMode;
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
            //The map like the campaign mode, CampaignId = tournament map id
            UIFrame.Instance.OpenWindow(ScreenIds.game_play_tournament,
                new GamePlayWindowProperties(CampaignId, GameMode.TournamentMode));
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

        void UpdateRanking(int currentWave)
        {
            var zitgaTournament = new ZitgaTournament();
            var currentPlatform = Application.platform;
            var userId = UserData.Instance.AccountData.tokenId;

            //point
            var score = this.spawnController.KilledEnemy;
            this._newRecord = UserData.Instance.TournamentData.SetHighScore(score);

            List<HeroUseDefend> heroesInUse = new List<HeroUseDefend>();
            var avatar = UserData.Instance.AccountData.avatarId;
            var playerName = UserData.Instance.AccountData.userName;
            var season = UserData.Instance.TournamentData.GetSeasonId();
            for (var i = 0; i < this.heroes.Count; i++)
            {
                heroesInUse.Add(new HeroUseDefend() { HeroId = heroes[i].Id, HeroLevel = this.heroes[i].Level });
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

            zitgaTournament.UpdateRanking(authProvider, userId,
                new RankingInbound() {
                    Ranking = new RankingTournament() {
                        CreepCleared = score,
                        ListOfHeroes = heroesInUse,
                        Avatar = avatar,
                        AvatarFrame = default,
                        Name = playerName,
                        WaveCleared = currentWave,
                        Season = season
                    }
                });
            zitgaTournament.OnUpdateRanking = ShowEndGame;
        }

        private void OnEndGame()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowEndGame(true);
                //Show notice
                var properties = new PopupNoticeWindowProperties(L.popup.notice_txt, L.popup.result_check_internet);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                return;
            }

            var currentWave = CallWave.Instance.CurrWave;
            UpdateRanking(currentWave);
        }

        private void ResetResourceInGame()
        {
            TotalUseSpellInWave = 0;
            TotalUseEnv = 0;
            listSpellUse.Clear();
            GamePlayData.Instance.ClearResourceInGame();
            this.spawnController.KilledEnemy = 0;
        }

        public async void ShowEndGame(bool isWin)
        {
            State = GamePlayState.End;
            //var currentWave = CallWave.Instance.CurrWave;
            

            UIFrame.Instance.SetEventSystem(false);
            //await UniTask.Delay(3000, true);

            UIFrame.Instance.SetEventSystem(true);
            //pause
            Speed = 0;


            var properties =
                new TournamentResultWindowProperties(MapId, this.infoAddHeroExp, this._newRecord);
            UIFrame.Instance.OpenWindow(ScreenIds.tournament_result, properties);
            //Sfx
            var audioClip1 = ResourceUtils.LoadSound(SoundConstant.VICTORY);
            EazySoundManager.PlaySound(audioClip1);

            CloseALlUiInGame();
            //tracking
            //TODO Tracking if needed
            try
            {
                //FirebaseLogic.Instance.SetCallWave(CampaignId, EarlyWave);
                var currentWave = CallWave.Instance.CurrWave;
                var userData = UserData.Instance.TournamentData;
                var remainingGold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);
                var time = Time.timeSinceLevelLoad * Mathf.Pow(10, 3);
                FirebaseLogic.Instance.EndATournamentGame(this.spawnController.KilledEnemy, currentWave, MapId,
                    userData.currRank, (int)remainingGold, time, this._towerBuilt);
            }
            finally { }
        }

        private void CloseALlUiInGame()
        {
            GamePlayUIManager.Instance.CloseCurrentUI(true);
            
            GamePlayUIManager.Instance.CloseAllUI();
        }

        private bool CheckResult()
        {
            var lifePoint = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.LifePoint);

            if (lifePoint <= 0)
            {
                return false;
            }

            return true;
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
            int towerId = building.Id;
            if (!this._towerBuilt.ContainsKey(towerId))
            {
                this._towerBuilt.Add(towerId, 1);
            }
            else
            {
                this._towerBuilt[towerId]++;
            }
            // if (buildings.Contains(building) == false)
            // {
            //     buildings.Add(building);
            // }
            // else
            // {
            //     throw new Exception("building is exist");
            // }
        }

        public override void RemoveBuilding(Building building)
        {
            // if (buildings.Contains(building))
            // {
            //     buildings.Remove(building);
            // }
            // else
            // {
            //     throw new Exception("building is not exist");
            // }
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
        
        
    }
}