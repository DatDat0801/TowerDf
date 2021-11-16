using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class RunePackageWindowController : AWindowController
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
        //[SerializeField] private TimeCountDownUi timer;
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

            OutTransitionFinished += controller =>
            {
                pack1GridReward.ReturnPool();
                pack2GridReward.ReturnPool();
                pack3GridReward.ReturnPool();
                // if (isPurchased && giftReceived != null)
                // {
                //     PopupUtils.ShowReward(giftReceived);
                // }
            };
        }

        // private void OnEnable()
        // {
        //     EventManager.StartListening(GamePlayEvent.OnUpdateSaleBundle, OnCloseCurrentWindow);
        // }
        //
        // private void OnDisable()
        // {
        //     EventManager.StopListening(GamePlayEvent.OnUpdateSaleBundle, OnCloseCurrentWindow);
        // }

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
            titleText.text = L.popup.rune_pack_name_txt;
        }
        
        void Initialize()
        {
            //Package 1
            if (pack1GridReward == null)
                pack1GridReward = new GridReward(pack1Container);
            var pack1Price = ProductsManager.GetLocalPriceStringById(packages[0].productId);
            if (pack1Price.Equals("$0.01") || string.IsNullOrEmpty(pack1Price))
            {
                pack1Price = $"${packages[0].price.ToString()}";
            }

            package1Price.text = pack1Price;
            Reward[] pack1ToShow = new Reward[packages[0].rewards.Length];
            for (int i = 0; i < pack1ToShow.Length; i++)
            {
                pack1ToShow[i] = packages[0].rewards[i];
            }

            pack1GridReward?.SetData(pack1ToShow);
            //Package 2 
            if (pack2GridReward == null)
                pack2GridReward = new GridReward(pack2Container);
            var pack2Price = ProductsManager.GetLocalPriceStringById(packages[1].productId);
            if (pack2Price.Equals("$0.01") || string.IsNullOrEmpty(pack2Price))
            {
                pack2Price = $"${packages[1].price.ToString()}";
            }

            package2Price.text = pack2Price;
            Reward[] pack2ToShow = new Reward[packages[1].rewards.Length];
            for (int i = 0; i < pack2ToShow.Length; i++)
            {
                pack2ToShow[i] = packages[1].rewards[i];
            }

            pack2GridReward?.SetData(pack2ToShow);
            //Package 3
            if (pack3GridReward == null)
                pack3GridReward = new GridReward(pack3Container);
            var pack3Price = ProductsManager.GetLocalPriceStringById(packages[2].productId);
            if (pack3Price.Equals("$0.01") || string.IsNullOrEmpty(pack3Price))
            {
                pack3Price = $"${packages[2].price.ToString()}";
            }

            package3Price.text = pack3Price;
            Reward[] pack3ToShow = new Reward[packages[2].rewards.Length];
            for (int i = 0; i < pack3ToShow.Length; i++)
            {
                pack3ToShow[i] = packages[2].rewards[i];
            }

            pack3GridReward?.SetData(pack3ToShow);

            //Timer
            // if (timer)
            // {
            //     timer.SetTitle(L.popup.end_time_txt);
            //     timer.SetData(UserData.Instance.UserEventData.RunePackageUserData.TimeRemain(), HandleOnTimerEnd);
            // }
        }

        private void UpdateBuyButtonState()
        {
            var canBuyPack1 = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(packages[0].productId);
            if (!canBuyPack1)
            {
                package1Price.text = L.popup.purchased_txt;
                buyPack1Button.interactable = false;
            }
            else
            {
                buyPack1Button.interactable = true;
            }
            var canBuyPack2 = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(packages[1].productId);
            if (!canBuyPack2)
            {
                package2Price.text = L.popup.purchased_txt;
                buyPack2Button.interactable = false;
            }
            else
            {
                buyPack2Button.interactable = true;
            }
            var canBuyPack3 = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(packages[2].productId);
            if (!canBuyPack3)
            {
                package3Price.text = L.popup.purchased_txt;
                buyPack3Button.interactable = false;
            }
            else
            {
                buyPack3Button.interactable = true;
            }

        }
        private void HandleOnTimerEnd()
        {
            EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            UIFrame.Instance.CloseCurrentWindow();
        }

        void SetShopData()
        {
            var packData = GameContainer.Instance.Get<ShopDataBase>().Get<RunePackageShopData>();
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
                //UIFrame.Instance.CloseCurrentWindow();
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
                //UIFrame.Instance.CloseCurrentWindow();
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
                //UIFrame.Instance.CloseCurrentWindow();
            }
        }

        // void OnCloseCurrentWindow()
        // {
        //     if (!UserData.Instance.UserEventData.RunePackageUserData.CheckCanShow())
        //     {
        //         UIFrame.Instance.CloseWindow(ScreenIds.rune_package);
        //     }
        // }
    }
}