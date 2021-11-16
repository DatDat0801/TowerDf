using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class RuneFlashSaleButton: SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.shop.rune_flash_sale_title.ToUpper();
            var data = UserData.Instance.UserEventData.RuneFlashSaleUserData;
            timeRemain.SetTimeRemain(data.TimeRemain());
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.rune_flash_sale);
            FirebaseLogic.Instance.ButtonClick("main_menu", "rune_flash_sale", 0);
        }
    }
}