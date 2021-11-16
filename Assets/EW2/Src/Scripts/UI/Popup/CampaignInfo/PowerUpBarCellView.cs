using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.CampaignInfo.PowerUpSelect
{
    /// <summary>
    /// This delegate handles the UI's button click
    /// </summary>
    /// <param name="cellView">The cell view that had the button click</param>
    public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);
    
    public class PowerUpBarCellView : EnhancedScrollerCellView
    {
        /// <summary>
        /// Reference to the underlying data driving this view
        /// </summary>
        private PowerUpBarData data;
        
        private bool isUnlockStage;

        [SerializeField] private Button buttonPowerUp;
        
        [SerializeField] private Text txtQuantity;

        [SerializeField] private Image icon;
        
        [SerializeField] private GameObject powerUp;
        
        [SerializeField] private GameObject powerUpLock;
        
        [SerializeField] private GameObject powerUpConfirm;
        
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
            buttonPowerUp.onClick.AddListener(OnSelected);
        }
        
        public void SetData(int dataIndex, PowerUpBarData data)
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

            // update logic ui here
            if (data.powerId < 0)
            {
                
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
            Debug.Log("Selected: " + selected);
            //stageSelected.SetActive(selected);
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
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
            }
        }
    }
}