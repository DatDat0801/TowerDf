using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ItemRewardTournamentUi : MonoBehaviour
    {
        [SerializeField] private Image iconMedal;
        [SerializeField] private Transform panelReward;

        private GridReward _gridReward;

        public void InitData(TournamentReward.TournamentRewardData data)
        {
            if (this._gridReward == null)
            {
                this._gridReward = new GridReward(this.panelReward);
            }

            this.iconMedal.sprite = ResourceUtils.GetRankIconSmallTournament(data.rank);

            this._gridReward.SetData(data.rewards);
            
            gameObject.SetActive(true);
        }
    }
}