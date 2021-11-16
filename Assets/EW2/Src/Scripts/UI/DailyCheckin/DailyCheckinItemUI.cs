using System;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.DailyCheckin
{
    public enum CheckinUIItemState
    {
        Today,
        Taken, //taken item
        CanTake, //can take the reward now
        Alive //can not taken yet
    }

    public class DailyCheckinItemUI : MonoBehaviour
    {
        [SerializeField] private GameObject takenBg;
        [SerializeField] private GameObject canTakeBg;
        [SerializeField] private GameObject normalBg;
        [SerializeField] private GameObject checkbox;
        [SerializeField] private Text dayNumber;
        [SerializeField] private Transform rewardContainer;
        private RewardUI rewardUi;
        public int DayId { get; private set; }

        public Reward Reward { get; private set; }

        public void Repaint(int day, Reward reward, CheckinUIItemState state)
        {
            DayId = day;
            Reward = reward;
            if (dayNumber != null)
            {
                dayNumber.text = string.Format(L.popup.day_count_txt, day) ;
            }
            //reward
            rewardUi = ResourceUtils.GetRewardUi(reward.type);
            rewardUi.SetData(reward);
            rewardUi.SetParent(rewardContainer);
            switch (state)
            {
                case CheckinUIItemState.Today:
                case CheckinUIItemState.CanTake:
                    takenBg.SetActive(false);
                    canTakeBg.SetActive(true);
                    normalBg.SetActive(false);
                    checkbox.SetActive(false);
                    break;
                case CheckinUIItemState.Taken:
                    takenBg.SetActive(true);
                    canTakeBg.SetActive(false);
                    normalBg.SetActive(false);
                    checkbox.SetActive(true);
                    break;

                case CheckinUIItemState.Alive:
                    takenBg.SetActive(false);
                    canTakeBg.SetActive(false);
                    normalBg.SetActive(true);
                    checkbox.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
    }
}