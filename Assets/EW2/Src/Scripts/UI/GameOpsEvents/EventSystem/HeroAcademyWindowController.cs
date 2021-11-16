using System;
using System.Collections.Generic;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class HeroAcademyWindowProperties : WindowProperties
    {
        public HeroAcademyTab tabId;

        public HeroAcademyWindowProperties(HeroAcademyTab tabId = HeroAcademyTab.DailyGift)
        {
            this.tabId = tabId;
        }
    }

    public enum HeroAcademyTab
    {
        DailyGift = 0,
        HeroChallenge = 1,
        GloryRoad = 2
    }

    public class HeroAcademyWindowController : AWindowController<HeroAcademyWindowProperties>, IUpdateTabBarChanged
    {
        [SerializeField] private List<TabContainer> tabContainers;

        [SerializeField] private TabsManager tabsManager;
        [SerializeField] private Button closeButton;
        [SerializeField] private Text txtTitle;


        private HeroAcademyTab currentTabId;


        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            txtTitle.text = L.game_event.hero_academy_event_name_txt;
            currentTabId = Properties.tabId;
            if (!tabsManager.IsInited)
            {
                tabsManager.InitTabManager(this, (int) currentTabId);
            }
            else
            {
                tabsManager.SetSelected((int) currentTabId);
            }
        }

        public void OnTabBarChanged(int indexActive)
        {
            currentTabId = (HeroAcademyTab) indexActive;

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
    }
}