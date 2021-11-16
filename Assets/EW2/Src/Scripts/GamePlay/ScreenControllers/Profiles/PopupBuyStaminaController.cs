using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class BuyStaminaWindowProperties : WindowProperties
    {
        public string title;
        public string lbBuyByGem;
        public string lbBuyByAds;
        public string lbValueBuyByGem;
        public string lbValueBuyByAds;
        public UnityAction buyByGemOnClick;
        public UnityAction buyByAdsOnClick;
        public UnityAction noOnClick;

        public BuyStaminaWindowProperties()
        {
            var shopStaminaDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<ShopStaminaData>();
            string textGem = shopStaminaDataBase.shopStaminaDataBases[0].costExchange + L.currency_type.currency_0;
            string valueStaminaGem= "x" + shopStaminaDataBase.shopStaminaDataBases[0].valueExchangeByMoney.ToString();
            string valueStaminaAds= "x" + shopStaminaDataBase.shopStaminaDataBases[0].valueExchangeByAds.ToString();
            this.title = L.popup.refill_stamina_title_txt;
            this.lbBuyByGem = textGem;
            this.lbBuyByAds = L.button.reward_video_claim;
            this.lbValueBuyByGem = valueStaminaGem;
            this.lbValueBuyByAds = valueStaminaAds;
        }
    }

    public class PopupBuyStaminaController : AWindowController<BuyStaminaWindowProperties>
    {
        public const int INDEX_ADS_DATA = 6;
        [SerializeField] private Text _txtTile;
        [SerializeField] private Text _buyByGemText;
        [SerializeField] private Text _buyByAdsText;
        [SerializeField] private Text _buyByGemTextValue;
        [SerializeField] private Text _buyByAdsTextValue;
        [SerializeField] private Text _AdsRemainText;
        [SerializeField] private TimeCountDownUi _timer;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Image _iconMoney;

        [SerializeField] private Button _btnBuyByGem;
        [SerializeField] private Button _btnBuyByAds;

        ShopStaminaUserData shopStaminaUserData;
        ShopStaminaDataBase shopStaminaDataBase;
        protected override void Awake()
        {
            base.Awake();
            _btnClose.onClick.AddListener(NoOnClick);
            _btnBuyByGem.onClick.AddListener(BuyByGemClick);
            _btnBuyByAds.onClick.AddListener(BuyByAdsClick);
        }
        private void OnEnable()
        {
            VideoAdPlayer.Instance.OnRewarded += RewardedAdSystem;
            VideoAdPlayer.Instance.OnWatchAdFailed += HandleOnPlayAdFailed;
            if (UserData.Instance.OtherUserData.CheckCanResetNewDay())
            {
                UserData.Instance.UserEventData.ShopStaminaUserData.watchAdsCount = 0;
            }
        }

        private void OnDisable()
        {
            VideoAdPlayer.Instance.OnRewarded -= RewardedAdSystem;
            VideoAdPlayer.Instance.OnWatchAdFailed -= HandleOnPlayAdFailed;
        }
        void RewardedAdSystem(string placementName)
        {
            Reward reward = Reward.Create(ResourceType.Money,MoneyType.Stamina,shopStaminaDataBase.valueExchangeByAds);
            PopupUtils.ShowReward(reward);
            reward.AddToUserData();
            RepaintAdsRemainText();
            SetInteractable(true);
        }

        private void SetInteractable(bool interactable)
        {
            RePaintButtonAds();
        }
        void HandleOnPlayAdFailed()
        {
            SetInteractable(true);
        }

        void OnAdClick()
        {
        }
        protected virtual void NoOnClick()
        {
            UIFrame.Instance.CloseCurrentWindow();
            Properties.noOnClick?.Invoke();
        }

        protected virtual void BuyByGemClick()
        {
            int realCost = shopStaminaDataBase.costExchange + shopStaminaDataBase.increaseRate * shopStaminaUserData.stackBuy;
            if (UserData.Instance.GetMoney(shopStaminaDataBase.moneyTypeToBuy) < realCost)
            {
                string content = string.Format(L.popup.insufficient_resource, L.currency_type.currency_0) +" "+L.popup.get_more_txt;
                PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(L.popup.notice_txt,content,PopupNoticeWindowProperties.PopupType.TwoOption,L.button.btn_ok,()=>
                {
                    UIFrame.Instance.CloseCurrentWindow();
                    ShopWindowProperties shopProperties = new ShopWindowProperties((ShopTabId)shopStaminaDataBase.moneyTypeToBuy);
                    UIFrame.Instance.OpenWindow(ScreenIds.shop_scene, shopProperties);
                },L.button.btn_no,
                    null);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                return;
            }
            if (shopStaminaUserData.stackBuy == 0)
            {
                shopStaminaUserData.SetTime();
                if (_timer)
                {
                    _timer.SetData(UserData.Instance.UserEventData.ShopStaminaUserData.TimeRemain(),
                        TimeRemainFormatType.Hhmmss, HandleTimeEnd);
                }
            }
            UserData.Instance.SubMoney(shopStaminaDataBase.moneyTypeToBuy, realCost, AnalyticsConstants.PopupBuyStamina, "");
            Reward reward = Reward.Create(ResourceType.Money,MoneyType.Stamina,shopStaminaDataBase.valueExchangeByMoney);
            PopupUtils.ShowReward(reward);
            reward.AddToUserData();
            shopStaminaUserData.AddStackBuy();
            RepaintGemText();
            Properties.buyByGemOnClick?.Invoke();
        }
        protected virtual void BuyByAdsClick()
        {
            if (shopStaminaUserData.CheckCanWatchAds())
            {
                shopStaminaUserData.BuyByWatchAds();
                OnAdClick();
                Properties.buyByGemOnClick?.Invoke();
                var adRewardData = GameContainer.Instance.GetAdRewardDatabase();
                var addData = adRewardData.ads[INDEX_ADS_DATA];
                if (addData != null)
                {
                    VideoAdPlayer.Instance.PlayAdClick(addData.placementName);
                }
            }
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            if (this._txtTile)
                _txtTile.text = Properties.title;
            _buyByGemText.text = Properties.lbBuyByGem;
            _buyByAdsText.text = Properties.lbBuyByAds;
            _buyByGemTextValue.text = Properties.lbValueBuyByGem;
            _buyByAdsTextValue.text = Properties.lbValueBuyByAds;
            shopStaminaUserData = UserData.Instance.UserEventData.ShopStaminaUserData;
            shopStaminaDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<ShopStaminaData>().shopStaminaDataBases[0];
            _iconMoney.sprite = ResourceUtils.GetIconMoney(shopStaminaDataBase.moneyTypeToBuy);
            RepaintGemText();
            RePaintButtonAds();
            RepaintAdsRemainText();
            if (_timer && shopStaminaUserData.stackBuy != 0)
            {
                _timer.SetData(UserData.Instance.UserEventData.ShopStaminaUserData.TimeRemain(),
                    TimeRemainFormatType.Hhmmss, HandleTimeEnd);
            }
        }

        private void HandleTimeEnd()
        {
            shopStaminaUserData.ResetStackBuy();
            RepaintGemText();
        }

        private void RePaintButtonAds()
        {
            bool available = shopStaminaUserData.CheckCanWatchAds();
            if (available)
            {
                _btnBuyByAds.onClick.RemoveAllListeners();
                _btnBuyByAds.interactable = true;
                _btnBuyByAds.onClick.AddListener(BuyByAdsClick);
                _buyByAdsText.color = Color.white;
            }
            else
            {
                _btnBuyByAds.onClick.RemoveAllListeners();
                _btnBuyByAds.interactable = false;
                _buyByAdsText.color = new Color(0.3333333f, 0.3333333f, 0.3333333f);
            }
        }

        private void RepaintGemText()
        {
            int realCost = shopStaminaDataBase.costExchange + shopStaminaDataBase.increaseRate * shopStaminaUserData.stackBuy;
            _buyByGemText.text = realCost.ToString();
        }
        private void RepaintAdsRemainText()
        {
            _AdsRemainText.text = L.popup.remain_txt + " " + (shopStaminaDataBase.adsCount - shopStaminaUserData.watchAdsCount);
        }
    }
}
