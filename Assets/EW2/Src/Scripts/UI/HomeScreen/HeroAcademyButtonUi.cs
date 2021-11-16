using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroAcademyButtonUi : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.game_event.hero_academy_event_name_txt.ToUpper();
            var data = UserData.Instance.UserEventData.HeroAcademyUserData;
            timeRemain.SetTimeRemain(data.TimeRemain(), TimeRemainFormatType.Hhmmss,
                () => { gameObject.SetActive(false); });
        }

        public override void ButtonOnClick()
        {
            var data = new HeroAcademyWindowProperties(HeroAcademyTab.DailyGift);
            UIFrame.Instance.OpenWindow(ScreenIds.hero_academy_event, data);
            FirebaseLogic.Instance.ButtonClick("main_menu", "hero_academy", 0);
        }
    }
}