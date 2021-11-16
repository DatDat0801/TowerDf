using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroListController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller scroller;

        [SerializeField] private float itemSize;

        [SerializeField] private Text txtNumbHeroUnlocked;

        [SerializeField] private EnhancedScrollerCellView cellView;

        private Dictionary<int, HeroCacheData> listHeroes = new Dictionary<int, HeroCacheData>();

        private bool _isInitScroll;

        private int _currHeroSelected;

        private int _currItemIndex;

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnHeroUnlocked, OnHeroUnlocked);
        }

        private void OnHeroUnlocked()
        {
            txtNumbHeroUnlocked.text = $"{UserData.Instance.UserHeroData.NumberHeroUnlocked()}/{GameConfig.HeroCount}";
        }

        public void SetData(Dictionary<int, HeroCacheData> listData)
        {
            listHeroes = listData;
        }

        public void SetHeroSelect(int currHeroId = 0)
        {
            this._currHeroSelected = currHeroId;

            this._currItemIndex = GetCurrDataIndex();

            txtNumbHeroUnlocked.text = $"{UserData.Instance.UserHeroData.NumberHeroUnlocked()}/{GameConfig.HeroCount}";

            if (!this._isInitScroll)
            {
                InitScroll();
            }
            else
            {
                scroller.JumpToDataIndex(this._currItemIndex);
                scroller.ReloadData();
            }
        }

        private int GetCurrDataIndex()
        {
            var listData = this.listHeroes.Values.ToList();
            for (int i = 0; i < listData.Count; i++)
            {
                if (listData[i].HeroId == this._currHeroSelected)
                {
                    return i;
                }
            }

            return 0;
        }

        private void ItemOnClick(int heroId, int itemIndex)
        {
            if (this._currItemIndex != itemIndex)
            {
                _currItemIndex = itemIndex;
                EventManager.EmitEventData(GamePlayEvent.OnHeroSelectChange, heroId);
            }
        }


        #region Scroller event

        private void InitScroll()
        {
            this._isInitScroll = true;

            if (scroller.Delegate == null)
                scroller.Delegate = this;
            else
                scroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if (listHeroes == null)
                return 0;

            return listHeroes.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return itemSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var heroItemUI = scroller.GetCellView(cellView) as HeroItemController;

            heroItemUI.dataIndex = dataIndex;

            var heroId = 1001 + dataIndex;

            heroItemUI.SetData(cellIndex, listHeroes[heroId],
                heroId == this._currHeroSelected, ItemOnClick);

            return heroItemUI;
        }

        #endregion
    }
}