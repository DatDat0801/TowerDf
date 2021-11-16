using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class TabDay : TabButton
    {
        [SerializeField] private GameObject imgLock;
        [SerializeField] private GameObject notice;
        [SerializeField] private Image imgTab;
        [SerializeField] private Sprite imgTabOn;
        [SerializeField] private Sprite imgTabOff;

        public override void SetTabActiveChangeImgAndLabel(bool isActive, Color colorActiveText,
            Color colorInactiveText)
        {
            lbButton.text = $"{L.common.day_name} {index + 1}";

            lbButton.color = isActive ? colorActiveText : colorInactiveText;

            imgTab.sprite = isActive ? imgTabOn : imgTabOff;
            
            imgLock.SetActive((index + 1) > UserData.Instance.UserEventData.HeroChallengeUserData.currentDay);
            if (imgLock.activeSelf)
            {
                notice.SetActive(false);
            }
            else
            {
                var countCanReceive = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroChallengeQuestEvent>()
                    .GetCountCanReceiveByDay(index + 1);
                notice.SetActive(countCanReceive > 0);
            }

            base.SetTabActiveChangeImgAndLabel(isActive, colorActiveText, colorInactiveText);
        }
    }
}