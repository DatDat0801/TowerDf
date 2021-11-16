using System;
using System.Collections.Generic;
using SocialTD2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class ShopWindowProperties : WindowProperties
    {
        public ShopTabId tabId;

        public ShopWindowProperties(ShopTabId shopTabId = ShopTabId.None)
        {
            this.tabId = shopTabId;
        }
    }

    public class ShopWindowController : AWindowController<ShopWindowProperties>, IUpdateTabBarChanged
    {
        [SerializeField] private Text titleShop;

        [SerializeField] private Button btnClose;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private List<TabContainer> tabContainers;

        private int currTabId;

        private bool isInited;

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            titleShop.text = L.worldmap.name_shop.ToUpper();

            if (Properties.tabId != ShopTabId.None)
                currTabId = (int)Properties.tabId;

            if (!isInited)
            {
                tabsManager.InitTabManager(this, currTabId);
                isInited = true;
            }
            else
            {
                tabsManager.SetSelected(currTabId);
            }
        }
        public void OnTabBarChanged(int indexActive)
        {
            if (indexActive == (int)ShopTabId.Tournament)
            {
                var dataConfig = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentConfigData>().GetTournamentConfig();
                if (!UserData.Instance.CampaignData.IsUnlockedStage(0, dataConfig.mapUnlock))
                {
                    var str = string.Format(L.popup.spell_unlock_condition, (dataConfig.mapUnlock + 1).ToString());
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, str);
                    tabsManager.SetSelected(currTabId);
                    return;
                }

                if (!LoadSaveUtilities.IsAuthenticated())
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.require_login_first);
                    tabsManager.SetSelected(currTabId);
                    return;
                }
            }
            currTabId = indexActive;
            for (int i = 0; i < tabContainers.Count; i++)
            {
                if (i == indexActive)
                {
                    tabContainers[i].ShowContainer();
                }
                else
                {
                    tabContainers[i].HideContainer();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(() => {
                UIFrame.Instance.CloseWindow(ScreenIds.shop_scene);
                UIFrame.Instance.OpenWindow(ScreenIds.home);
            });

            PlayerPrefs.SetInt("number_tab_shop", GameConfig.NumberTabShop);
            BadgeUI.numberTabShop = GameConfig.NumberTabShop;
            EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
            EventManager.StartListening(GamePlayEvent.OnFocusTabShop, HandleFocusTabShop);
        }

        private void HandleFocusTabShop()
        {
            currTabId = EventManager.GetInt(GamePlayEvent.OnFocusTabShop);

            if (!isInited)
            {
                tabsManager.InitTabManager(this, currTabId);
                isInited = true;
            }
            else
            {
                tabsManager.SetSelected(currTabId);
            }
        }
    }
}
