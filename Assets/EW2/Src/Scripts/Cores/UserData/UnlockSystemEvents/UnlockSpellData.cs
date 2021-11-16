using System;
using TigerForge;

namespace EW2
{
    [Serializable]
    public class UnlockSpellData : UnlockFlowData
    {
        public UnlockSpellData()
        {
            SetContent();
            icon = ResourceUtils.GetSpriteTab("icon_spell");
            EventManager.StartListening(GamePlayEvent.OnChangeLanggueSuccess, SetContent);
        }
        protected sealed override void SetContent()
        {
            content1Step1 = string.Format(L.popup.unlock_spell_txt);
            content2Step1 = string.Format(L.popup.unlock_spell_txt_1);
            content1Step2 = string.Format(L.popup.equip_spell_remind_txt);
        }
        protected override void GoToGacha()
        {
            DirectionGoTo.GotoGacha(GachaTabId.Spell);
        }

        protected override void GotoHeroRoom()
        {
            DirectionGoTo.GotoSpellHeroRoom();
            FinishedOrEndedFlow();
        }
    }
}
