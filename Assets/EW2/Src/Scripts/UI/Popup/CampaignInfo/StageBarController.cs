using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using EW2.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EW2.CampaignInfo.StageSelect
{
    public class StageBarController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        public CampaignInfoWindowController campaignInfoWindowController;

        /// <summary>
        /// Reference to the cell prefab
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;

        /// <summary>
        /// Reference to the scrollers
        /// </summary>
        public EnhancedScroller hScroller;

        private int worldId;

        private int stageId;

        private int modeId;

        /// <summary>
        /// The list of inventory data
        /// </summary>
        private SmallList<StageBarData> data;

        void Start()
        {
            // set up the scroller delegates
            hScroller.Delegate = this;
            
        }

        public void SetInfo(int worldId, int stageId, int modeId)
        {
            this.worldId = worldId;

            this.stageId = stageId;

            this.modeId = modeId;

            Reload();

            SelectStage(this.stageId);
        }

        /// <summary>
        /// This function sets up our inventory data and tells the scrollers to reload
        /// </summary>
        private void Reload()
        {
            // if the data existed previously, loop through
            // and remove the selection change handlers before
            // clearing out the data.
            if (data != null)
            {
                for (var i = 0; i < data.Count; i++)
                {
                    data[i].selectedChanged = null;
                }
            }

            // set up a new inventory list
            data = new SmallList<StageBarData>();

            var stageSize = GameContainer.Instance.GetWorldMapSize(worldId);

            for (int i = 0; i < stageSize; i++)
            {
                data.Add(new StageBarData() {worldId = this.worldId, modeId = this.modeId});
            }

            // tell the scrollers to reload
            float percentage = (float)stageId / stageSize;
            //Debug.LogWarning($"percentage : &{percentage}");
            hScroller.ReloadData(percentage);
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if (data != null)
                return data.Count;
            return 0;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 142;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            StageBarCellView cellView = scroller.GetCellView(cellViewPrefab) as StageBarCellView;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in
            // the scene hierarchy.
            cellView.name = "Cell " + dataIndex;

            // set the selected callback to the CellViewSelected function of this controller. 
            // this will be fired when the cell's button is clicked
            cellView.selected = CellViewSelected;

            // set the data for the cell
            cellView.SetData(dataIndex, data[dataIndex]);
            
            // return the cell view to the scroller
            return cellView;
        }

        /// <summary>
        /// This function handles the cell view's button click event
        /// </summary>
        /// <param name="cellView">The cell view that had the button clicked</param>
        private void CellViewSelected(EnhancedScrollerCellView cellView)
        {
            if (cellView == null)
            {
                // nothing was selected
            }
            else
            {
                // get the selected data index of the cell view
                var selectedDataIndex = ((StageBarCellView) cellView).DataIndex;

                campaignInfoWindowController.SetStage(selectedDataIndex);

                SelectStage(selectedDataIndex);
            }
        }

        private void SelectStage(int stageId)
        {
            // loop through each item in the data list and turn
            // on or off the selection state. This is done so that
            // any previous selection states are removed and new
            // ones are added.
            for (var i = 0; i < data.Count; i++)
            {
                data[i].Selected = (stageId == i);
            }

        }
                
        /// <summary>
        /// Focus the stage scroll rect on current
        /// </summary>
        [Button]
        public async void SetScrollRect(int dataIndex)
        {
            await UniTask.Delay(200);
            //var index = hScroller.GetCellViewAtDataIndex(stageId) as StageBarCellView;
            
            hScroller.JumpToDataIndex(dataIndex);
            
        }
    }
}