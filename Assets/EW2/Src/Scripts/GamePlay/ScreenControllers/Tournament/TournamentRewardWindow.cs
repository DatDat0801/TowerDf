using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;
using System.Linq;

namespace EW2
{
    public class TournamentRewardWindow : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtRank;
        [SerializeField] private Text txtReward;
        [SerializeField] private GameObject rewardPrefab;
        [SerializeField] private ScrollRect panelReward;
        [SerializeField] private Button btnClose;

        protected override void Awake()
        {
            base.Awake();
            this.btnClose.onClick.AddListener(CloseClick);
            CreateRewards();
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_reward_tournament);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            SetLocalization();
            this.panelReward.content.anchoredPosition = Vector2.zero;
        }

        public override void SetLocalization()
        {
            this.txtTitle.text = L.button.rewards;
            this.txtRank.text = L.playable_mode.rank_txt;
            this.txtReward.text = L.button.rewards;
        }

        private void CreateRewards()
        {
            var database = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentReward>();
            if (database != null)
            {
                var dataRewards = database.tournamentRewardDatas;

                dataRewards = dataRewards.OrderByDescending(data => data.rank).ToArray();
                
                foreach (var reward in dataRewards)
                {
                    var go = Instantiate(this.rewardPrefab, this.panelReward.content.transform);
                    if (go != null)
                    {
                        var control = go.GetComponent<ItemRewardTournamentUi>();
                        if (control != null)
                        {
                            control.InitData(reward);
                        }
                    }
                }
            }
        }
    }
}