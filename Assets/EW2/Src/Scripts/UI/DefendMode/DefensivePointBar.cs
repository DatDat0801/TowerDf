using System;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2
{
    public class DefensivePointBar : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScrollerCellView cellViewPrefab;
        [SerializeField] private EnhancedScroller hScroller;
        [SerializeField] private float itemPreSize;
        [SerializeField] private SelectDefendPointWindow controll;


        private List<DefensivePointSelectedData> _lstDFPData = new List<DefensivePointSelectedData>();
        private bool _isInitedPool;

        public void SetData(List<DefensivePointSelectedData> listData)
        {
            this._lstDFPData.Clear();
            this._lstDFPData.AddRange(listData);
            if (!this._isInitedPool)
            {
                this._isInitedPool = true;
                this.hScroller.Delegate = this;
            }
            else
            {
                this.hScroller.ReloadData();
            }
        }

        #region Enhance Delegate

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return this._lstDFPData.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return this.itemPreSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            ItemDefensivePointController
                cellView = scroller.GetCellView(cellViewPrefab) as ItemDefensivePointController;
            if (cellView != null)
            {
                cellView.SetData(this._lstDFPData[dataIndex], CellClick);

                return cellView;
            }

            return null;
        }

        private void CellClick(int dfpSelect)
        {
            foreach (var defensivePoint in this._lstDFPData)
            {
                if (defensivePoint.defensivePointId == dfpSelect)
                    defensivePoint.isSelected = true;
                else
                    defensivePoint.isSelected = false;
            }

            this.controll.UpdateDfpSelect(dfpSelect);
            this.hScroller.ReloadData();
        }

        #endregion
    }
}