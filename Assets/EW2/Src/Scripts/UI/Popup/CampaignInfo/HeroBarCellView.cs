using System;
using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2.CampaignInfo.HeroSelect
{
    /// <summary>
    /// This delegate handles the UI's button click
    /// </summary>
    /// <param name="cellView">The cell view that had the button click</param>
    public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);

    public class HeroBarCellView : EnhancedScrollerCellView
    {
        [SerializeField] private GameObject levelBadge;

        [SerializeField] private Text txtLevel;

        [SerializeField] private Image iconHero;

        [SerializeField] private GameObject iconTick;

        [SerializeField] private GameObject iconLock;

        [SerializeField] private GameObject iconAvatar;

        [SerializeField] private GameObject iconGray;

        [SerializeField] private Button buttonHero;

        /// <summary>
        /// Reference to the underlying data driving this view
        /// </summary>
        private HeroBarData data;

        /// <summary>
        /// Public reference to the index of the data
        /// </summary>
        public int DataIndex { get; private set; }

        /// <summary>
        /// The handler to call when this cell's button traps a click event
        /// </summary>
        public SelectedDelegate selected;

        private void Start()
        {
            buttonHero.onClick.AddListener(OnSelected);
        }


        public void SetData(int index, HeroBarData heroBarData)
        {
            // if there was previous data assigned to this cell view,
            // we need to remove the handler for the selection change
            if (heroBarData != null)
            {
                heroBarData.selectedChanged -= SelectedChanged;
            }

            // link the data to the cell view
            DataIndex = index;
            this.data = heroBarData;

            txtLevel.text = heroBarData.level.ToString();

            iconAvatar.SetActive(heroBarData.heroId > 0);

            if (heroBarData.heroId > 0)
            {
                iconLock.SetActive(heroBarData.level == 0);

                levelBadge.SetActive(heroBarData.level != 0);

                iconTick.SetActive(heroBarData.Selected);

                iconGray.SetActive(heroBarData.level == 0 || heroBarData.Selected);

                iconHero.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_{heroBarData.heroId}");
            }

            // set up a handler so that when the data changes
            // the cell view will update accordingly. We only
            // want a single handler for this cell view, so 
            // first we remove any previous handlers before
            // adding the new one
            heroBarData.selectedChanged -= SelectedChanged;
            heroBarData.selectedChanged += SelectedChanged;

            // update the selection state UI
            if (heroBarData.level > 0)
                SelectedChanged(heroBarData.Selected);
        }

        /// <summary>
        /// This is called if the cell is destroyed. The EnhancedScroller will
        /// not call this since it uses recycling, but we include it in case 
        /// the user decides to destroy the cell anyway
        /// </summary>
        void OnDestroy()
        {
            if (data != null)
            {
                // remove the handler from the data so 
                // that any changes to the data won't try
                // to call this destroyed view's function
                data.selectedChanged -= SelectedChanged;
            }
        }

        /// <summary>
        /// This function changes the UI state when the item is 
        /// selected or unselected.
        /// </summary>
        /// <param name="select">The selection state of the cell</param>
        private void SelectedChanged(bool @select)
        {
            iconGray.SetActive(data.Selected);
            iconTick.SetActive(data.Selected);
        }

        /// <summary>
        /// This function is called by the cell's button click event
        /// </summary>
        private void OnSelected()
        {
            if (data.level > 0)
            {
                // if a handler exists for this cell, then
                // call it.
                selected?.Invoke(this);
            }
            else
            {
                if (data.heroId > 0)
                {
                    GoToUnlockHero();
                }
                else
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
                }
            }
        }

        private void GoToUnlockHero()
        {
            var dataNotice = new PopupNoticeWindowProperties(L.popup.notice_txt,L.popup.hero_not_been_unlock_txt,
                PopupNoticeWindowProperties.PopupType.TwoOption_OkPriority, L.button.btn_unlock_now,
                () =>
                {
                    if (data.heroId ==  (int)HeroType.Marco)
                    {
                        var unlockConditionDataBase = GameContainer.Instance.GetHeroUnlockData();
                        var macroUnlockCondition = Array.Find(unlockConditionDataBase.heroConditions,x => x.heroId ==  (int)HeroType.Marco);
                        if (UserData.Instance.CampaignData.GetStar(0, macroUnlockCondition.unlockStage) == 0)
                        {
                            string notice = string.Format(L.popup.spell_unlock_condition,macroUnlockCondition.unlockStage +1);
                            EventManager.EmitEventData(GamePlayEvent.ShortNoti, notice);
                            return;
                        }
                        else if (UserData.Instance.UserEventData.Hero4BundleUserData.CheckCanShow())
                        {
                            UIFrame.Instance.OpenWindow(ScreenIds.hero_4_bundle);
                            return;
                        }
                    }
                    // UIFrame.Instance.CloseWindow(ScreenIds.campaign_info);
                    UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
                    UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene, new HeroRoomWindowProperties(data.heroId));
                }, L.button.later_name);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, dataNotice);
        }
    }
}
