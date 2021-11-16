using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class BuyNowWindowController : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private TimeCountDownUi timeRemain;
        [SerializeField] private Text txtTip;
        [SerializeField] private Text txtBenifit;
        [SerializeField] private Text txtTitleBattlePass;
        [SerializeField] private Text txtSale;
        [SerializeField] private Text txtSalePercent;
        [SerializeField] private Text txtPrice;
        [SerializeField] private Button btnClose;
        [SerializeField] private Transform grRewards;
        [SerializeField] private GameObject panelSale;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button btnGoTo1StPurchase;
        [SerializeField] private Button btnGoToGloryRoad;


        private BuyNowData packData;
        private ShopItemData itemData;
        private GridReward gridReward;
        private bool isPurchased;
        private Reward[] giftReceived;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(() => { UIFrame.Instance.CloseWindow(ScreenIds.popup_buy_now); });

            buyButton.onClick.AddListener(BuyOnClick);

            this.btnGoTo1StPurchase.onClick.AddListener(GoToFirstPurachase);

            btnGoToGloryRoad.onClick.AddListener(GoToGloryRoad);

            OutTransitionFinished += controller => {
                gridReward.ReturnPool();

                if (isPurchased && giftReceived != null)
                {
                    PopupUtils.ShowReward(giftReceived);
                }
            };
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            if (gridReward == null)
                gridReward = new GridReward(grRewards);

            GetData();

            ShowUi();
        }

        private void ShowUi()
        {
            txtTitle.text = L.popup.buy_now_title_txt.ToUpper();
            txtTip.text = L.popup.buy_now_slogan_title_txt;
            txtBenifit.text = L.popup.buy_now_1st_purchase_txt;
            txtSale.text = L.shop.profit_title;
            txtTitleBattlePass.text = L.game_event.premium_glory_road_item_txt;
            if (packData != null)
            {
                if (txtPrice)
                {
                    var price = ProductsManager.GetLocalPriceStringById(itemData.productId);


                    if (price.Equals("$0.01") || string.IsNullOrEmpty(price))
                    {
                        price = $"${itemData.price}";
                    }

                    txtPrice.text = price;
                }

                if (panelSale)
                {
                    txtSalePercent.text = $"{packData.packConditions[0].percentProfit}%";
                    panelSale.SetActive(true);
                }

                Reward[] rewardShows = new Reward[itemData.rewards.Length];
                for (int i = 0; i < rewardShows.Length; i++)
                {
                    rewardShows[i] = itemData.rewards[i];
                }

                gridReward?.SetData(rewardShows);

                if (timeRemain)
                {
                    timeRemain.SetTitle(L.popup.end_time_txt);
                    timeRemain.SetData(UserData.Instance.UserEventData.BuyNowUserData.TimeRemain(), default,
                        HandleTimeEnd);
                }
            }
        }

        private void HandleTimeEnd()
        {
            EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            UIFrame.Instance.CloseCurrentWindow();
        }

        private void GetData()
        {
            packData = GameContainer.Instance.Get<ShopDataBase>().Get<BuyNowData>();
            if (packData != null)
                itemData = packData.shopItemDatas[0];
        }

        private void BuyOnClick()
        {
            var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(itemData.productId);

            if (canBuy)
            {
                ShopService.Instance.BuyPack(itemData, HandleBuySucess);
            }
        }

        private void GoToFirstPurachase()
        {
            var userData = UserData.Instance.UserEventData.FirstPurchase;
            if (userData.IsClaimed)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.already_receive_txt);
            }
            else
            {
                UIFrame.Instance.CloseWindow(ScreenIds.popup_buy_now);
                UIFrame.Instance.OpenWindow(ScreenIds.first_purchase_popup);
            }
        }

        private void HandleBuySucess(bool result, Reward[] gifts)
        {
            if (result)
            {
                giftReceived = gifts;
                isPurchased = true;
                var premiumKey = UserData.Instance.AccountData.tokenId;
                UserData.Instance.UserEventData.GloryRoadUser.SetPremiumKey($"{premiumKey}{ShortId.Generate(10)}");
                UserData.Instance.Save();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
                EventManager.EmitEvent(GamePlayEvent.OnRepaintGloryRoad);
                UIFrame.Instance.CloseCurrentWindow();
            }
        }

        private void GoToGloryRoad()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_buy_now);
            DirectionGoTo.GoToGloryRoad();
            FirebaseLogic.Instance.ButtonClick("banner", "premium_glory_road", 0);
        }
    }
}
