using System;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroItemController : EnhancedScrollerCellView
    {
        [SerializeField] private Image iconHero;

        [SerializeField] private Text heroLevel;

        [SerializeField] private GameObject goCover;

        [SerializeField] private GameObject goLock;

        [SerializeField] private GameObject goFocus;

        [SerializeField] private GameObject iconNoti;

        private HeroItem heroItem;

        private HeroData heroData;

        private int heroId;
        private bool isUnlocked;

        public int HeroId => heroId;

        private int itemIndex;

        private Action<int, int> itemOnClick;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void SetData(int index, HeroCacheData dataCache, bool isSelected, Action<int, int> itemClick)
        {
            this.itemIndex = index;

            this.heroData = GameContainer.Instance.GetHeroData(heroId);

            this.heroId = dataCache.HeroId;

            this.itemOnClick = itemClick;

            this.heroItem = dataCache.HeroItemData;

            ShowUi(isSelected);
        }

        private void ShowUi(bool isSelected)
        {
            bool showNotify;
            isUnlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(this.heroId);

            if (iconHero)
                iconHero.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_{heroId}");

            if (!isUnlocked)
            {
                if (heroLevel)
                    heroLevel.transform.parent.gameObject.SetActive(false);

                if (goCover) goCover.SetActive(true);

                if (goLock) goLock.SetActive(true);

                if (isSelected && !UserData.Instance.UserHeroData.CheckDisplayFirstNotify(heroId))
                {
                    UserData.Instance.UserHeroData.SeeFirstNotify(heroId);
                    UserData.Instance.Save();
                }
            }
            else
            {
                if (heroLevel)
                {
                    heroLevel.text = heroItem.level.ToString();

                    heroLevel.transform.parent.gameObject.SetActive(true);
                }

                if (goCover) goCover.SetActive(false);

                if (goLock) goLock.SetActive(false);
            }

            if (iconNoti)
            {
                showNotify = CheckShowNoti();
                iconNoti.SetActive(showNotify);
            }

            if (goFocus) goFocus.SetActive(isSelected);
        }

        private void OnClick()
        {
            itemOnClick?.Invoke(heroId, this.itemIndex);
        }

        private bool CheckShowNoti()
        {
            if (!UserData.Instance.UserHeroData.CheckDisplayFirstNotify(heroId))
            {
                return true;
            }

            var skillUpgrade = GameContainer.Instance.GetHeroSkillUpgrade();

            var levelSkills = heroItem.levelSkills;

            for (int i = 0; i < levelSkills.Length; i++)
            {
                if (levelSkills[i] >= GameConfig.HeroSkillLevelMax) continue;

                var costUpgrade = skillUpgrade.skillUpgradeDatas[levelSkills[i]].cost;

                if (heroItem.skillPoint >= costUpgrade)
                    return true;
            }

            return false;
        }
    }
}