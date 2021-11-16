using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class InfoSeasonDefenseModeController : MonoBehaviour
    {
        [SerializeField] private Text titleTimeSeason;
        [SerializeField] private TimeRemainUi timeCountdown;
        [SerializeField] private Image timeProgress;
        [SerializeField] private Text txtBestScore;
        [SerializeField] private Image rankIcon;

        private int _rank, _bestScore;

        public void SetData(int rank, int bestScore)
        {
            this._rank = rank;
            this._bestScore = bestScore;
            ShowUi();
        }

        private void ShowUi()
        {
            var userData = UserData.Instance.UserHeroDefenseData;
            var dataConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();
            this.titleTimeSeason.text = L.playable_mode.season_end_txt.ToUpper();
            this.timeCountdown.SetTimeRemain(userData.GetTimeRemainSeason(), TimeRemainFormatType.Ddhhmmss,
                HandleEndSeason);
            this.timeProgress.fillAmount = userData.GetTimeRemainSeason() * 1f / dataConfig.seasonDuration * 1f;
            this.txtBestScore.text = string.Format(L.playable_mode.best_record_waves_txt, this._bestScore);
            this.rankIcon.sprite = GetIconMedal();
            this.rankIcon.SetNativeSize();
        }

        private void HandleEndSeason()
        {
            UserData.Instance.UserHeroDefenseData.HandleResetNewSeason();
            UserData.Instance.Save();
            EventManager.EmitEvent(GamePlayEvent.OnNewSeasonHeroDefense);
        }

        private Sprite GetIconMedal()
        {
            var nameMedal = "icon_no_medal";
            if (this._rank == 0)
            {
                nameMedal = "icon_gold_medal";
            }
            else if (this._rank == 1)
            {
                nameMedal = "icon_silver_medal";
            }
            else if (this._rank == 2)
            {
                nameMedal = "icon_wolden_medal";
            }

            return ResourceUtils.GetSpriteAtlas("icon_rank_dfp", nameMedal);
        }
    }
}