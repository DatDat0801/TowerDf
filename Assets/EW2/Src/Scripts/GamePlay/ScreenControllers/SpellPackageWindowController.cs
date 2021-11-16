using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class SpellPackageWindowController : AWindowController
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button buyPack1Button;
        [SerializeField] private Button buyPack2Button;
        [SerializeField] private Button buyPack3Button;
        [SerializeField] private RectTransform pack1Container;
        [SerializeField] private RectTransform pack2Container;
        [SerializeField] private RectTransform pack3Container;
        [SerializeField] private Text package1Price;
        [SerializeField] private Text package2Price;
        [SerializeField] private Text package3Price;
        [SerializeField] private Text titleText;


        private ShopItemData[] packages;
        private GridReward pack1GridReward;
        private GridReward pack2GridReward;
        private GridReward pack3GridReward;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(CloseClick);
            buyPack1Button.onClick.AddListener(OnPack1Click);
            buyPack2Button.onClick.AddListener(OnPack2Click);
            buyPack3Button.onClick.AddListener(OnPack3Click);

            OutTransitionFinished += ReturnPool;
        }
        private void ReturnPool(IUIScreenController controller)
        {
            pack1GridReward.ReturnPool();
            pack2GridReward.ReturnPool();
            pack3GridReward.ReturnPool();
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            SetShopData();
            Initialize();
            UpdateBuyButtonState();
            titleText.text = L.shop.spell_pack_name_txt;
        }

        void Initialize()
        {
            PrepareReward(packages[0], ref pack1GridReward, package1Price, pack1Container);
            PrepareReward(packages[1], ref pack2GridReward, package2Price, pack2Container);
            PrepareReward(packages[2], ref pack3GridReward, package3Price, pack3Container);
        }


        private void PrepareReward(ShopItemData itemData, ref GridReward gridReward, Text textPrice, RectTransform transform)
        {
            if (gridReward == null)
                gridReward = new GridReward(transform);
            var packPrice = ProductsManager.GetLocalPriceStringById(itemData.productId);
            if (packPrice.Equals("$0.01") || string.IsNullOrEmpty(packPrice))
            {
                packPrice = $"${itemData.price}";
            }

            textPrice.text = packPrice;
            Reward[] packToShow = new Reward[itemData.rewards.Length];
            for (int i = 0; i < packToShow.Length; i++)
            {
                packToShow[i] = itemData.rewards[i];
            }

            gridReward?.SetData(packToShow);
        }
        private void UpdateBuyButtonState()
        {
            CheckBuyCondition(packages[0], buyPack1Button, package1Price);
            CheckBuyCondition(packages[1], buyPack2Button, package2Price);
            CheckBuyCondition(packages[2], buyPack3Button, package3Price);

        }
        private void CheckBuyCondition(ShopItemData shopItem, Button buttonBuy, Text packagePrice)
        {
            var canBuyPack = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(shopItem.productId);
            if (!canBuyPack)
            {
                packagePrice.text = L.popup.purchased_txt;
                buttonBuy.interactable = false;
            }
            else
            {
                buttonBuy.interactable = true;
            }
        }
        private void HandleOnTimerEnd()
        {
            EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            UIFrame.Instance.CloseCurrentWindow();
        }

        void SetShopData()
        {
            var packData = GameContainer.Instance.Get<ShopDataBase>().Get<SpellpackageData>();
            if (packData != null)
                packages = packData.shopItemDatas;
        }

        void OnPack1Click()
        {
            var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(packages[0].productId);
            if (canBuy)
            {
                ShopService.Instance.BuyPack(packages[0], OnBuyPack1Success);
            }
        }

        void OnBuyPack1Success(bool result, Reward[] gifts)
        {
            if (result)
            {
                PopupUtils.ShowReward(gifts);
                UpdateBuyButtonState();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            }
        }

        void OnPack2Click()
        {
            var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(packages[1].productId);
            if (canBuy)
            {
                ShopService.Instance.BuyPack(packages[1], OnBuyPack2Success);
            }
        }

        void OnBuyPack2Success(bool result, Reward[] gifts)
        {
            if (result)
            {
                PopupUtils.ShowReward(gifts);
                UpdateBuyButtonState();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            }
        }

        void OnPack3Click()
        {
            var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(packages[2].productId);
            if (canBuy)
            {
                ShopService.Instance.BuyPack(packages[2], OnBuyPack3Success);
            }
        }

        void OnBuyPack3Success(bool result, Reward[] gifts)
        {
            if (result)
            {
                PopupUtils.ShowReward(gifts);
                UpdateBuyButtonState();
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            }
        }
    }
}
