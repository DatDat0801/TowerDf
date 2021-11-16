using UnityEngine;
using UnityEngine.UI;
using ZitgaRankingDefendMode;

namespace EW2
{
    public class LeaderboardContainer : TabContainer
    {
        [SerializeField] private Text txtRank;
        [SerializeField] private Text txtPlayer;
        [SerializeField] private Text txtFormation;
        [SerializeField] private Text txtWave;
        [SerializeField] private ItemRankingHeroDefense currentRanking;
        [SerializeField] private EnahanceLeaderboardDefenseMode leaderboard;

        private RankingDefenseOutbound _currDataRanking;

        public void SetData(RankingDefenseOutbound dataRanking)
        {
            this._currDataRanking = dataRanking;
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            ShowUi();
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void ShowUi()
        {
            this.txtRank.text = L.playable_mode.rank_txt;
            this.txtPlayer.text = L.playable_mode.player_txt;
            this.txtFormation.text = L.playable_mode.formation_txt;
            this.txtWave.text = L.gameplay.ingame_wave;
            this.currentRanking.SetData(this._currDataRanking.RankingData);
            this.leaderboard.SetData(this._currDataRanking.ListRankingData);
        }
    }
}