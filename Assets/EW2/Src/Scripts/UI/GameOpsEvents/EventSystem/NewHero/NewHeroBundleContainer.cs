using System;
using System.Collections;
using System.Collections.Generic;
using EW2;
using EW2.Events;
using UnityEngine;
using UnityEngine.UI;

public class NewHeroBundleContainer : TabContainer
{
    [SerializeField] private Text txtTip;
    [SerializeField] private Text txtLevel;
    [SerializeField] private Transform parentRewardBonus;
    [SerializeField] private Button btnBuyPack1;
    [SerializeField] private Button btnBuyPack1Disable;
    [SerializeField] private Button btnBuyPack2;
    [SerializeField] private Button btnBuyPack2Disable;

    private List<ShopLitmitedItemData> _datasBundle;
    private GridReward _pack2GridReward;

    private void Awake()
    {
        this.btnBuyPack1.onClick.AddListener(() => BuyClick(0));
        this.btnBuyPack1Disable.onClick.AddListener(ButtonDisableClick);
        this.btnBuyPack2.onClick.AddListener(() => BuyClick(1));
        this.btnBuyPack2Disable.onClick.AddListener(ButtonDisableClick);
        this._pack2GridReward = new GridReward(this.parentRewardBonus);
    }

    private void ButtonDisableClick()
    {
        Ultilities.ShowToastNoti(L.popup.already_own_this_hero_txt);
    }

    private void BuyClick(int indexData)
    {
        var bundleData = this._datasBundle[indexData];
        var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(bundleData.productId);
        if (canBuy)
        {
            ShopService.Instance.BuyLimitedBundle(bundleData, OnBuySuccess);
        }
    }

    private void OnBuySuccess(bool success, Reward[] gifts)
    {
        if (success)
        {
            PopupUtils.ShowReward(gifts);
            RefreshUiAfterPurchase();
        }
    }

    public override void ShowContainer()
    {
        GetDataBundle();
        gameObject.SetActive(true);
        ShowUi();
    }

    public override void HideContainer()
    {
        gameObject.SetActive(false);
    }

    private void GetDataBundle()
    {
        if (this._datasBundle == null)
        {
            this._datasBundle = new List<ShopLitmitedItemData>();
            var database = GameContainer.Instance.Get<EventDatabase>().Get<NewHeroEventBundle>();
            if (database)
            {
                for (int i = 0; i < 2; i++)
                {
                    this._datasBundle.Add(database.shopLitmitItemDatas[i]);
                }
            }
        }
    }

    private void ShowUi()
    {
        this.txtTip.text = L.game_event.hero_bundle_slogan_txt.ToUpper();
        this.txtLevel.text = string.Format(L.game_event.hero_bundle_level_tag_txt, 15).ToUpper();
        var isOwnedHero = UserData.Instance.UserHeroData.CheckHeroUnlocked((int)HeroType.NeroCat);

        var pack1Price = ProductsManager.GetLocalPriceStringById(this._datasBundle[0].productId);
        if (pack1Price.Equals("$0.01") || string.IsNullOrEmpty(pack1Price))
        {
            pack1Price = $"${this._datasBundle[0].price}";
        }

        this.btnBuyPack1.GetComponentInChildren<Text>().text = pack1Price;
        this.btnBuyPack1Disable.GetComponentInChildren<Text>().text = pack1Price;

        var pack2Price = ProductsManager.GetLocalPriceStringById(this._datasBundle[1].productId);
        if (pack2Price.Equals("$0.01") || string.IsNullOrEmpty(pack2Price))
        {
            pack2Price = $"${this._datasBundle[1].price}";
        }

        this.btnBuyPack2.GetComponentInChildren<Text>().text = pack2Price;
        this.btnBuyPack2Disable.GetComponentInChildren<Text>().text = pack2Price;

        this.btnBuyPack1.gameObject.SetActive(!isOwnedHero);
        this.btnBuyPack1Disable.gameObject.SetActive(isOwnedHero);
        this.btnBuyPack2.gameObject.SetActive(!isOwnedHero);
        this.btnBuyPack2Disable.gameObject.SetActive(isOwnedHero);

        this._pack2GridReward.ReturnPool();
        this._pack2GridReward.SetData(this._datasBundle[1].rewards);
    }

    private void RefreshUiAfterPurchase()
    {
        var isOwnedHero = UserData.Instance.UserHeroData.CheckHeroUnlocked((int)HeroType.NeroCat);
        this.btnBuyPack1.gameObject.SetActive(!isOwnedHero);
        this.btnBuyPack1Disable.gameObject.SetActive(isOwnedHero);
        this.btnBuyPack2.gameObject.SetActive(!isOwnedHero);
        this.btnBuyPack2Disable.gameObject.SetActive(isOwnedHero);
    }
}