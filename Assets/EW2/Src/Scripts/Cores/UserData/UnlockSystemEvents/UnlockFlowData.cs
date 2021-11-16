using System;
using TigerForge;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public abstract class UnlockFlowData
    {
        public bool canShow = true;
        public bool isInUnlockFlow =false;
        public bool isFirstTimeGaCha = true;
        protected Sprite icon;
        protected string content1Step1;
        protected string content2Step1;
        protected string content1Step2;
        protected string content2Step2;

        public void StartUnlockFlow()
        {
            this.canShow = false;
            UserData.Instance.Save();
            this.isInUnlockFlow = true;
            EventManager.StartListening(GamePlayEvent.OnApplicationQuit, BreakFlow);
        }
        protected abstract void SetContent();

        public void FinishedOrEndedFlow()
        {
            BreakFlow();
            EventManager.StopListening(GamePlayEvent.OnChangeLanggueSuccess, SetContent);
            EventManager.StopListening(GamePlayEvent.OnApplicationQuit, BreakFlow);
        }

        private void BreakFlow()
        {
            this.isInUnlockFlow = false;
        }
        public void UnlockFlowStep1()
        {
            SetContent();
            var properties = new UnlockSystemWindowProperties(L.popup.notice_txt, content1Step1,content2Step1,icon, L.button.go_to_btn, GoToGacha,FinishedOrEndedFlow);
            StartUnlockFlow();
            this.canShow = false;
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_unlock_system, properties);
        }
        protected abstract void GoToGacha();

        public void UnlockFlowStep2()
        {
            var properties = new UnlockSystemWindowProperties(L.popup.notice_txt, content1Step2,content2Step2,null, L.button.go_to_btn, GotoHeroRoom,FinishedOrEndedFlow);
            this.isFirstTimeGaCha = false;
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_unlock_system, properties);
        }

        protected abstract void GotoHeroRoom();

    }
}
