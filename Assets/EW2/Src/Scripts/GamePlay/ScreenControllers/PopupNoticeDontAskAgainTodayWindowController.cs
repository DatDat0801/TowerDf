using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class DontAskToday
    {
        public string feature;
        public DateTime lastAskTime;
    }
    public class PopupNoticeDontAskAgainTodayWindowController : PopupNoticeWindowController
    {
        public const string DISMANTLE_DONT_ASK_TODAY = "DontAskTodayDismantle";
        [SerializeField] private Toggle checkbox;
        
        protected override void NoOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_notice_dont_ask_again_today);
            Properties.noOnClick?.Invoke();
            DontAskAgainToday();
        }

        protected override void OkOnClick()
        {
            base.OkOnClick();
            DontAskAgainToday();
        }

        private void DontAskAgainToday()
        {
            if (checkbox.isOn)
            {
                var ask = new DontAskToday(){feature = DISMANTLE_DONT_ASK_TODAY, lastAskTime = TimeManager.NowUtc};
                var json = JsonConvert.SerializeObject(ask);
                PlayerPrefs.SetString(DISMANTLE_DONT_ASK_TODAY, json);
            }
        }
    }
}