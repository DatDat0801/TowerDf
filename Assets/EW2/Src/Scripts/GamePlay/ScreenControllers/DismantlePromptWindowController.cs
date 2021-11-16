using System;
using EW2.Tools;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class DismantlePromptProperties : PopupNoticeWindowProperties
    {
        public RuneItem RuneItem { get; private set; }

        public int TotalExp { get; private set; }

        public DismantlePromptProperties(string title, string content, RuneItem runeItem, int TotalExp, PopupType popupType = PopupType.OneOption, string lbOk = "", UnityAction okCallback = null, string lbNo = "", UnityAction noCallback = null, bool hasSound = true) : base(title, content, popupType, lbOk, okCallback, lbNo, noCallback, hasSound)
        {
            this.TotalExp = TotalExp;
            RuneItem = runeItem;
        }
    }
    public class DismantlePromptWindowController : AWindowController<DismantlePromptProperties>
    {
        [SerializeField] private Text txtTile;

        [SerializeField] private Text txtContent;

        [SerializeField] private Button btnClose;

        [SerializeField] private GameObject panelButton;

        [SerializeField] private Button btnYes;

        [SerializeField] private Button btnNo;
        [SerializeField] private Transform rewardContainer;
        private bool isSwappedButtons;

        protected override void Awake()
        {
            isSwappedButtons = false;

            base.Awake();

            btnClose.onClick.AddListener(() => {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                UIFrame.Instance.CloseCurrentWindow();
            });

            btnYes.onClick.AddListener(OkOnClick);

            btnNo.onClick.AddListener(NoOnClick);
        }

        protected virtual void NoOnClick()
        {
            UIFrame.Instance.CloseCurrentWindow();
            Properties.noOnClick?.Invoke();
        }

        protected virtual void OkOnClick()
        {
            UIFrame.Instance.CloseCurrentWindow();
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
            RepaintReward();
        }

        public void RepaintReward()
        {
            var reward = new Reward()
            {
                id = MoneyType.ExpRune,
                type = ResourceType.Money,
                number = Properties.TotalExp,
                itemType = InventoryType.None
            };
            rewardContainer.DestroyAllChildren();
            var  rewardUi = ResourceUtils.GetRewardUi(reward.type);
            rewardUi.SetData(reward);
            rewardUi.SetParent(rewardContainer);
        }
    }
}
