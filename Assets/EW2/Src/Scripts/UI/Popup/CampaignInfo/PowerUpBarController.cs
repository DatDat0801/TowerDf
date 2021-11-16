using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2.CampaignInfo.PowerUpSelect
{
    public class PowerUpBarController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Reference to the cell prefab
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;
        
        /// <summary>
        /// Reference to the scrollers
        /// </summary>
        public EnhancedScroller hScroller;
        
        private List<PowerUpBarData> data;
        
        void Start()
        {
            // set up the scroller delegates
            hScroller.Delegate = this;

            // set up some simple data
            data = new List<PowerUpBarData>();
            for (var i = 0; i < 10; i++)
                data.Add(new PowerUpBarData(){powerId = -1, quantity = 0});

            // tell the scroller to reload now that we have the data
            hScroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return data.Count;
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
            PowerUpBarCellView cellView = scroller.GetCellView(cellViewPrefab) as PowerUpBarCellView;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in
            // the scene hierarchy.
            cellView.name = "Cell " + dataIndex.ToString();

            // in this example, we just pass the data to our cell's view which will update its UI
            cellView.SetData(dataIndex, data[dataIndex]);

            // return the cell to the scroller
            return cellView;
        }
    }
}