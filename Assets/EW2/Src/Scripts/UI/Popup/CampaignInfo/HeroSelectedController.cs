using System;
using System.Collections.Generic;
using EnhancedUI;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.CampaignInfo.HeroSelect
{
    public class HeroSelectedController : MonoBehaviour
    {
        private const int MaxSlot = 3;

        //[SerializeField] private CampaignInfoWindowController campaignInfoWindowController;
        [SerializeField] private Transform root;
        [SerializeField] private GameObject lockTrialHero;
        [SerializeField] private GameObject lockBehindTrialHero;
        [SerializeField] private Text txtTrialHero;
        [SerializeField] private Button buttonLockHero;

        private SmallList<HeroSelectedData> datas;

        private HeroSelectedView[] uiList;

        public void Start()
        {
            buttonLockHero.onClick.AddListener(ButtonLockHeroClick);
            
        }

        private void OnEnable()
        {
            txtTrialHero.text = L.popup.hero_pre_trial_tag_txt.ToUpper();
        }

        private void ButtonLockHeroClick()
        {
            Ultilities.ShowToastNoti(L.popup.hero_pre_trial_toast_txt);
        }
        
        public void SetInfo(List<HeroSelectedData> selectedList, bool isTrial = false)
        {
            if (selectedList == null || selectedList.Count > MaxSlot)
                return;

            if (datas == null)
            {
                datas = new SmallList<HeroSelectedData>();
            }
            else
            {
                datas.Clear();
            }

            for (int i = 0; i < selectedList.Count; i++)
            {
                var heroData = selectedList[i];
                datas.Add(new HeroSelectedData() {heroId = heroData.heroId, level = heroData.level, slot = i});
            }

            for (int i = selectedList.Count; i < MaxSlot; i++)
            {
                datas.Add(new HeroSelectedData() {heroId = 0, level = 0, slot = i});
            }

            uiList = root.GetComponentsInChildren<HeroSelectedView>();

            if (uiList.Length != datas.Count)
            {
                throw new Exception($"data is not valid: {uiList.Length} != {datas.Count}");
            }

            for (int i = 0; i < MaxSlot; i++)
            {
                uiList[i].SetInfo(datas[i]);
            }

            if (lockBehindTrialHero)
                lockBehindTrialHero.SetActive(isTrial);

            if (lockTrialHero)
                lockTrialHero.SetActive(isTrial);

            //init notify
            ShowNotify();
        }

        public void AddHero(HeroSelectedData data)
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = datas[i];
                if (heroData.heroId == 0)
                {
                    heroData.SetData(data);

                    uiList[i].SetInfo(heroData);
                    ShowNotify();
                    return;
                }
            }
        }

        public void RemoveHero(int heroId)
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = datas[i];
                if (heroData.heroId == heroId)
                {
                    heroData.heroId = 0;

                    heroData.level = 0;

                    uiList[i].SetInfo(heroData);
                    ShowNotify();
                    return;
                }
            }
        }

        public bool CanAddHero()
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                var heroData = datas[i];
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
                var heroData = datas[i];
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
                if (uiList[i].Hero.heroId <= 0)
                {
                    availableSlots++;
                }
                else if (uiList[i].Hero.heroId > 0 && userHeroes.CheckHeroUnlocked(uiList[i].Hero.heroId))
                {
                    selectedHeroes++;
                }
            }

            for (int i = 0; i < MaxSlot; i++)
            {
                if (userHeroes.NumberHeroUnlocked() > selectedHeroes && uiList[i].Hero.heroId <= 0)
                {
                    uiList[i].Notify(true);
                }
                else
                {
                    uiList[i].Notify(false);
                }
            }
        }
    }
}