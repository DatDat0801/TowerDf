using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class GloryRoadTitleUI : MonoBehaviour
    {
        [SerializeField] private Text pointText;
        [SerializeField] private Text currentTierText;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button getMoreButton;
        [SerializeField] private TabsManager tabsManager;
        [SerializeField] private Button unlockButton;
        [SerializeField] private GameObject lockIcon;

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnMoneyChange(ResourceType.Money, MoneyType.GloryRoadPoint),
                Repaint);
            infoButton.onClick.AddListener(OnInfoClick);
            getMoreButton.onClick.AddListener(OnGetMoreClick);
            unlockButton.onClick.AddListener(OnUnlockClick);
        }

        private void OnEnable()
        {
            //Repaint();
        }

        void OnInfoClick()
        {
            var properties = new PopupInfoWindowProperties(L.popup.notice_txt, $"{L.game_event.hero_chalenge_rule_txt}\n{L.game_event.glory_road_rule_txt}\n \n{L.game_event.medal_redeem_warning_txt}");
            UIFrame.Instance.OpenWindow(ScreenIds.popup_info, properties);
        }

        private void OnGetMoreClick()
        {
            var properties = new GetMoreProperties(tabsManager);
            UIFrame.Instance.OpenWindow(ScreenIds.glory_road_getmore, properties);
        }

        void OnUnlockClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
        }

        public void Repaint()
        {
            var gloryData = GameContainer.Instance.GetGloryRoadData();
            var gloryUserData = UserData.Instance.UserEventData.GloryRoadUser;
            var unlockedTier = gloryUserData.UnlockedTier();

            var gloryRoadPoint = UserData.Instance.GetMoney(MoneyType.GloryRoadPoint);
            if (unlockedTier >= gloryData.items.Length)
            {
                pointText.text =
                    $"{gloryRoadPoint}/{gloryData.GetGloryDataPoint(unlockedTier).ToString()}";
                currentTierText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                pointText.text =
                    $"{gloryRoadPoint}/{gloryData.GetGloryDataPoint(unlockedTier + 1).ToString()}";
                currentTierText.text = (unlockedTier + 1).ToString();
            }

            if (gloryUserData.IsUnlockPremium())
            {
                unlockButton.gameObject.SetActive(false);
                lockIcon.SetActive(false);
            }
            else
            {
                lockIcon.SetActive(true);
                unlockButton.gameObject.SetActive(true);
            }
        }
    }
}