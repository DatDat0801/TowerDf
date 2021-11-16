using UnityEngine;

namespace EW2
{
    public class EventTabUI : TabButton
    {
        [SerializeField] protected TimeRemainUi timeRemainUi;

        public override void SetTabActiveChangeImg(bool isActive)
        {
            if (timeRemainUi)
            {
                timeRemainUi.SetTimeRemain(UserData.Instance.UserEventData.HeroAcademyUserData.TimeRemain());
            }

            button.image.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("game_event_system", $"icon_event_tab_on")
                : ResourceUtils.GetSpriteAtlas("game_event_system", $"icon_event_tab_off");
            button.image.SetNativeSize();

            if (index == 0)
            {
                lbButton.text = L.game_event.event_daily_reward_title_txt;
            }
            else if (index == 1)
            {
                lbButton.text = L.game_event.hero_chalenge_txt;
            }
            else if (index == 2)
            {
                lbButton.text = L.game_event.glory_road_title_txt;
            }

            base.SetTabActiveChangeImg(isActive);
        }
    }
}