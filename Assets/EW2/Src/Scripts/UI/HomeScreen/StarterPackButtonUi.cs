using System;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class StarterPackButtonUi : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.shop.starter_pack_name.ToUpper();
            var data = UserData.Instance.UserEventData.StarterPackUserData;
            timeRemain.SetTimeRemain(data.TimeRemain());
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popp_starter_pack);
            FirebaseLogic.Instance.ButtonClick("main_menu", "starter_pack", 0);
        }
    }
}