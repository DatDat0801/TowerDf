using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class Hero4ResourceFlashSaleWindowController : AWindowController
    {
        [SerializeField] private Text title;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Text slogan;
        [SerializeField] private Text price;
        [SerializeField] private TimeCountDownUi timer;
        [SerializeField] private Text hotDealText;
        [SerializeField] private Toggle dontShowToday;
        
        private Hero4ResourceFlashSaleData FlashSaleData { get; set; }
        
        private bool _isPurchased;
        private Reward[] _giftReceived;
        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            buyButton.onClick.AddListener(OnBuyClick);
            OutTransitionFinished += controller => {
                //this._rewards.ReturnPool();

                if (_isPurchased && _giftReceived != null)
                {
                    PopupUtils.ShowReward(_giftReceived);
                }
            };  
        }
        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
            //Set dont show today
            if (dontShowToday.isOn)
            {
                UserData.Instance.UserEventData.CrystalFlashSaleUserData.SetStopAutoShow();
                UserData.Instance.Save();
            }
        }
        private void OnBuyClick()
        {
            var canBuy =
                !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(FlashSaleData.shopItemDatas[0].productId);
            if (canBuy)
            {
                ShopService.Instance.BuyPack(FlashSaleData.shopItemDatas[0], OnBuySuccess);
            }
        }
        private void OnBuySuccess(bool result, Reward[] gifts)
        {
            if (result)
            {
                _giftReceived = gifts;
                _isPurchased = true;

               // PopupUtils.ShowReward(gifts);
                UpdateBuyButtonState();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
                UIFrame.Instance.CloseCurrentWindow();
                //UIFrame.Instance.CloseWindow(ScreenIds.hero_4_resource_flash_sale);
            }
        }
        private void UpdateBuyButtonState()
        {
            var canBuyPack1 =
                !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(FlashSaleData.shopItemDatas[0].productId);
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
        
        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            var packData = GameContainer.Instance.Get<ShopDataBase>().Get<Hero4ResourceFlashSaleData>();
            FlashSaleData = packData;
            RepaintText();
        }
        void RepaintText()
        {
            title.text = L.shop.hero_resource_title.ToUpper();
            slogan.text = string.Format(L.shop.hero_resource_slogan.ToUpper(), FlashSaleData.shopItemDatas[0].rewards[0].number);
            hotDealText.text = L.shop.hot_deal_2_txt.ToUpper();

            var hero4BundlePrice = ProductsManager.GetLocalPriceStringById(FlashSaleData.shopItemDatas[0].productId);
            if (hero4BundlePrice.Equals("$0.01") || string.IsNullOrEmpty(hero4BundlePrice))
            {
                hero4BundlePrice = $"${FlashSaleData.shopItemDatas[0].price}";
            }

            price.text = hero4BundlePrice;
            if (timer)
            {
                timer.SetTitle(L.popup.end_time_txt);
                timer.SetData(UserData.Instance.UserEventData.CrystalFlashSaleUserData.TimeRemain(),
                    TimeRemainFormatType.Hhmmss, HandleTimeEnd);
            }
        }
        private void HandleTimeEnd()
        {
            EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            UIFrame.Instance.CloseCurrentWindow();
        }
    }
}