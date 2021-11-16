using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class TournamentOutOfTicketWindow : AWindowController
    {
        private const int INDEX_ADS_DATA = 9;

        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtTip;
        [SerializeField] private Text txtRemainNumberExchange;
        [SerializeField] private Text txtCostExchange;
        [SerializeField] private Text txtLabelExchangeBtn;
        [SerializeField] private Text txtLabelWatchBtn;
        [SerializeField] private Text txtCostExchangeDisable;
        [SerializeField] private Text txtLabelExchangeBtnDisable;
        [SerializeField] private Text txtLabelWatchBtnDisable;
        [SerializeField] private Transform trfReward;
        [SerializeField] private Button exchangeBtn;
        [SerializeField] private Button watchAdsBtn;
        [SerializeField] private Button exchangeBtnDisable;
        [SerializeField] private Button watchAdsBtnDisable;
        [SerializeField] private Button closeBtn;
        [SerializeField] private TimeCountDownUi countdown;
        private TournamentTicketExchange _dataBaseExchange;
        private GridReward _gridReward;
        private Reward _reward;

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
            this.exchangeBtn.onClick.AddListener(ExchangeTicketClick);
            this.watchAdsBtn.onClick.AddListener(WatchAdsClick);
            this.exchangeBtnDisable.onClick.AddListener(ExchangeTicketClick);
            this.watchAdsBtnDisable.onClick.AddListener(WatchAdsClick);
            this.closeBtn.onClick.AddListener(CloseClick);
            this._dataBaseExchange = GameContainer.Instance.Get<TournamentDataBase>().Get<TournamentTicketConfig>()
                .tournamentTicketExchanges[0];
            this._gridReward = new GridReward(this.trfReward);
            this._reward = Reward.Create(ResourceType.Money, MoneyType.TournamentTicket,
                this._dataBaseExchange.valueExchange);
        }

        private void CloseClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_out_of_tournament_ticket);
        }


        private void WatchAdsClick()
        {
            if (UserData.Instance.TournamentData.remainNumberExchange <= 0)
            {
                Ultilities.ShowToastNoti(string.Format(L.popup.no_more_purchase_txt,
                    Ultilities.GetNameCurrency(MoneyType.TournamentTicket)));
                return;
            }

            var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
            var addData = adRewardData.ads[INDEX_ADS_DATA];
            if (addData != null)
            {
                VideoAdPlayer.Instance.PlayAdClick(addData.placementName);
            }
        }

        private void WatchAdsSuccess(string arg0)
        {
            HandleChange("watch_ads");
        }

        private void ExchangeTicketClick()
        {
            if (UserData.Instance.TournamentData.remainNumberExchange <= 0)
            {
                Ultilities.ShowToastNoti(string.Format(L.popup.no_more_purchase_txt,
                    Ultilities.GetNameCurrency(MoneyType.TournamentTicket)));
                return;
            }

            var numberCurrency = UserData.Instance.GetMoney(this._dataBaseExchange.moneyTypeExchange);
            if (numberCurrency < this._dataBaseExchange.costExchange)
            {
                PopupUtils.ShowPopupSuggestBuyResource(this._dataBaseExchange.moneyTypeExchange);
                return;
            }

            UserData.Instance.SubMoney(this._dataBaseExchange.moneyTypeExchange, this._dataBaseExchange.costExchange,
                AnalyticsConstants.SourceTournament,
                this._dataBaseExchange.moneyTypeExchange.ToString());

            HandleChange("exchange_gem");
        }

        private void HandleChange(string sourceId)
        {
            UserData.Instance.TournamentData.remainNumberExchange--;
            Reward.AddToUserData(new Reward[] {this._reward}, AnalyticsConstants.SourceTournament, sourceId);
            PopupUtils.ShowReward(this._reward);
            this.txtRemainNumberExchange.text = $"{L.popup.available_txt}{UserData.Instance.TournamentData.remainNumberExchange}";
            ActiveButton();
        }


        public override void SetLocalization()
        {
            this.txtTitle.text = string.Format(L.popup.insufficient_resource, Ultilities.GetNameCurrency(MoneyType.TournamentTicket));
            this.txtTip.text = $"{string.Format(L.popup.run_out_resource_txt, Ultilities.GetNameCurrency(MoneyType.TournamentTicket))} {L.popup.buy_or_wait_txt}";
            this.txtRemainNumberExchange.text =
                $"{L.popup.available_txt}{UserData.Instance.TournamentData.remainNumberExchange}";
            this.txtCostExchange.text = this._dataBaseExchange.costExchange.ToString();
            this.txtLabelExchangeBtn.text = L.button.btn_buy;
            this.txtLabelWatchBtn.text = L.button.reward_video_claim;
            this.txtCostExchangeDisable.text = this._dataBaseExchange.costExchange.ToString();
            this.txtLabelExchangeBtnDisable.text = L.button.btn_buy;
            this.txtLabelWatchBtnDisable.text = L.button.reward_video_claim;
            var userData = UserData.Instance.TournamentData;
            userData.CheckNewDay();
             this.countdown.SetData(userData.GetTimeRemainResetNewDay(), TimeRemainFormatType.Hhmmss,
                HandleResetTicket);

        }

        private void HandleResetTicket()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_out_of_tournament_ticket);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();


            if (this._gridReward != null)
            {
                this._gridReward.ReturnPool();
                this._gridReward.SetData(new List<Reward>() {this._reward});
            }

            SetLocalization();

            ActiveButton();
        }

        private void ActiveButton()
        {
            var isCanExchange = UserData.Instance.TournamentData.remainNumberExchange > 0;

            this.exchangeBtn.gameObject.SetActive(isCanExchange);
            this.exchangeBtnDisable.gameObject.SetActive(!isCanExchange);
            this.watchAdsBtn.gameObject.SetActive(isCanExchange);
            this.watchAdsBtnDisable.gameObject.SetActive(!isCanExchange);
        }
    }
}