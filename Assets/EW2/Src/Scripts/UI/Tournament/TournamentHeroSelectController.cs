using System;
using System.Collections.Generic;
using EnhancedUI;
using EW2.CampaignInfo.HeroSelect;
using UnityEngine;

namespace EW2
{
    public class TournamentHeroSelectController : MonoBehaviour
    {
        private const int MaxSlot = 3;
        
        [SerializeField] private Transform root;
        
        
        private SmallList<HeroSelectedData> _datas;
        private HeroSelectedView[] _uiList;

        public void ShowListHeroUsed()
        {
            var listHeroSelected = UserData.Instance.TournamentData.listHeroSelected;
            if (listHeroSelected.Count == 0)
            {
                var userHeroData = UserData.Instance.UserHeroData;
                UserData.Instance.TournamentData.listHeroSelected.AddRange(userHeroData.SelectedHeroes);
                SetInfo(userHeroData.SelectedHeroes);
            }
            else
            {
                SetInfo(listHeroSelected);
            }
        }
        public void SetInfo(List<HeroSelectedData> selectedList)
        {
            if (selectedList == null || selectedList.Count > MaxSlot)
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

            for (int i = selectedList.Count; i < MaxSlot; i++)
            {
                this._datas.Add(new HeroSelectedData() {heroId = 0, level = 0, slot = i});
            }

            this._uiList = root.GetComponentsInChildren<HeroSelectedView>();

            if (this._uiList.Length != this._datas.Count)
            {
                throw new Exception($"data is not valid: {this._uiList.Length} != {this._datas.Count}");
            }

            for (int i = 0; i < MaxSlot; i++)
            {
                this._uiList[i].SetInfo(this._datas[i]);
            }

            //init notify
            ShowNotify();
        }
                public void AddHero(HeroSelectedData data)
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = this._datas[i];
                if (heroData.heroId == 0)
                {
                    heroData.SetData(data);

                    this._uiList[i].SetInfo(heroData);
                    ShowNotify();
                    return;
                }
            }
        }

        public void RemoveHero(int heroId)
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = this._datas[i];
                if (heroData.heroId == heroId)
                {
                    heroData.heroId = 0;

                    heroData.level = 0;

                    this._uiList[i].SetInfo(heroData);
                    ShowNotify();
                    return;
                }
            }
        }

        public bool CanAddHero()
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = this._datas[i];
                if (heroData.heroId == 0)
                    return true;
            }

            return false;
        }

        public List<int> GetHeroList()
        {
            var heroList = new List<int>();

            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = this._datas[i];
                if (heroData.heroId > 0)
                    heroList.Add(heroData.heroId);
            }

            return heroList;
        }

        /// <summary>
        /// Show notify icon if user can use more hero
        /// </summary>
        private void ShowNotify()
        {
            int availableSlots = 0;
            int selectedHeroes = 0;
            var userHeroes = UserData.Instance.UserHeroData;
            for (int i = 0; i < MaxSlot; i++)
            {
                if (this._uiList[i].Hero.heroId <= 0)
                {
                    availableSlots++;
                }
                else if (this._uiList[i].Hero.heroId > 0 && userHeroes.CheckHeroUnlocked(this._uiList[i].Hero.heroId))
                {
                    selectedHeroes++;
                }
            }

            for (int i = 0; i < MaxSlot; i++)
            {
                if (userHeroes.NumberHeroUnlocked() > selectedHeroes && this._uiList[i].Hero.heroId <= 0)
                {
                    this._uiList[i].Notify(true);
                }
                else
                {
                    this._uiList[i].Notify(false);
                }
            }
        }
    }
}