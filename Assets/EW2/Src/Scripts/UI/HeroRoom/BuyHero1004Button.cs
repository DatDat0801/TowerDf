using System;
using TigerForge;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    public class BuyHero1004Button : IBuyHeroButton
    {
        public int HeroId { get; set; }
        public GameObject EventBtn { get; set; }
        public GameObject BuyBtn { get; set; }
        public void ButtonClick()
        {
            var unlockConditionDataBase = GameContainer.Instance.GetHeroUnlockData();
            var macroUnlockCondition = Array.Find(unlockConditionDataBase.heroConditions, x => x.heroId == HeroId);
            if (UserData.Instance.CampaignData.GetStar(0, macroUnlockCondition.unlockStage) == 0)
            {
                string notice = string.Format(L.popup.spell_unlock_condition,
                    (macroUnlockCondition.unlockStage + 1).ToString());
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, notice);
                //return;
            }
            else if (UserData.Instance.UserEventData.Hero4BundleUserData.CheckCanShow())
            {
                UIFrame.Instance.OpenWindow(ScreenIds.hero_4_bundle);
                //return;
            }
        }
        public void ChangeBehavior()
        {
            this.EventBtn.SetActive(false);
            this.BuyBtn.SetActive(true);
        }

        public void ButtonDisableClick()
        {
            
        }
    }
}