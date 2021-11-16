using System;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class PopupNoticeWindowProperties : WindowProperties
    {
        public string title;
        public string content;

        public string lbButtonYes;

        public string lbButtonNo;

        public UnityAction okOnClick;

        public UnityAction noOnClick;
        public bool hasSound;

        public enum PopupType
        {
            NoOption,
            OneOption,
            TwoOption,
            TwoOption_OkPriority,
            TwoOption_CancelPriority
        }

        public PopupType popupType;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">title of popup</param>
        /// <param name="content">content of popup</param>
        /// <param name="popupType">popup type</param>
        /// <param name="lbOk">"ok" Text</param>
        /// <param name="okCallback">"Ok" call back</param>
        /// <param name="lbNo"></param>
        /// <param name="noCallback">"No" call back</param>
        /// <param name="hasSound">has sound ?</param>
        public PopupNoticeWindowProperties(string title, string content, PopupType popupType = PopupType.OneOption,
            string lbOk = "", UnityAction okCallback = null, string lbNo = "",
            UnityAction noCallback = null, bool hasSound = true)
        {
            this.title = title;
            this.content = content;
            this.lbButtonYes = lbOk;
            this.lbButtonNo = lbNo;
            this.okOnClick = okCallback;
            this.noOnClick = noCallback;
            this.popupType = popupType;
            this.hasSound = hasSound;
        }
    }

    public class PopupNoticeWindowController : AWindowController<PopupNoticeWindowProperties>
    {
        [SerializeField] private Text txtTile;

        [SerializeField] private Text txtContent;

        [SerializeField] private Button btnClose;

        [SerializeField] private GameObject panelButton;

        [SerializeField] private Button btnYes;

        [SerializeField] private Button btnNo;
        [SerializeField] private bool isHideCloseButton = false;

        private bool isSwappedButtons;

        protected override void Awake()
        {
            isSwappedButtons = false;

            base.Awake();

            btnClose.onClick.AddListener(() => {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                NoOnClick();
            });

            btnYes.onClick.AddListener(OkOnClick);

            btnNo.onClick.AddListener(NoOnClick);
        }

        protected virtual void NoOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_notice);
            Properties.noOnClick?.Invoke();
        }

        protected virtual void OkOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_notice);
            Properties.okOnClick?.Invoke();
            var audioClip = ResourceUtils.LoadSound(SoundConstant.CONFIRM);
            EazySoundManager.PlaySound(audioClip);
        }

        private void SwapButtons()
        {
            if (!isSwappedButtons)
                btnYes.transform.SetAsLastSibling();
            else
                btnYes.transform.SetAsFirstSibling();

            isSwappedButtons = !isSwappedButtons;
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            txtTile.text = Properties.title; //L.popup.notice_txt;

            txtContent.text = Properties.content;

            if (Properties.lbButtonYes.Length > 0 || Properties.lbButtonNo.Length > 0)
            {
                panelButton.SetActive(true);

                if (Properties.lbButtonYes.Length > 0)
                {
                    btnYes.GetComponentInChildren<Text>().text = Properties.lbButtonYes;

                    btnYes.gameObject.SetActive(true);
                }
                else
                {
                    btnYes.gameObject.SetActive(false);
                }

                if (Properties.lbButtonNo.Length > 0)
                {
                    btnNo.GetComponentInChildren<Text>().text = Properties.lbButtonNo;

                    btnNo.gameObject.SetActive(true);
                }
                else
                {
                    btnNo.gameObject.SetActive(false);
                }
            }
            else
            {
                panelButton.SetActive(false);
            }

            switch (Properties.popupType)
            {
                case PopupNoticeWindowProperties.PopupType.NoOption:
                    btnNo.gameObject.SetActive(false);
                    btnYes.gameObject.SetActive(false);
                    break;
                case PopupNoticeWindowProperties.PopupType.OneOption:
                    if (isSwappedButtons) SwapButtons();
                    btnYes.gameObject.SetActive(true);
                    btnNo.gameObject.SetActive(false);
                    break;
                case PopupNoticeWindowProperties.PopupType.TwoOption:
                    if (isSwappedButtons) SwapButtons();
                    btnYes.gameObject.SetActive(true);
                    btnNo.gameObject.SetActive(true);
                    break;
                case PopupNoticeWindowProperties.PopupType.TwoOption_OkPriority:
                    if (!isSwappedButtons) SwapButtons();
                    btnYes.gameObject.SetActive(true);
                    btnNo.gameObject.SetActive(true);
                    break;
                case PopupNoticeWindowProperties.PopupType.TwoOption_CancelPriority:
                    if (!isSwappedButtons) SwapButtons();
                    btnYes.gameObject.SetActive(true);
                    btnNo.gameObject.SetActive(true);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Set sound for yes/no button
            btnYes.GetComponent<ButtonAnimation>().SetMuteSound(!Properties.hasSound);
            btnNo.GetComponent<ButtonAnimation>().SetMuteSound(!Properties.hasSound);
            if (isHideCloseButton)
            {
                btnClose.gameObject.SetActive(false);
            }
            else
            {
                btnClose.gameObject.SetActive(true);
            }
        }
    }
}
