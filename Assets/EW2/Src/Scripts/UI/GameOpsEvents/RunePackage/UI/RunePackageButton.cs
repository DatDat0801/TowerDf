using UnityEngine;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class RunePackageButton :  SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.popup.rune_pack_name_txt.ToUpper();
            var data = UserData.Instance.UserEventData.RunePackageUserData;
            var timeRemaining = data.TimeRemain();
            //timeRemain.SetTimeRemain(timeRemaining);
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.rune_package);
            FirebaseLogic.Instance.ButtonClick("main_menu", "rune_package", 0);
        }
    }
}