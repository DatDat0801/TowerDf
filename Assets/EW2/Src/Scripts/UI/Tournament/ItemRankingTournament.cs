using System;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EW2.CampaignInfo.HeroSelect;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using ZitgaTournamentMode;

namespace EW2
{
    public class ItemRankingTournament : EnhancedScrollerCellView
    {
        [SerializeField] private Text txtRank;
        [SerializeField] private Text txtName;
        [SerializeField] private Text txtWave;
        [SerializeField] private Text txtScore;
        [SerializeField] private Image imgAvatar;
        [SerializeField] private Image imgMedal;
        [SerializeField] private Transform imgArrow;
        [SerializeField] private List<HeroBarCellView> fomations;
        [SerializeField] private List<ItemTowerAvatarController> towers;
        [SerializeField] private GameObject towerGroups;
        [SerializeField] private Button itemButton;
        [SerializeField] private RectTransform bgrContent;

        private RankingTournament _rankingData;
        private int _indexItem;
        private Action<int> _itemClick;
        List<HeroBarData> _fomations = new List<HeroBarData>();
        private LeaderboardArenaTabId _currTab;

        private void Awake()
        {
            this.itemButton.onClick.AddListener(ItemClick);
        }

        private void ItemClick()
        {
            this._itemClick?.Invoke(this._indexItem);
        }

        public void SetData(int indexData, RankingTournament rank, Action<int> itemClickCb,
            LeaderboardArenaTabId arenaTabId)
        {
            this._indexItem = indexData;
            this._itemClick = itemClickCb;
            this._rankingData = rank;
            this._currTab = arenaTabId;
            ShowUi();
            ShowMore();
        }

        private void ShowUi()
        {
            if (this._rankingData != null)
            {
                var rank = this._currTab == LeaderboardArenaTabId.Season
                    ? this._rankingData.RankSeason
                    : this._rankingData.RankTopPlayer;
                this.txtName.text = this._rankingData.Name;
                this.txtWave.text = this._rankingData.WaveCleared.ToString();
                this.txtRank.text = rank > 0 ? rank.ToString() : "--";
                this.imgAvatar.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
                this.txtScore.text = this._rankingData.CreepCleared.ToString();

                var medalId = Ultilities.GetRankTournamentId(rank);
                if (medalId >= 0)
                {
                    this.imgMedal.sprite = ResourceUtils.GetRankIconSmallTournament(medalId);
                    this.imgMedal.gameObject.SetActive(true);
                }
                else
                {
                    this.imgMedal.gameObject.SetActive(false);
                }


                var fomationsData = GetFomations();

                for (int i = 0; i < fomations.Count; i++)
                {
                    if (i < fomationsData.Count)
                    {
                        this.fomations[i].SetData(i, fomationsData[i]);
                        this.fomations[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        this.fomations[i].gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < this.towers.Count; i++)
                {
                    this.towers[i].InitItem(2001 + i);
                }
            }
        }

        private List<HeroBarData> GetFomations()
        {
            this._fomations.Clear();

            if (this._rankingData != null)
            {
                if (this._rankingData.ListOfHeroes == null) return this._fomations;

                foreach (var hero in this._rankingData.ListOfHeroes)
                {
                    this._fomations.Add(new HeroBarData() {
                        heroId = hero.HeroId, level = hero.HeroLevel, Selected = false
                    });
                }
            }
            else
            {
                var userData = UserData.Instance.UserHeroData;
                foreach (var hero in userData.SelectedHeroes)
                {
                    this._fomations.Add(new HeroBarData() {heroId = hero.heroId, level = hero.level, Selected = false});
                }
            }

            return this._fomations;
        }

        private void ShowMore()
        {
            this.imgArrow.rotation = this._rankingData.IsShowMore
                ? Quaternion.Euler(new Vector3(0f, 0f, 180f))
                : Quaternion.Euler(Vector3.zero);
            this.bgrContent.sizeDelta =
                this._rankingData.IsShowMore ? new Vector2(1595f, 310f) : new Vector2(1595f, 170f);
            this.towerGroups.SetActive(this._rankingData.IsShowMore);
        }
    }
}