using System;
using TigerForge;

namespace EW2
{
    [Serializable]
    public class UnlockRuneData : UnlockFlowData
    {
        public UnlockRuneData()
        {
            SetContent();
            icon = ResourceUtils.GetSpriteTab("icon_rune");
            EventManager.StartListening(GamePlayEvent.OnChangeLanggueSuccess, SetContent);
        }
        protected override void SetContent()
        {
            content1Step1 = string.Format(L.popup.unlock_rune_txt);
            content2Step1 = string.Format(L.popup.unlock_rune_txt_1);
            content1Step2 = string.Format(L.popup.equip_rune_remind_txt);
            //content2Step2 = string.Format(L.popup.equip_rune_remind_txt);
        }
        protected override void GoToGacha()
        {
            DirectionGoTo.GotoGacha(GachaTabId.Rune);
        }

        protected override void GotoHeroRoom()
        {
            DirectionGoTo.GotoRuneHeroRoom();
            FinishedOrEndedFlow();
        }
    }
}
