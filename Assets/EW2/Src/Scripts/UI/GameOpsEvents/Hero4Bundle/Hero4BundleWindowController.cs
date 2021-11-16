using System;
using System.Linq;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class Hero4BundleWindowController : AWindowController
    {
        [SerializeField] private Text title;
        [SerializeField] private Text heroName;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Text slogan;
        [SerializeField] private Text price;
        [SerializeField] private TimeCountDownUi timer;

        [SerializeField] private RectTransform rewardContainer;
        [SerializeField] private RewardUI heroRewardUI;
        [SerializeField] private Text profitText;
        [SerializeField] private Text profitTag;

        private GridReward _rewards;

        private Hero4BundleData Hero4Bundle { get; set; }
        private bool _isPurchased;
        private Reward[] _giftReceived;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            buyButton.onClick.AddListener(OnBuyClick);

            OutTransitionFinished += controller => {
                this._rewards.ReturnPool();
                if (_isPurchased && _giftReceived != null)
                {
                    PopupUtils.ShowReward(_giftReceived);
                }
            };
        }


        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            var packData = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4BundleData>();
            Hero4Bundle = packData;

            RepaintReward();
            RepaintText();
        }

        private void OnBuyClick()
        {
            var canBuy =
                !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(Hero4Bundle.shopItemDatas[0].productId);
            if (canBuy)
            {
                ShopService.Instance.BuyPack(Hero4Bundle.shopItemDatas[0], OnBuyPack1Success);
            }
        }

        private void OnBuyPack1Success(bool result, Reward[] gifts)
        {
            if (result)
            {
                _giftReceived = gifts;
                _isPurchased = true;


                // PopupUtils.ShowReward(gifts);
                UpdateBuyButtonState();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
                UIFrame.Instance.CloseCurrentWindow();
                //UIFrame.Instance.CloseWindow(ScreenIds.hero_4_bundle);
            }
        }

        private void UpdateBuyButtonState()
        {
            var canBuyPack1 =
                !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(Hero4Bundle.shopItemDatas[0].productId);
            if (!canBuyPack1)
            {
                price.text = L.popup.purchased_txt;
                buyButton.interactable = false;
            }
            else
            {
                buyButton.interactable = true;
            }
        }

        void RepaintReward()
        {
            if (this._rewards == null)
                this._rewards = new GridReward(rewardContainer);
            var currencyReward = Hero4Bundle.shopItemDatas[0].rewards.Where(reward => reward.type != ResourceType.Hero);
            this._rewards?.SetData(currencyReward.ToArray());

            try
            {
                var hero = Array.Find(Hero4Bundle.shopItemDatas[0].rewards, reward => reward.type == ResourceType.Hero);
                heroRewardUI.SetData(hero);
            }
            catch (Exception e)
            {
                Debug.LogError("Must have a hero in the database");
            }
        }

        void RepaintText()
        {
            title.text = L.shop.hero_4_bundle.ToUpper();
            heroName.text = L.heroes.hero_name_1004;
            slogan.text = L.shop.hero_4_bundle_txt;


            var hero4BundlePrice = ProductsManager.GetLocalPriceStringById(Hero4Bundle.shopItemDatas[0].productId);
            if (hero4BundlePrice.Equals("$0.01") || string.IsNullOrEmpty(hero4BundlePrice))
            {
                hero4BundlePrice = $"${Hero4Bundle.shopItemDatas[0].price}";
            }

            price.text = hero4BundlePrice;
            if (timer)
            {
                timer.SetTitle(L.popup.end_time_txt);
                timer.SetData(UserData.Instance.UserEventData.Hero4BundleUserData.TimeRemain(),
                    TimeRemainFormatType.Hhmmss, HandleTimeEnd);
            }

            this.profitText.text = string.Format(L.shop.profit_title);
            this.profitTag.text = $"{Hero4Bundle.GetProfit().ToString()}%";
        }

        private void HandleTimeEnd()
        {
            EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            UIFrame.Instance.CloseCurrentWindow();
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }
    }
}