using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class QuestWindowController : AWindowController, IUpdateTabBarChanged
    {
        [SerializeField] private Text titleShop;

        [SerializeField] private Button btnClose;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private List<TabContainer> tabContainers;

        public TabsManager TabsManager => tabsManager;

        private int currTabId;
        private bool isInited;

        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(() =>
            {
                UIFrame.Instance.CloseWindow(ScreenIds.quest_scene);
                UIFrame.Instance.OpenWindow(ScreenIds.home);
            });
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            titleShop.text = L.worldmap.name_quest.ToUpper();

            currTabId = 0;

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
    }
}