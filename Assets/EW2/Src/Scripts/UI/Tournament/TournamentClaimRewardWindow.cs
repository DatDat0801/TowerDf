using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    public class TournamentClaimRewardWindow : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtNotice;
        [SerializeField] private Text txtReward;
        [SerializeField] private Text txtTapToClose;
        [SerializeField] private Image iconRank;
        [SerializeField] private Transform panelReward;
        private int _rankId;
        private GridReward _gridReward;

        protected override void Awake()
        {
            base.Awake();

            OutTransitionFinished += controller => {
                UIFrame.Instance.OpenWindow(ScreenIds.popup_reward_tournament);
            };
        }

        public override void SetLocalization()
        {
            this.txtTitle.text = L.popup.congratulation_txt;
            var nameRank = Localization.Current.Get("playable_mode", $"tournament_rank_{this._rankId}");
            this.txtNotice.text = string.Format(L.playable_mode.congratulation_notice_txt, nameRank);
            this.txtReward.text = L.button.rewards.ToUpper();
            this.txtTapToClose.text = L.popup.tap_to_close.ToUpper();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            if (_gridReward == null)
            {
                _gridReward = new GridReward(this.panelReward);
            }

            this.iconRank.sprite = ResourceUtils.GetRankIconSmallTournament(this._rankId);

            SetLocalization();

            ParseReward();
        }

        private void ParseReward()
        {
            this._gridReward.ReturnPool();
            var database = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentReward>();
            if (database != null)
            {
                var rewards = database.GetRewardData(this._rankId).rewards;
                Reward.AddToUserData(rewards);
                this._gridReward.SetData(rewards);
                UpdateRewardStatus();
            }
        }

        private void UpdateRewardStatus()
        {
            UserData.Instance.TournamentData.isClaimedReward = true;
            EventManager.EmitEvent(GamePlayEvent.OnClaimRewardTournament);
        }
    }
}