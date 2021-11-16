using UnityEngine;

namespace EW2
{
    public class NewHeroTabUi : TabButton
    {
        [SerializeField] protected TimeRemainUi timeRemainUi;
        public override void SetTabActiveChangeImg(bool isActive)
        {
            if (timeRemainUi)
            {
                timeRemainUi.SetTimeRemain(UserData.Instance.UserEventData.NewHeroEventUserData.TimeRemain());
            }

            button.image.sprite = isActive
                ? ResourceUtils.GetSpriteAtlas("game_event_system", $"icon_event_tab_on")
                : ResourceUtils.GetSpriteAtlas("game_event_system", $"icon_event_tab_off");
            button.image.SetNativeSize();

            if (index == (int)NewHeroTab.Mission)
            {
                lbButton.text = L.game_event.nhero_mission_txt;
            }
            else if (index == (int)NewHeroTab.HeroBundle)
            {
                lbButton.text = L.game_event.nhero_bundle_txt;
            }
            else if (index == (int)NewHeroTab.HerosBackpack)
            {
                lbButton.text = L.game_event.hero_backpack_txt;
            }

            base.SetTabActiveChangeImg(isActive);
        }
    }
}