using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using EW2.CampaignInfo.HeroSelect;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class HeroBarTournamentController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        public TournamentHeroSelectController heroSelectController;
        /// <summary>
        /// Reference to the cell prefab
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;

        /// <summary>
        /// Reference to the scrollers
        /// </summary>
        public EnhancedScroller hScroller;

        private SmallList<HeroBarData> _data;

        public SmallList<HeroBarData> Data => this._data;

        public void SetInfo()
        {
            if (this._data == null)
            {
                // set up the scroller delegates
                hScroller.Delegate = this;

                // set up some simple data
                this._data = new SmallList<HeroBarData>();
            }
            else
            {
                this._data.Clear();
            }

            var heroCollection = GameContainer.Instance.GetHeroCollection().heroList;

            foreach (var heroCollectionInfo in heroCollection)
            {
                bool isUnlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(heroCollectionInfo.heroId);

                if (isUnlocked)
                {
                    int level = UserData.Instance.UserHeroData.GetHeroById(heroCollectionInfo.heroId).level;

                    var heroData = new HeroBarData {
                        heroId = heroCollectionInfo.heroId,
                        level = level,
                        Selected = IsHeroSelected(heroCollectionInfo.heroId)
                    };

                    this._data.Add(heroData);
                }
                else
                {
                    this._data.Add(new HeroBarData {heroId = heroCollectionInfo.heroId, level = 0});
                }
            }

            for (int i = this._data.Count; i < 10; i++)
            {
                this._data.Add(new HeroBarData {heroId = 0, level = 0});
            }

            // tell the scroller to reload now that we have the data
            hScroller.ReloadData();

            hScroller.JumpToDataIndex(0);
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return this._data.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 142;
        }
        
        public bool IsHeroSelected(int heroId)
        {
            var userHeroData = UserData.Instance.TournamentData;
            return userHeroData.listHeroSelected.Exists(data => data.heroId == heroId);
        }
        
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            HeroBarItemTournament cellView = scroller.GetCellView(cellViewPrefab) as HeroBarItemTournament;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in
            // the scene hierarchy.
            cellView.name = "Cell " + dataIndex;
            cellView.gameObject.SetActive(true);

            // set the selected callback to the CellViewSelected function of this controller. 
            // this will be fired when the cell's button is clicked
            cellView.selected = CellViewSelected;

            // in this example, we just pass the data to our cell's view which will update its UI
            cellView.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

            cellView.SetData(dataIndex, this._data[dataIndex]);

            // return the cell to the scroller
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
                var selectedDataIndex = ((HeroBarItemTournament)cellView).DataIndex;

                OnClickHeroAvatar(selectedDataIndex);
            }
        }

        private void OnClickHeroAvatar(int dataIndex)
        {
            var heroData = this._data[dataIndex];
            if (heroData.Selected)
            {
                OnHeroUnselected(heroData);
            }
            else
            {
                OnHeroSelected(heroData);
            }
        }

        private void OnHeroSelected(HeroBarData heroData)
        {
            if (this.heroSelectController.CanAddHero())
            {
                heroData.Selected = true;
                SetHero(new HeroSelectedData {
                    heroId = heroData.heroId, level = heroData.level
                });
            }
            else
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.cannot_select_more_hero);
            }
        }

        private void OnHeroUnselected(HeroBarData heroData)
        {
            heroData.Selected = false;

            SetHero(new HeroSelectedData {heroId = heroData.heroId, level = 0});
        }
        
        public void SetHero(HeroSelectedData data)
        {
            if (data.level > 0)
            {
                this.heroSelectController.AddHero(data);
            }
            else
            {
                this.heroSelectController.RemoveHero(data.heroId);
            }

            ConfirmHeroSelected();
        }

        private void ConfirmHeroSelected()
        {
            var userHeroData = UserData.Instance.TournamentData;
            //userHeroData.SelectedHeroes.Clear();

            var heroList = this.heroSelectController.GetHeroList();
            var newSlectedHeroes = new List<HeroSelectedData>();
            for (int i = 0; i < heroList.Count; i++)
            {
                //if (!userHeroData.CheckHeroUnlocked(heroList[i])) continue;

                newSlectedHeroes.Add(new HeroSelectedData() { slot = i, heroId = heroList[i] });
            }

            //Save selected hero into user data
            //userHeroData.SetDefaultSelectedHeroes(newSlectedHeroes);
            userHeroData.SetSelectedHeroes(newSlectedHeroes);
            UserData.Instance.Save();
            // foreach (var h in userHeroData.GetListHeroes())
            // {
            //     Debug.LogAssertion($"Selected {h.ToString()}");
            // }
        }
    }
}