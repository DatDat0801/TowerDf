using System;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroChallengeContainer : TabContainer, IUpdateTabBarChanged
    {
        [SerializeField] private TabsManager tabsManager;
        [SerializeField] private QuestContainer questContainer;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtSlogan;
        [SerializeField] private Button btnInfo;

        private int currTab;

        private void Awake()
        {
            btnInfo.onClick.AddListener(OnInfoClick);
        }

        void OnInfoClick()
        {
            var properties = new PopupInfoWindowProperties(L.popup.notice_txt,
                $"{L.game_event.hero_chalenge_rule_txt}\n{L.game_event.glory_road_rule_txt}\n \n{L.game_event.medal_redeem_warning_txt}");
            UIFrame.Instance.OpenWindow(ScreenIds.popup_info, properties);
        }

        public override void ShowContainer()
        {
            txtTitle.text = L.game_event.hero_chalenge_txt;
            txtSlogan.text = L.game_event.hero_chalenge_slogan_txt;

            gameObject.SetActive(true);
            InitData();

            tabsManager.InitTabManager(this, currTab);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        public void OnTabBarChanged(int indexActive)
        {
            if (CheckDayUnlocked(indexActive))
            {
                currTab = indexActive;
                var listQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroChallengeQuestEvent>()
                    .GetQuestsByDay(currTab + 1);

                questContainer.ShowQuest(listQuest);
                questContainer.RefreshTab(RefreshTabContent);
            }
            else
            {
                var notice = L.game_event.quest_not_unlocked_txt;
                Ultilities.ShowToastNoti(notice);
                tabsManager.SetSelected(currTab);
            }
        }

        private void RefreshTabContent()
        {
            tabsManager.SetSelected(currTab);
        }

        private void InitData()
        {
            currTab = UserData.Instance.UserEventData.HeroChallengeUserData.currentDay - 1;
        }

        private bool CheckDayUnlocked(int indexTab)
        {
            var day = indexTab + 1;
            return day <= UserData.Instance.UserEventData.HeroChallengeUserData.currentDay;
        }
    }
}