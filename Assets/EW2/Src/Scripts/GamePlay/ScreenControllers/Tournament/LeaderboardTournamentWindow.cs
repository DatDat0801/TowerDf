using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;
using ZitgaRankingDefendMode;
using ZitgaTournamentMode;
using Random = UnityEngine.Random;

namespace EW2
{
    public enum LeaderboardArenaTabId
    {
        Season = 0,
        TopPlayer = 1
    }

    [Serializable]
    public class LeaderboardArenaWindowProperties : WindowProperties
    {
        public List<RankingTournament> listSeason;
        public List<RankingTournament> listTopPlayer;
        public RankingTournament myRankSeason;
        public RankingTournament myRankTopPlayer;

        public LeaderboardArenaWindowProperties(List<RankingTournament> seasons, List<RankingTournament> topPlayers,
            RankingTournament myRankSeason, RankingTournament myRankTopPlayer)
        {
            this.listSeason = seasons;
            this.listTopPlayer = topPlayers;
            this.myRankSeason = myRankSeason;
            this.myRankTopPlayer = myRankTopPlayer;
        }
    }

    public class LeaderboardTournamentWindow : AWindowController<LeaderboardArenaWindowProperties>, IUpdateTabBarChanged
    {
        [SerializeField] private Text titleLeaderboard;

        [SerializeField] private Text txtRank;

        [SerializeField] private Text txtPlayer;

        [SerializeField] private Text txtHero;

        [SerializeField] private Text txtPoint;

        [SerializeField] private Text txtWave;

        [SerializeField] private Button buttonClose;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private EnahanceLeaderboardTournament _leaderboardTournament;

        [SerializeField] private ItemRankingTournament _myRank;

        private int _currTab;

        protected override void Awake()
        {
            base.Awake();
            this.buttonClose.onClick.AddListener(CloseClick);
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_leaderboard_top_player);
        }

        public void SetLocalizationl()
        {
            this.txtRank.text = L.playable_mode.rank_txt;

            this.txtPlayer.text = L.playable_mode.player_txt;

            this.txtHero.text = L.playable_mode.formation_txt;

            this.txtPoint.text = L.gameplay.score;

            this.txtWave.text = L.playable_mode.waves_cleared_record_txt;
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            // FakeData();

            SetLocalizationl();

            if (!tabsManager.IsInited)
            {
                tabsManager.InitTabManager(this, (int)LeaderboardArenaTabId.Season);
            }
            else
            {
                tabsManager.SetSelected((int)LeaderboardArenaTabId.Season);
            }
        }


        public void OnTabBarChanged(int indexActive)
        {
            this._currTab = indexActive;

            if (indexActive == (int)LeaderboardArenaTabId.Season)
            {
                this.titleLeaderboard.text = L.playable_mode.season_rank_txt.ToUpper();

                if (Properties.listSeason != null)
                    _leaderboardTournament.SetData(Properties.listSeason, LeaderboardArenaTabId.Season);
                if (Properties.myRankSeason != null)
                {
                    this._myRank.SetData(0, Properties.myRankSeason, MyRankClick, LeaderboardArenaTabId.Season);
                }
                else
                {
                    Properties.myRankSeason = RankingTournament.CreateRankingEmpty();
                    this._myRank.SetData(0, Properties.myRankSeason, MyRankClick, LeaderboardArenaTabId.Season);
                }
            }
            else if (indexActive == (int)LeaderboardArenaTabId.TopPlayer)
            {
                this.titleLeaderboard.text = L.playable_mode.hall_of_fame_txt.ToUpper();

                if (Properties.listTopPlayer != null)
                    _leaderboardTournament.SetData(Properties.listTopPlayer, LeaderboardArenaTabId.TopPlayer);
                if (Properties.myRankTopPlayer != null)
                {
                    this._myRank.SetData(0, Properties.myRankTopPlayer, MyRankClick, LeaderboardArenaTabId.TopPlayer);
                }
                else
                {
                    Properties.myRankTopPlayer = RankingTournament.CreateRankingEmpty();
                    this._myRank.SetData(0, Properties.myRankTopPlayer, MyRankClick, LeaderboardArenaTabId.TopPlayer);
                }
            }
        }

        private void FakeData()
        {
            Properties.listSeason = new List<RankingTournament>();
            Properties.listTopPlayer = new List<RankingTournament>();
            Properties.myRankSeason = new RankingTournament();

            for (int i = 0; i < 100; i++)
            {
                var rank = new RankingTournament();
                rank.Avatar = 0;
                rank.Name = $"Player_{i}";
                rank.RankSeason = i + 1;
                rank.CreepCleared = 1000 - i;
                rank.WaveCleared = 10;
                rank.ListOfHeroes = new List<HeroUseDefend>();
                for (int j = 0; j < 3; j++)
                {
                    var heroSelected = new HeroUseDefend() {HeroId = 1001 + j, HeroLevel = Random.Range(1, 25)};
                    rank.ListOfHeroes.Add(heroSelected);
                }

                Properties.listSeason.Add(rank);
            }

            for (int i = 100; i < 200; i++)
            {
                var rank = new RankingTournament();
                rank.Avatar = 0;
                rank.Name = $"Player_{i}";
                rank.RankSeason = i + 1;
                rank.CreepCleared = 1000 - i;
                rank.WaveCleared = 10;
                rank.ListOfHeroes = new List<HeroUseDefend>();
                for (int j = 0; j < 3; j++)
                {
                    var heroSelected = new HeroUseDefend() {HeroId = 1001 + j, HeroLevel = Random.Range(1, 25)};
                    rank.ListOfHeroes.Add(heroSelected);
                }

                Properties.listTopPlayer.Add(rank);
            }
        }

        private void MyRankClick(int dataIndex)
        {
            if (this._currTab == (int)LeaderboardArenaTabId.Season)
            {
                if (Properties.myRankSeason.IsShowMore)
                {
                    Properties.myRankSeason.IsShowMore = false;
                    this._myRank.SetData(0, Properties.myRankSeason, MyRankClick, LeaderboardArenaTabId.Season);
                }
                else
                {
                    Properties.myRankSeason.IsShowMore = true;
                    this._myRank.SetData(0, Properties.myRankSeason, MyRankClick, LeaderboardArenaTabId.Season);
                }
            }
            else
            {
                if (Properties.myRankTopPlayer.IsShowMore)
                {
                    Properties.myRankTopPlayer.IsShowMore = false;
                    this._myRank.SetData(0, Properties.myRankTopPlayer, MyRankClick, LeaderboardArenaTabId.TopPlayer);
                }
                else
                {
                    Properties.myRankTopPlayer.IsShowMore = true;
                    this._myRank.SetData(0, Properties.myRankTopPlayer, MyRankClick, LeaderboardArenaTabId.TopPlayer);
                }
            }
        }
    }
}