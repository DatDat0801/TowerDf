using System;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class PopupInfoWindowProperties : WindowProperties
    {
        public string title;
        public string content;

        public PopupInfoWindowProperties(string title, string content)
        {
            this.title = title;
            this.content = content;
        }
    }

    public class PopupInfoWindowController : AWindowController<PopupInfoWindowProperties>
    {
        [SerializeField] private Text txtTile;

        [SerializeField] private Text txtContent;

        [SerializeField] private Text txtTapToClose;

        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private Button closeClick;

        protected override void Awake()
        {
            base.Awake();
            this.closeClick.onClick.AddListener(CloseClick);
        }
        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_info_big);
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            if (this.txtTile)
                txtTile.text = Properties.title;
            if (this.txtContent)
                txtContent.text = Properties.content;
            if (this.txtTapToClose)
                txtTapToClose.text = L.popup.tap_to_close;
            
            scrollRect.content.anchoredPosition = Vector2.zero;
        }
    }
}