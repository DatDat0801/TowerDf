using System.Linq;
using EW2;
using EW2.Tools;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

public class RuneFlashSaleWindowController : AWindowController
{
    [SerializeField] private Text title;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text price;
    [SerializeField] private TimeCountDownUi timer;
    [SerializeField] private Toggle dontShowToday;
    [SerializeField] private RectTransform rewardContainer;
    private RuneFlashSaleData FlashSaleData { get; set; }
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
            UserData.Instance.UserEventData.RuneFlashSaleUserData.SetStopAutoShow();
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

            //PopupUtils.ShowReward(gifts);
            UpdateBuyButtonState();
            EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            UIFrame.Instance.CloseCurrentWindow();
            //UIFrame.Instance.CloseWindow(ScreenIds.rune_flash_sale);
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
        var packData = GameContainer.Instance.Get<ShopDataBase>().Get<RuneFlashSaleData>();
        FlashSaleData = packData;
        RepaintText();
        RepaintReward();
    }

    void RepaintReward()
    {
        this.rewardContainer.DestroyAllChildren();
        if (_rewards == null)
            _rewards = new GridReward(rewardContainer);
        var currencyReward = FlashSaleData.shopItemDatas[0].rewards;
        _rewards?.SetData(currencyReward.ToArray());
    }

    void RepaintText()
    {
        title.text = L.shop.rune_flash_sale_title.ToUpper();

        var runeFlashSalePrice = ProductsManager.GetLocalPriceStringById(FlashSaleData.shopItemDatas[0].productId);
        if (runeFlashSalePrice.Equals("$0.01") || string.IsNullOrEmpty(runeFlashSalePrice))
        {
            runeFlashSalePrice = $"${FlashSaleData.shopItemDatas[0].price.ToString()}";
        }

        price.text = runeFlashSalePrice;
        if (timer)
        {
            timer.SetTitle(L.popup.end_time_txt);
            timer.SetData(UserData.Instance.UserEventData.RuneFlashSaleUserData.TimeRemain(),
                TimeRemainFormatType.Hhmmss, HandleTimeEnd);
        }
    }
    private void HandleTimeEnd()
    {
        EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
        UIFrame.Instance.CloseCurrentWindow();
    }
}