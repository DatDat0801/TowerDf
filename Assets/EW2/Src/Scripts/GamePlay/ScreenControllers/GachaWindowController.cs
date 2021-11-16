using System;
using System.Collections.Generic;
using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class GachaWindowProperties : WindowProperties
    {
        public GachaTabId tabId;

        public GachaWindowProperties(GachaTabId tabId = GachaTabId.None)
        {
            this.tabId = tabId;
        }
    }

    public class GachaWindowController : AWindowController<GachaWindowProperties>, IUpdateTabBarChanged
    {
        [SerializeField] private Text titleGacha;

        [SerializeField] private Button btnClose;

        [SerializeField] private TabsManager tabsManager;

        [SerializeField] private List<TabContainer> tabContainers;

        [SerializeField] private List<GameCurrencyData> groupKeySpell;

        [SerializeField] private List<GameCurrencyData> groupKeyRune;

        private int currTabId;

        private bool isInited;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(() =>
            {
                UIFrame.Instance.CloseCurrentWindow();
                UIFrame.Instance.OpenWindow(ScreenIds.home);
            });
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            titleGacha.text = L.worldmap.name_gacha.ToUpper();

            if (Properties.tabId != GachaTabId.None)
                currTabId = (int) Properties.tabId;

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
            if (indexActive == (int) GachaTabId.Spell)
            {
                if (!UnlockFeatureUtilities.IsSpellAvailable())
                {
                    var titleNoti = String.Format(L.popup.spell_unlock_condition,
                        UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID + 1);
                    Ultilities.ShowToastNoti(titleNoti);
                    tabsManager.SetSelected((int) GachaTabId.Rune);
                    return;
                }
            }

            currTabId = indexActive;

            if (currTabId == (int) GachaTabId.Spell)
            {
                ShowGroupKeySpell(true);
                ShowGroupKeyRune(false);
            }
            else
            {
                ShowGroupKeySpell(false);
                ShowGroupKeyRune(true);
            }

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

        private void ShowGroupKeySpell(bool isShow)
        {
            foreach (var keySpell in groupKeySpell)
            {
                keySpell.gameObject.SetActive(isShow);
            }
        }

        private void ShowGroupKeyRune(bool isShow)
        {
            foreach (var keySpell in groupKeyRune)
            {
                keySpell.gameObject.SetActive(isShow);
            }
        }
    }
}