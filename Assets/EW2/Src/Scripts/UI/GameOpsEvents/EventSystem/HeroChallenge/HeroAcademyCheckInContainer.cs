using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroAcademyCheckInContainer : TabContainer
    {
        [SerializeField] private ItemCheckInContainer questContainer;
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
            txtTitle.text = L.game_event.event_daily_reward_title_txt;
            txtSlogan.text = L.game_event.event_daily_slogan_txt;

            gameObject.SetActive(true);

            var listQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroAcademyCheckinEvent>()
                .GetAllQuests();
            if (listQuest != null)
            {
                questContainer.ShowQuest(listQuest);
                questContainer.RefreshTab(RefreshTab);
            }
        }

        private void RefreshTab()
        {
            var listQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<HeroAcademyCheckinEvent>()
                .GetAllQuests();
            questContainer.ShowQuest(listQuest);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }
    }
}