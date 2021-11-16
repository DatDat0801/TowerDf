using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;

namespace EW2.CampaignInfo.HeroSelect
{
    public class HeroBarController : MonoBehaviour, IEnhancedScrollerDelegate
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

        private SmallList<HeroBarData> data;

        public SmallList<HeroBarData> Data => data;

        public void SetInfo()
        {
            if (data == null)
            {
                // set up the scroller delegates
                hScroller.Delegate = this;

                // set up some simple data
                data = new SmallList<HeroBarData>();
            }
            else
            {
                data.Clear();
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
                        Selected = campaignInfoWindowController.IsHeroSelected(heroCollectionInfo.heroId)
                    };

                    data.Add(heroData);
                }
                else
                {
                    data.Add(new HeroBarData {heroId = heroCollectionInfo.heroId, level = 0});
                }
            }

            for (int i = data.Count; i < 10; i++)
            {
                data.Add(new HeroBarData {heroId = 0, level = 0});
            }

            // tell the scroller to reload now that we have the data
            hScroller.ReloadData();

            hScroller.JumpToDataIndex(0);
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
            HeroBarCellView cellView = scroller.GetCellView(cellViewPrefab) as HeroBarCellView;

            // set the name of the game object to the cell's data index.
            // this is optional, but it helps up debug the objects in
            // the scene hierarchy.
            cellView.name = "Cell " + dataIndex;

            // set the selected callback to the CellViewSelected function of this controller. 
            // this will be fired when the cell's button is clicked
            cellView.selected = CellViewSelected;

            // in this example, we just pass the data to our cell's view which will update its UI
            cellView.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

            cellView.SetData(dataIndex, data[dataIndex]);

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
                var selectedDataIndex = ((HeroBarCellView)cellView).DataIndex;

                OnClickHeroAvatar(selectedDataIndex);
            }
        }

        private void OnClickHeroAvatar(int dataIndex)
        {
            var heroData = data[dataIndex];
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
            if (campaignInfoWindowController.CanAddHero())
            {
                heroData.Selected = true;
                campaignInfoWindowController.SetHero(new HeroSelectedData {
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

            campaignInfoWindowController.SetHero(new HeroSelectedData {heroId = heroData.heroId, level = 0});
        }
    }
}