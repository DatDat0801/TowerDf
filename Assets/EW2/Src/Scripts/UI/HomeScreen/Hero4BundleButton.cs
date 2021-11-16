using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class Hero4BundleButton : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.shop.hero_4_bundle.ToUpper();
            var data = UserData.Instance.UserEventData.Hero4BundleUserData;
            timeRemain.SetTimeRemain(data.TimeRemain(), TimeRemainFormatType.Hhmmss, () => { this.gameObject.SetActive(false); });
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.hero_4_bundle);
            FirebaseLogic.Instance.ButtonClick("main_menu", "hero_4_bundle", 0);
        }
    }
}
