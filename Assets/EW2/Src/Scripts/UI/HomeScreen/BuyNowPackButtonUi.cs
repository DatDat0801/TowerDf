using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class BuyNowPackButtonUi : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.popup.buy_now_title_txt.ToUpper();
            var data = UserData.Instance.UserEventData.BuyNowUserData;
            timeRemain.SetTimeRemain(data.TimeRemain(), TimeRemainFormatType.Hhmmss,
                () => { gameObject.SetActive(false); });
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
            FirebaseLogic.Instance.ButtonClick("main_menu", "buy_now", 0);
        }
    }
}