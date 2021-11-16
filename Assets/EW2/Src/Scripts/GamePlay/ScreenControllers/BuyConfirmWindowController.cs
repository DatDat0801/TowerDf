using System;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class BuyConfirmWindowProperties : WindowProperties
    {
        public string title;
        public string content;
        public string currencyQuantity;
        public string lbButtonYes;

        public string lbButtonNo;

        public UnityAction okOnClick;

        public UnityAction noOnClick;
        public bool hasSound;
        public BuyConfirmWindowProperties(string title, string content, string currencyQuantity,
            string lbOk = "", UnityAction okCallback = null, string lbNo = "", UnityAction noCallback = null, bool hasSound = true)
        {
            this.title = title;
            this.content = content;
            this.lbButtonYes = lbOk;
            this.lbButtonNo = lbNo;
            this.okOnClick = okCallback;
            this.noOnClick = noCallback;
            this.currencyQuantity = currencyQuantity;
            this.hasSound = hasSound;
        }
    }

    public class BuyConfirmWindowController : AWindowController<BuyConfirmWindowProperties>
    {
        [SerializeField] private Text txtTile;

        [SerializeField] private Text txtContent;

        [SerializeField] private Button btnClose;
        

        [SerializeField] private Button btnYes;

        [SerializeField] private Button btnNo;

        [SerializeField] private Text quantity;

        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(CloseClick);

            btnYes.onClick.AddListener(OkOnClick);

            btnNo.onClick.AddListener(NoOnClick);
        }

        void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }
        private void NoOnClick()
        {
            Properties.noOnClick?.Invoke();

            UIFrame.Instance.CloseCurrentWindow();
        }

        private void OkOnClick()
        {
            Properties.okOnClick?.Invoke();

            UIFrame.Instance.CloseCurrentWindow();
            var audioClip = ResourceUtils.LoadSound(SoundConstant.CONFIRM);
            EazySoundManager.PlaySound(audioClip);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            txtTile.text = Properties.title;
            txtContent.text = Properties.content;
            quantity.text = Properties.currencyQuantity;
            
            btnYes.GetComponentInChildren<Text>().text = Properties.lbButtonYes;
            btnNo.GetComponentInChildren<Text>().text = Properties.lbButtonNo;
            
            //Set sound for yes/no button
            btnYes.GetComponent<ButtonAnimation>().SetMuteSound(!Properties.hasSound);
            btnNo.GetComponent<ButtonAnimation>().SetMuteSound(!Properties.hasSound);
        }
    }
}