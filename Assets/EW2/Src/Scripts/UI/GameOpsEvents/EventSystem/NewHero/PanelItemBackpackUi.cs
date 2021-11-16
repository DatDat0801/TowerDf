using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EW2
{
    public class PanelItemBackpackUi : MonoBehaviour
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtLimit;
        [SerializeField] private Transform parentRewardBonus;
        [SerializeField] private Button btnBuyPack;
        [SerializeField] private Button btnBuyPackDisable;

        private ShopLitmitedItemData _datasBundle;
        private GridReward _packGridReward;
        private int _indexData;
        private int _numbRemainPurchase;

        private void Awake()
        {
            this.btnBuyPack.onClick.AddListener(BuyClick);
            this.btnBuyPackDisable.onClick.AddListener(ButtonDisableClick);

            this._packGridReward = new GridReward(this.parentRewardBonus);
        }

        private void ButtonDisableClick()
        {
            Ultilities.ShowToastNoti(L.popup.already_own_this_pack_txt);
        }

        private void BuyClick()
        {
            // var canBuy = !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(this._datasBundle.productId);
            // if (canBuy)
            // {
                ShopService.Instance.BuyLimitedBundle(this._datasBundle, OnBuySuccess);
            // }
        }

        private void OnBuySuccess(bool success, Reward[] gifts)
        {
            if (success)
            {
                PopupUtils.ShowReward(gifts);
                var userData = UserData.Instance.UserEventData.NewHeroEventUserData;

                if (this._indexData == 0)
                {
                    userData.numberBuyCrystalPack++;
                    _numbRemainPurchase = this._datasBundle.limit - userData.numberBuyCrystalPack;
                }
                else if (this._indexData == 1)
                {
                    userData.numberBuyRunePack++;
                    _numbRemainPurchase = this._datasBundle.limit - userData.numberBuyRunePack;
                }
                else if (this._indexData == 2)
                {
                    userData.numberBuySpellPack++;
                    _numbRemainPurchase = this._datasBundle.limit - userData.numberBuySpellPack;
                }

                ShowUiUpdate();
            }
        }

        public void InitData(int indexData, ShopLitmitedItemData dataBundle)
        {
            this._indexData = indexData;
            this._datasBundle = dataBundle;

            var numbPurchased = 0;
            var userData = UserData.Instance.UserEventData.NewHeroEventUserData;

            if (this._indexData == 0)
            {
                numbPurchased = userData.numberBuyCrystalPack;
            }
            else if (this._indexData == 1)
            {
                numbPurchased = userData.numberBuyRunePack;
            }
            else if (this._indexData == 2)
            {
                numbPurchased = userData.numberBuySpellPack;
            }

            _numbRemainPurchase = this._datasBundle.limit - numbPurchased;

            ShowUi();
        }

        private void ShowUi()
        {
            ShowLocalize();
            ShowUiUpdate();
        }

        private void ShowLocalize()
        {
            var title = "";
            var userData = UserData.Instance.UserEventData.NewHeroEventUserData;

            if (this._indexData == 0)
            {
                title = L.game_event.event_crystal_pack_txt;
            }
            else if (this._indexData == 1)
            {
                title = L.game_event.event_rune_pack_txt;
            }
            else if (this._indexData == 2)
            {
                title = L.game_event.event_spell_pack_txt;
            }

            this.txtTitle.text = title;

            var packPrice = ProductsManager.GetLocalPriceStringById(this._datasBundle.productId);
            if (packPrice.Equals("$0.01") || string.IsNullOrEmpty(packPrice))
            {
                packPrice = $"${this._datasBundle.price}";
            }

            this.btnBuyPack.GetComponentInChildren<Text>().text = packPrice;
            this.btnBuyPackDisable.GetComponentInChildren<Text>().text = packPrice;
        }

        private void ShowUiUpdate()
        {
            if (_numbRemainPurchase > 0)
            {
                this.txtLimit.text = $"{L.popup.limit_txt} {_numbRemainPurchase}/{this._datasBundle.limit}";
            }
            else
            {
                _numbRemainPurchase = 0;
                this.txtLimit.text =
                    $"{L.popup.limit_txt} <color='#ff2730'>{_numbRemainPurchase}/{this._datasBundle.limit}</color>";
            }

            this.btnBuyPack.gameObject.SetActive(_numbRemainPurchase > 0);
            this.btnBuyPackDisable.gameObject.SetActive(_numbRemainPurchase <= 0);

            this._packGridReward.ReturnPool();
            this._packGridReward.SetData(this._datasBundle.rewards);
        }
    }
}