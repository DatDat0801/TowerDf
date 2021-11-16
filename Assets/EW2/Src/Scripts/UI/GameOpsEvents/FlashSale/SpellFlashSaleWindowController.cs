using System.Linq;
using EW2.Tools;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class SpellFlashSaleWindowController : AWindowController
    {
        [SerializeField] private Text title;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Text price;
        [SerializeField] private TimeCountDownUi timer;
        [SerializeField] private Toggle dontShowToday;
        [SerializeField] private RectTransform rewardContainer;
        private SpellFlashSaleData SpellFlashSaleData { get; set; }
        private GridReward _rewards;

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

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
            //Set dont show today
            if (dontShowToday.isOn)
            {
                UserData.Instance.UserEventData.SpellFlashSaleUserData.SetStopAutoShow();
                UserData.Instance.Save();
            }
        }

        private void OnBuyClick()
        {
            var canBuy =
                !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(this.SpellFlashSaleData.shopItemDatas[0].productId);
            if (canBuy)
            {
                ShopService.Instance.BuyPack(this.SpellFlashSaleData.shopItemDatas[0], OnBuySuccess);
            }
        }

        private void OnBuySuccess(bool result, Reward[] gifts)
        {
            if (result)
            {
                _giftReceived = gifts;
                _isPurchased = true;

                //PopupUtils.ShowReward(gifts);
                UpdateBuyButtonState();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
                UIFrame.Instance.CloseCurrentWindow();
            }
        }

        private void UpdateBuyButtonState()
        {
            var canBuyPack1 =
                !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(this.SpellFlashSaleData.shopItemDatas[0].productId);
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
            var packData = GameContainer.Instance.Get<ShopDataBase>().Get<SpellFlashSaleData>();
            this.SpellFlashSaleData = packData;
            RepaintText();
            RepaintReward();
        }

        void RepaintReward()
        {
            this.rewardContainer.DestroyAllChildren();
            if (_rewards == null)
                _rewards = new GridReward(rewardContainer);
            var currencyReward = this.SpellFlashSaleData.shopItemDatas[0].rewards;
            _rewards?.SetData(currencyReward.ToArray());
        }

        void RepaintText()
        {
            title.text = L.shop.spell_flash_sale_title.ToUpper();

            var spellFlashSalePrice = ProductsManager.GetLocalPriceStringById(this.SpellFlashSaleData.shopItemDatas[0].productId);
            if (spellFlashSalePrice.Equals("$0.01") || string.IsNullOrEmpty(spellFlashSalePrice))
            {
                spellFlashSalePrice = $"${this.SpellFlashSaleData.shopItemDatas[0].price.ToString()}";
            }

            price.text = spellFlashSalePrice;
            if (timer)
            {
                timer.SetTitle(L.popup.end_time_txt);
                timer.SetData(UserData.Instance.UserEventData.SpellFlashSaleUserData.TimeRemain(),
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