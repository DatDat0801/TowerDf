using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class PopupNoticeTextWindowController : AWindowController<PopupInfoWindowProperties>
    {
        [SerializeField] private Text txtTile;

        [SerializeField] private Text txtContent;

        [SerializeField] private Text txtTapToClose;

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            txtTile.text = Properties.title;

            txtContent.text = Properties.content;

            txtTapToClose.text = L.popup.tap_to_close;
        }
    }
}