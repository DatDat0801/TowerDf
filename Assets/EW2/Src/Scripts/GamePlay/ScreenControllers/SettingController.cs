using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class SettingController : AWindowController
    {
        [SerializeField] private Text textTitle;

        [SerializeField] private Text textTitleGraphic;

        [SerializeField] private Button btnClose;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(CloseClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SetTextLocalize();
        }

        private void SetTextLocalize()
        {
            textTitle.text = L.popup.setting_txt;

            textTitleGraphic.text = L.popup.graphic_txt;
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }
    }
}