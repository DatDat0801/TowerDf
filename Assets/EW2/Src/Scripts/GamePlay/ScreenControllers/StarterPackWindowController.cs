using System;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class StarterPackWindowController : AWindowController
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private TimeRemainUi timeRemain;
        [SerializeField] private Text txtTip;
        [SerializeField] private Text txtSale;
        [SerializeField] private Text txtSalePercent;
        [SerializeField] private Text txtPrice;
        [SerializeField] private Text txtTapToClose;
        [SerializeField] private Transform grRewards;
        [SerializeField] private GameObject panelSale;
        [SerializeField] private Button buyButton;

        private StarterPackData packData;
        private ShopItemData itemData;
        private GridReward gridReward;
        private bool isPurchased;


        protected override void Awake()
        {
            base.Awake();

            buyButton.onClick.AddListener(BuyOnClick);

            OutTransitionFinished += controller =>
            {
                gridReward.ReturnPool();

                if (isPurchased)
                {
                    PopupUtils.ShowReward(itemData.rewards);
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
            txtTitle.text = L.shop.starter_pack_name.ToUpper();
            txtTip.text = string.Format(L.shop.starter_pack_des, L.heroes.hero_name_1003);
            txtTapToClose.text = L.popup.tap_to_close;
            txtSale.text = L.shop.sale_txt;

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
                    if (itemData.pricePrevious <= 0f || Math.Abs(itemData.pricePrevious - itemData.price) <= 0)
                    {
                        panelSale.SetActive(false);
                    }
                    else
                    {
                        var percentSale = itemData.price / itemData.pricePrevious;
                        txtSalePercent.text = $"{(percentSale * 100):n0}%";
                        panelSale.SetActive(true);
                    }
                }

                Reward[] rewardShows = new Reward[itemData.rewards.Length - 1];
                for (int i = 0; i < rewardShows.Length; i++)
                {
                    rewardShows[i] = itemData.rewards[i];
                }

                gridReward?.SetData(rewardShows);

                if (timeRemain)
                    timeRemain.SetTimeRemain(UserData.Instance.UserEventData.StarterPackUserData.TimeRemain());
            }
        }

        private void GetData()
        {
            packData = GameContainer.Instance.Get<ShopDataBase>().Get<StarterPackData>();
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

        private void HandleBuySucess(bool result, Reward[] gifts)
        {
            if (result)
            {
                isPurchased = true;
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
                UIFrame.Instance.CloseCurrentWindow();
            }
        }
    }
}