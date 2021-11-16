using System;
using System.Collections.Generic;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class NewHeroWindowProperties : WindowProperties
    {
        public NewHeroTab tabId;

        public NewHeroWindowProperties(NewHeroTab tabId = NewHeroTab.HeroBundle)
        {
            this.tabId = tabId;
        }
    }

    public enum NewHeroTab
    {
        HeroBundle = 0,
        HerosBackpack = 1,
        Mission = 2
    }

    public class NewHeroWindowController : AWindowController<NewHeroWindowProperties>, IUpdateTabBarChanged
    {
        [SerializeField] private List<TabContainer> tabContainers;

        [SerializeField] private TabsManager tabsManager;
        [SerializeField] private Button closeButton;
        [SerializeField] private Text txtTitle;


        private NewHeroTab _currentTabId;


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
            txtTitle.text = L.game_event.nhero_event_name_txt.ToUpper();
            this._currentTabId = Properties.tabId;
            if (!tabsManager.IsInited)
            {
                tabsManager.InitTabManager(this, (int)this._currentTabId);
            }
            else
            {
                tabsManager.SetSelected((int)this._currentTabId);
            }
        }

        public void OnTabBarChanged(int indexActive)
        {
            this._currentTabId = (NewHeroTab)indexActive;

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