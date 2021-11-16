using System;
using EW2.Spell;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class PopupIntroduceNeetanProperties : WindowProperties
    {
        public string title;
        public string content1;
        public string lbButtonGoToGloryRoad;
        public string lbButtonUnlockNow;

        public UnityAction goToOnClick;
        public UnityAction unlockNowOnClick;


        public PopupIntroduceNeetanProperties(string title, string content1,
            string lbGoTo = "", string lbUnlockNow = "", UnityAction goToCallback = null,
            UnityAction unlockNowOnClick = null)
        {
            this.title = title;
            this.content1 = content1;
            this.lbButtonGoToGloryRoad = lbGoTo;
            this.lbButtonUnlockNow = lbUnlockNow;
            this.goToOnClick = goToCallback;
            this.unlockNowOnClick = unlockNowOnClick;
        }
    }

    public class PopupIntroduceNeetanController : AWindowController<PopupIntroduceNeetanProperties>
    {
        [SerializeField] private Text _txtTile;
        [SerializeField] private Text _goToText;
        [SerializeField] private Text _unlockNowText;
        [SerializeField] private Text _unlockNowDisableText;
        [SerializeField] private Text _txtContent;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnGoTo;
        [SerializeField] private Button _btnUnlockNow;
        [SerializeField] private Button _btnUnlockNowDisable;


        protected override void Awake()
        {
            base.Awake();
            _btnClose.onClick.AddListener(NoOnClick);
            _btnGoTo.onClick.AddListener(OkOnClick);
            _btnUnlockNow.onClick.AddListener(UnlockNowClick);
            _btnUnlockNowDisable.onClick.AddListener(UnlockNowDisableClick);
        }

        private void UnlockNowDisableClick()
        {
            Ultilities.ShowToastNoti(L.game_event.claim_hero_notice);
        }

        protected virtual void UnlockNowClick()
        {
            UIFrame.Instance.CloseWindow(ScreenId);
            Properties.unlockNowOnClick?.Invoke();
        }

        protected virtual void NoOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenId);
        }

        protected virtual void OkOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenId);
            Properties.goToOnClick?.Invoke();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            _txtContent.text = Properties.content1;

            if (this._txtTile)
                _txtTile.text = Properties.title;
            _goToText.text = Properties.lbButtonGoToGloryRoad;
            _unlockNowText.text = Properties.lbButtonUnlockNow;
            _unlockNowDisableText.text = Properties.lbButtonUnlockNow;

            if (UnlockFeatureUtilities.IsCanClaimHero1003Free())
            {
                _btnUnlockNow.gameObject.SetActive(false);
                _btnUnlockNowDisable.gameObject.SetActive(true);
            }
            else
            {
                _btnUnlockNow.gameObject.SetActive(true);
                _btnUnlockNowDisable.gameObject.SetActive(false);
            }
        }
    }
}