using EW2.Tools;
using Sirenix.OdinInspector;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class GloryRoadContainer : TabContainer
    {
        [SerializeField] private GloryRoadTierItemUI[] tierItems;
        [SerializeField] private GloryRoadTitleUI titleUi;
        [SerializeField] private ScrollRect scrollRect;

        #region MonobehaviourMethod

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnRepaintGloryRoad, Initialize);

        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnRepaintGloryRoad, Initialize);
        }

        #endregion

        public void Initialize()
        {
            var gloryData = GameContainer.Instance.GetGloryRoadData();
            var gloryUserData = UserData.Instance.UserEventData.GloryRoadUser;
            GloryRoadTierItemUI lastItem = new GloryRoadTierItemUI();
            for (var i = 0; i < tierItems.Length; i++)
            {
                tierItems[i].Repaint(gloryData.items[i], gloryUserData);
                if (gloryData.items[i].rewards.Length == 2)
                {
                    lastItem = tierItems[i];
                }
            }
            lastItem.GetFreeItem().AddListinerForItem(OnLastFreeRewardClaimed);
            if (titleUi != null)
                titleUi.Repaint();
            if (scrollRect != null)
            {
                var child = gloryUserData.GetTierCanClaimReward() - 1;
                if (child > tierItems.Length - 4)
                {
                    child = tierItems.Length - 4;
                }

                if (child < 3)
                {
                    child = 0;
                }
                if (child > 0)
                    scrollRect.SnapTo((RectTransform)scrollRect.content.GetChild(child), 300, true);
            }
        }

        [Button]
        void AddFakeUserPoint(int value)
        {
            UserData.Instance.AddMoney(MoneyType.GloryRoadPoint, value, string.Empty, string.Empty);
            EventManager.EmitEvent(GamePlayEvent.OnRepaintGloryRoad);
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            Initialize();
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void OnLastFreeRewardClaimed()
        {
            string content2 = string.Format(L.game_event.unlock_premium_progress_notice,L.game_event.premium_glory_road_item_txt);
            var property = new UnlockSystemWindowProperties(L.popup.notice_txt,L.game_event.full_basic_progress_notice,content2,null, L.button.btn_unlock_now,
                ()=>{
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
                }, null);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_unlock_system, property);
        }
    }
}
