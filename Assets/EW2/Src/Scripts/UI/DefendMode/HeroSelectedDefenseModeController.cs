using System;
using System.Collections.Generic;
using EnhancedUI;
using EW2.CampaignInfo.HeroSelect;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroSelectedDefenseModeController : MonoBehaviour
    {
        [SerializeField] private List<ItemHeroSelectedDefense> uiList;

        private SmallList<HeroSelectedData> _datas;

        public void ShowListHeroUsed()
        {
            var listHeroSelected = UserData.Instance.UserHeroDefenseData.listHeroSelected;
            if (listHeroSelected.Count == 0)
            {
                var userHeroData = UserData.Instance.UserHeroData;
                UserData.Instance.UserHeroDefenseData.listHeroSelected.AddRange(userHeroData.SelectedHeroes);
                SetInfo(userHeroData.SelectedHeroes);
            }
            else
            {
                SetInfo(listHeroSelected);
            }
        }

        private void SetInfo(List<HeroSelectedData> selectedList)
        {
            if (selectedList == null || selectedList.Count > GameConfig.MAX_SLOT_HERO_DEFENSE)
                return;

            if (this._datas == null)
            {
                this._datas = new SmallList<HeroSelectedData>();
            }
            else
            {
                this._datas.Clear();
            }

            for (int i = 0; i < selectedList.Count; i++)
            {
                var heroData = selectedList[i];
                this._datas.Add(new HeroSelectedData() {heroId = heroData.heroId, level = heroData.level, slot = i});
            }

            for (int i = selectedList.Count; i < GameConfig.MAX_SLOT_HERO_DEFENSE; i++)
            {
                this._datas.Add(new HeroSelectedData() {heroId = 0, level = 0, slot = i});
            }


            if (uiList.Count != this._datas.Count)
            {
                throw new Exception($"data is not valid: {uiList.Count} != {this._datas.Count}");
            }

            for (int i = 0; i < GameConfig.MAX_SLOT_HERO_DEFENSE; i++)
            {
                uiList[i].SetInfo(this._datas[i]);
                uiList[i].SetItemClickCb(ItemHeroSelectClick);
                uiList[i].Notify(this._datas[i].heroId == 0);
            }
        }

        private void ItemHeroSelectClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.hero_defend_select_hero);
        }

        public void UpdateListHeroSelected()
        {
            var listHeroSelected = UserData.Instance.UserHeroDefenseData.listHeroSelected;
            SetInfo(listHeroSelected);
        }

        public void UpdateListHeroPreview(List<HeroSelectedData> selectedList)
        {
            SetInfo(selectedList);
        }
    }
}