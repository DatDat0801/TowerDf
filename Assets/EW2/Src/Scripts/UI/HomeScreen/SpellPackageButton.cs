using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class SpellPackageButton : SalebundleButtonUi
    {
        private void OnEnable()
        {
            title.text = L.shop.spell_pack_name_txt.ToUpper();
        }

        public override void ButtonOnClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_spell_package);
            FirebaseLogic.Instance.ButtonClick("main_menu", "popup_spell_package", 0);
        }
    }
}
