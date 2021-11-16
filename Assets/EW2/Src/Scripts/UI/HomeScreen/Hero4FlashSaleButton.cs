using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class Hero4FlashSaleButton : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.shop.hero_resource_title.ToUpper();
            var data = UserData.Instance.UserEventData.CrystalFlashSaleUserData;
            timeRemain.SetTimeRemain(data.TimeRemain());
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.hero_4_resource_flash_sale);
            FirebaseLogic.Instance.ButtonClick("main_menu", "hero_4_resource_flash_sale", 0);
        }
    }
}