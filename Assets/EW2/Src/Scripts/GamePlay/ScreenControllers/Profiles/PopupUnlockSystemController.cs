using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class UnlockSystemWindowProperties : WindowProperties
    {
        public string title;
        public string content1;
        public string content2;
        public Sprite icon;
        public string lbButtonYes;

        public UnityAction okOnClick;
        public UnityAction noOnClick;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content1"></param>
        /// <param name="content2"></param>
        /// <param name="icon"></param>
        /// <param name="lbOk"></param>
        /// <param name="okCallback"></param>
        public UnlockSystemWindowProperties(string title, string content1, string content2, Sprite icon,
            string lbOk = "", UnityAction okCallback = null, UnityAction noOnClick = null)
        {
            this.noOnClick = noOnClick;
            this.icon = icon;
            this.title = title;
            this.content1 = content1;
            this.content2 = content2;
            this.lbButtonYes = lbOk;
            this.okOnClick = okCallback;
        }
    }

    public class PopupUnlockSystemController : AWindowController<UnlockSystemWindowProperties>
    {
        [SerializeField] private Text txtTile;
        [SerializeField] private Text goToText;
        [SerializeField] private Text txtContent;
        [SerializeField] private Text txtContent2;
        [SerializeField] private Image icon;

        [SerializeField] private Button btnClose;

        [SerializeField] private Button btnYes;


        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(NoOnClick);
            btnYes.onClick.AddListener(OkOnClick);
        }

        protected virtual void NoOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_notice_unlock_system);
            Properties.noOnClick?.Invoke();
        }

        protected virtual void OkOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_notice_unlock_system);
            Properties.okOnClick?.Invoke();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            icon.gameObject.SetActive(true);
            txtContent2.gameObject.SetActive(true);
            txtContent.text = Properties.content1;
            txtContent2.text = Properties.content2;
            icon.sprite = Properties.icon;
            if (this.txtTile)
                txtTile.text = Properties.title;
            goToText.text = Properties.lbButtonYes;
            if (string.IsNullOrEmpty(txtContent2.text))
            {
                txtContent2.gameObject.SetActive(false);
            }

            if (icon.sprite == null)
            {
                icon.gameObject.SetActive(false);
            }
        }
    }
}