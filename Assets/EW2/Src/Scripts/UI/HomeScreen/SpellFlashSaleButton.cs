using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class SpellFlashSaleButton : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.shop.spell_flash_sale_title.ToUpper();
            var data = UserData.Instance.UserEventData.SpellFlashSaleUserData;
            timeRemain.SetTimeRemain(data.TimeRemain());
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.spell_flash_sale);
            FirebaseLogic.Instance.ButtonClick("main_menu", "spell_flash_sale", 0);
        }
    }
}
