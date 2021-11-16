using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.CampaignInfo.StageSelect
{
    /// <summary>
    /// This delegate handles the UI's button click
    /// </summary>
    /// <param name="cellView">The cell view that had the button click</param>
    public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);

    public class StageBarCellView : EnhancedScrollerCellView
    {
        [SerializeField] private Button buttonStage;

        [SerializeField] private Text txtStage;

        [SerializeField] private Image[] stars;

        [SerializeField] private GameObject stageLock;

        [SerializeField] private GameObject stageSlot;

        [SerializeField] private GameObject stagePlayed;

        [SerializeField] private GameObject stageSelected;

        [SerializeField] private GameObject stageCurrent;

        /// <summary>
        /// Reference to the underlying data driving this view
        /// </summary>
        private StageBarData data;

        private bool isUnlockStage;

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
            buttonStage.onClick.AddListener(OnSelected);
        }

        public void SetData(int dataIndex, StageBarData data)
        {
            // if there was previous data assigned to this cell view,
            // we need to remove the handler for the selection change
            if (data != null)
            {
                data.selectedChanged -= SelectedChanged;
            }

            // link the data to the cell view
            DataIndex = dataIndex;
            this.data = data;

            txtStage.text = (this.DataIndex + 1).ToString();

            if (this.data.modeId == 0)
            {
                isUnlockStage = UserData.Instance.CampaignData.IsUnlockedStage(data.worldId, DataIndex);
            }
            else
            {
                isUnlockStage =
                    UserData.Instance.CampaignData.IsUnlockedHardStage(data.worldId, dataIndex);
            }

            SetUnlockStage(isUnlockStage);

            if (isUnlockStage)
            {
                var isCurrentStage =
                    UserData.Instance.CampaignData.IsCurrentStage(data.worldId, DataIndex, data.modeId);

                SetCurrentStage(isCurrentStage);

                if (isCurrentStage == false)
                {
                    SetStar();
                }
            }

            // set up a handler so that when the data changes
            // the cell view will update accordingly. We only
            // want a single handler for this cell view, so 
            // first we remove any previous handlers before
            // adding the new one
            data.selectedChanged -= SelectedChanged;
            data.selectedChanged += SelectedChanged;

            // update the selection state UI
            SelectedChanged(data.Selected);
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
        /// <param name="selected">The selection state of the cell</param>
        private void SelectedChanged(bool selected)
        {
            //print("Selected: " + selected);
            stageSelected.SetActive(selected);
        }

        /// <summary>
        /// This function is called by the cell's button click event
        /// </summary>
        private void OnSelected()
        {
            if (isUnlockStage)
            {
                // if a handler exists for this cell, then
                // call it.
                selected?.Invoke(this);
            }
            else
            {
                if (data.modeId == CampaignMode.Normal)
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.locked_mode_normal);
                }
                else
                {
                    var content = string.Format(L.popup.locked_mode_notice, DataIndex + 1);
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, content);
                }
            }
        }

        private void SetUnlockStage(bool unlocked)
        {
            stageLock.SetActive(!unlocked);
            stageSlot.SetActive(unlocked);
        }

        private void SetCurrentStage(bool isCurrentStage)
        {
            stageCurrent.SetActive(isCurrentStage);
            stagePlayed.SetActive(!isCurrentStage);
        }

        private void SetStar()
        {
            int star = UserData.Instance.CampaignData.GetStar(data.worldId, DataIndex, data.modeId);

            var fillImage = ResourceUtils.GetSpriteAtlas("stars", data.modeId == 0 ? "1" : "2");
            var emptyImage = ResourceUtils.GetSpriteAtlas("stars", "0");
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].sprite = i < star ? fillImage : emptyImage;
            }
        }
    }
}
