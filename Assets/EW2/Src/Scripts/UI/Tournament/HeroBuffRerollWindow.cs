using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EW2.CampaignInfo.HeroSelect;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using Random = UnityEngine.Random;

namespace EW2
{
    public class HeroBuffRerollWindow : AWindowController
    {
        private const int INDEX_ADS_DATA = 8;
        private const int MIN_FONT_SIZE = 18;

        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtTitleHeroBuffReroll;
        [SerializeField] private Text txtTipHeroBuffReroll;
        [SerializeField] private Text txtButtonHeroBuffReroll;
        [SerializeField] private Text txtPriceHeroBuffReroll;
        [SerializeField] private Text txtTitleBuffChange;
        [SerializeField] private Text txtDescBuffOld;
        [SerializeField] private Text txtDescBuffCurr;
        [SerializeField] private Text txtButtonChangeBuff;
        [SerializeField] private Text txtPriceChangeBuff;
        [SerializeField] private Text txtButtonAdsChangeBuff;

        [SerializeField] private Button btnHeroReroll;
        [SerializeField] private Button btnChangeBuff;
        [SerializeField] private Button btnWatchAdsChangeBuff;
        [SerializeField] private Button btnClose;

        [SerializeField] private List<HeroSelectedView> listHeroBuffUi;

        private RerollDataConfig _rerollDataConfig;
        private List<TournamentStatData> _tournamentBuffDatas;

        private void OnEnable()
        {
            VideoAdPlayer.Instance.OnRewarded += WatchAdsSuccess;
        }

        private void OnDisable()
        {
            if (VideoAdPlayer.Instance != null)
                VideoAdPlayer.Instance.OnRewarded -= WatchAdsSuccess;
        }

        protected override void Awake()
        {
            base.Awake();
            this.btnHeroReroll.onClick.AddListener(HeroReroll);
            this.btnChangeBuff.onClick.AddListener(ChangeBuff);
            this.btnWatchAdsChangeBuff.onClick.AddListener(WatchAdsChangeBuff);
            this.btnClose.onClick.AddListener(ClosePopup);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            GetDataConfig();

            SetLocalization();

            ShowUiHeroBuff();

            ShowUiPriceChange();

            ShowUiDescBuff();
        }

        private void ClosePopup()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_change_buff);
        }

        private void WatchAdsChangeBuff()
        {
            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var addData = adRewardData.ads[INDEX_ADS_DATA];
            if (addData != null)
            {
                VideoAdPlayer.Instance.PlayAdClick(addData.placementName);
            }
        }

        private void WatchAdsSuccess(string arg0)
        {
            HandleChangeBuff();
        }

        private void ChangeBuff()
        {
            var numberCurrency = UserData.Instance.GetMoney(this._rerollDataConfig.typeMoneyChangeBuff);
            if (numberCurrency < this._rerollDataConfig.priceChangeBuff)
            {
                PopupUtils.ShowPopupSuggestBuyResource(this._rerollDataConfig.typeMoneyChangeBuff);
                return;
            }

            UserData.Instance.SubMoney(this._rerollDataConfig.typeMoneyChangeBuff,
                this._rerollDataConfig.priceChangeBuff, AnalyticsConstants.SourceChangeBuffTournament,
                this._rerollDataConfig.typeMoneyChangeBuff.ToString());

            HandleChangeBuff();
        }

        private void HandleChangeBuff()
        {
            var currIndex = 0;
            var userData = UserData.Instance.TournamentData;
            for (int i = 0; i < this._tournamentBuffDatas.Count; i++)
            {
                if (this._tournamentBuffDatas[i].statId == userData.buffStatId)
                {
                    currIndex = i;
                    break;
                }
            }

            userData.buffStatIdPrev = userData.buffStatId;
            userData.buffStatId = currIndex + 1 < this._tournamentBuffDatas.Count
                ? this._tournamentBuffDatas[currIndex + 1].statId
                : this._tournamentBuffDatas[0].statId;
            UserData.Instance.Save();
            ShowUiDescBuff();
            EventManager.EmitEvent(GamePlayEvent.OnChangeHeroBuffTournament);
        }

        private void HeroReroll()
        {
            var numberCurrency = UserData.Instance.GetMoney(this._rerollDataConfig.typeMoneyChangeHero);
            var feeReroll = this._rerollDataConfig.priceChangeHero +
                            (this._rerollDataConfig.valueIncrease * UserData.Instance.TournamentData.numberRerollHero);

            if (numberCurrency < feeReroll)
            {
                PopupUtils.ShowPopupSuggestBuyResource(this._rerollDataConfig.typeMoneyChangeHero);
                return;
            }

            UserData.Instance.SubMoney(this._rerollDataConfig.typeMoneyChangeHero, feeReroll,
                AnalyticsConstants.SourceRerollHeroTournament,
                this._rerollDataConfig.typeMoneyChangeHero.ToString());

            HandleRerollHero();
        }

        private void HandleRerollHero()
        {
            var randomDone = false;
            int[] arrHeroRandom = new int[2] {0, 0};
            var currHeroBuff = UserData.Instance.TournamentData.GetHeroIdBuff();
            var currHeroNerf = UserData.Instance.TournamentData.heroNerfId;

            while (!randomDone)
            {
                arrHeroRandom[0] = 1000 + Random.Range(1, GameConfig.HeroCount + 1);
                arrHeroRandom[1] = 1000 + Random.Range(1, GameConfig.HeroCount + 1);

                if (arrHeroRandom[0] == currHeroNerf || arrHeroRandom[1] == currHeroNerf) continue;

                if (arrHeroRandom[0] != arrHeroRandom[1])
                {
                    if (!currHeroBuff.Contains(arrHeroRandom[0]) || !currHeroBuff.Contains(arrHeroRandom[1]))
                    {
                        randomDone = true;
                    }
                }
            }

            UserData.Instance.TournamentData.SetListHeroBuff(arrHeroRandom.ToList());
            UserData.Instance.TournamentData.numberRerollHero++;
            UserData.Instance.Save();
            ShowUiHeroBuff();
            ShowUiPriceChange();
            EventManager.EmitEvent(GamePlayEvent.OnChangeHeroBuffTournament);
        }

        private void GetDataConfig()
        {
            if (this._rerollDataConfig == null)
            {
                this._rerollDataConfig = new RerollDataConfig();
                var rerollData = GameContainer.Instance.Get<TournamentDataBase>().Get<RerollHeroBuffConfig>();
                if (rerollData != null)
                    this._rerollDataConfig = rerollData.rerollDataConfigs[0];
            }

            if (this._tournamentBuffDatas == null)
            {
                this._tournamentBuffDatas = new List<TournamentStatData>();
                var buffData = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentBuffConfig>();
                if (buffData != null)
                    this._tournamentBuffDatas.AddRange(buffData.tournamentBuffDatas);
            }
        }

        public override void SetLocalization()
        {
            this.txtTitle.text = L.playable_mode.change_buff_txt.ToUpper();
            this.txtTitleHeroBuffReroll.text = L.playable_mode.change_hero_pair_txt;
            this.txtTipHeroBuffReroll.text = L.playable_mode.change_price_alert_txt;
            this.txtButtonHeroBuffReroll.text = L.button.reroll_btn;
            this.txtTitleBuffChange.text = L.playable_mode.change_buff_status_txt;
            this.txtButtonChangeBuff.text = L.button.change_btn;
            this.txtButtonAdsChangeBuff.text = L.button.reward_video_claim;
        }

        private void ShowUiHeroBuff()
        {
            var listHeroBuff = UserData.Instance.TournamentData.listHeroBuff;
            for (int i = 0; i < listHeroBuff.Count; i++)
            {
                if (i < this.listHeroBuffUi.Count)
                {
                    listHeroBuffUi[i].SetInfo(listHeroBuff[i]);
                }
            }
        }

        private void ShowUiPriceChange()
        {
            var feeReroll = this._rerollDataConfig.priceChangeHero +
                            (this._rerollDataConfig.valueIncrease * UserData.Instance.TournamentData.numberRerollHero);
            this.txtPriceHeroBuffReroll.text = feeReroll.ToString();
            this.txtPriceChangeBuff.text = this._rerollDataConfig.priceChangeBuff.ToString();
        }

        private void ShowUiDescBuff()
        {
            var userData = UserData.Instance.TournamentData;
            var descCurr = "";
            // var descOld = "";
            //
            // if (userData.buffStatIdPrev <= 0)
            // {
            //     descOld = GetDescBuff(userData.buffStatId);
            // }
            // else
            // {
            //     descOld = GetDescBuff(userData.buffStatIdPrev);
            //     descCurr = GetDescBuff(userData.buffStatId);
            // }
            //
            // this.txtDescBuffOld.text = descOld;
            descCurr = GetDescBuff(userData.buffStatId);
            this.txtDescBuffCurr.text = descCurr;

            if (UserData.Instance.SettingData.userLanguage == SystemLanguage.Japanese)
            {
                StringBuilder content = new StringBuilder();
                bool isPlit = false;

                // if (this.txtDescBuffOld.cachedTextGenerator.fontSizeUsedForBestFit < MIN_FONT_SIZE)
                // {
                //     for (int i = 0; i < descOld.Length; i++)
                //     {
                //         if (i >= descOld.Length / 2 && !CheckIsNumber(descOld[i]) && !descOld[i].Equals('%') && !isPlit)
                //         {
                //             isPlit = true;
                //             content.Append("\n");
                //         }
                //
                //         content.Append(descOld[i]);
                //     }
                //
                //     this.txtDescBuffOld.text = content.ToString();
                // }
                //
                // content.Clear();
                // isPlit = false;

                if (this.txtDescBuffCurr.cachedTextGenerator.fontSizeUsedForBestFit < MIN_FONT_SIZE)
                {
                    for (int i = 0; i < descCurr.Length; i++)
                    {
                        if (i >= descCurr.Length / 2 && !CheckIsNumber(descCurr[i]) && !descCurr[i].Equals('%') &&
                            !isPlit)
                        {
                            isPlit = true;
                            content.Append("\n");
                        }

                        content.Append(descCurr[i]);
                    }

                    this.txtDescBuffCurr.text = content.ToString();
                }
            }
        }

        private string GetDescBuff(int buffId)
        {
            var desc = Localization.Current.Get("playable_mode", $"tournament_buff_{buffId}");

            var valueBuff = 0;
            for (int i = 0; i < this._tournamentBuffDatas.Count; i++)
            {
                if (this._tournamentBuffDatas[i].statId == buffId)
                {
                    if (this._tournamentBuffDatas[i].ratioBonus > 0)
                    {
                        valueBuff = (int)(this._tournamentBuffDatas[i].ratioBonus * 100);
                    }
                    else
                    {
                        valueBuff = this._tournamentBuffDatas[i].valueBonus;
                    }

                    break;
                }
            }

            desc = string.Format(desc, valueBuff);

            return desc;
        }

        bool CheckIsNumber(char text)
        {
            return char.IsDigit(text);
        }
    }
}