using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EW2.CampaignInfo.HeroSelect;
using UnityEngine;
using UnityEngine.UI;
using ZitgaRankingDefendMode;

namespace EW2
{
    public class ItemRankingHeroDefense : EnhancedScrollerCellView
    {
        [SerializeField] private Text txtRank;
        [SerializeField] private Text txtName;
        [SerializeField] private Text txtWave;
        [SerializeField] private Image imgAvatar;
        [SerializeField] private Image imgAvatarBorder;
        [SerializeField] private Image imgDefensivePoint;
        [SerializeField] private Image imgLeaderRank;
        [SerializeField] private List<HeroBarCellView> fomations;

        private RankingDefense _rankingDefense;

        public void SetData(RankingDefense rank)
        {
            this._rankingDefense = rank;
            ShowUi();
        }

        private void ShowUi()
        {
            if (this._rankingDefense != null)
            {
                if (this._rankingDefense.Rank <= 2)
                {
                    this.txtRank.gameObject.SetActive(false);
                    this.imgLeaderRank.sprite =
                        ResourceUtils.GetSpriteAtlas("icon_rank_dfp", $"icon_medals_{this._rankingDefense.Rank}");
                    this.imgLeaderRank.gameObject.SetActive(true);
                    this.imgLeaderRank.SetNativeSize();
                    this.imgAvatarBorder.sprite =
                        ResourceUtils.GetSpriteAtlas("icon_rank_dfp", $"icon_rank_{this._rankingDefense.Rank}");
                    this.imgAvatarBorder.SetNativeSize();
                    var sizeImg = this.imgAvatarBorder.GetComponent<RectTransform>().sizeDelta;
                    sizeImg = new Vector2(sizeImg.x * 1.3f, sizeImg.y * 1.3f);
                    this.imgAvatarBorder.GetComponent<RectTransform>().sizeDelta = sizeImg;
                }
                else
                {
                    this.txtRank.text = (this._rankingDefense.Rank + 1).ToString();
                    this.txtRank.gameObject.SetActive(true);
                    this.imgLeaderRank.gameObject.SetActive(false);
                    this.imgAvatarBorder.sprite = ResourceUtils.GetSpriteAtlas("avatar", $"avatar_border");
                    this.imgAvatarBorder.SetNativeSize();
                }

                this.txtName.text = this._rankingDefense.Name;
                this.txtWave.text = this._rankingDefense.WaveCleared.ToString();
                this.imgDefensivePoint.sprite = ResourceUtils.GetSpriteAtlas("defensive_point",
                    $"icon_defensive_point_{this._rankingDefense.DefensivePoint}");
                this.imgAvatar.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
            }
            else
            {
                var userData = UserData.Instance;
                this.txtRank.text = "--";
                this.imgLeaderRank.gameObject.SetActive(false);
                this.txtName.text = userData.AccountData.userName;
                this.txtWave.text = "0";
                this.imgDefensivePoint.sprite = ResourceUtils.GetSpriteAtlas("defensive_point",
                    $"icon_defensive_point_{userData.UserHeroDefenseData.defensePointId}");
                this.imgAvatar.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
                this.imgAvatarBorder.sprite = ResourceUtils.GetSpriteAtlas("avatar", $"avatar_border");
                this.imgAvatarBorder.SetNativeSize();
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
        }

        private List<HeroBarData> GetFomations()
        {
            List<HeroBarData> results = new List<HeroBarData>();

            if (this._rankingDefense != null)
            {
                foreach (var hero in this._rankingDefense.ListOfHeroes)
                {
                    results.Add(new HeroBarData() {heroId = hero.HeroId, level = hero.HeroLevel, Selected = false});
                }
            }
            else
            {
                var userData = UserData.Instance.UserHeroData;
                foreach (var hero in userData.SelectedHeroes)
                {
                    results.Add(new HeroBarData() {heroId = hero.heroId, level = hero.level, Selected = false});
                }
            }

            return results;
        }
    }
}