using System;
using Coffee.UIEffects;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class ShopDefensiveBuffItemUi : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text desText;
        [SerializeField] private Text priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Text purchasedText;
        [SerializeField] private GameObject currencyGameObject;

        [SerializeField] private int defensivePointId;
        private Action<int> _onInsufficientGem;

        private void Awake()
        {
            this.buyButton.onClick.AddListener(OnBuyClick);
        }

        public void Repaint(Action<int> onInsufficient = null)
        {
            this._onInsufficientGem = onInsufficient;
            if (this.titleText)
            {
                this.titleText.text =
                    Localization.Current.Get("playable_mode", $"defensive_point_name_{defensivePointId.ToString()}");
            }

            if (this.desText)
            {
                this.desText.text = Localization.Current.Get("playable_mode", $"defensive_point_skill_{defensivePointId}");
            }

            var shopBuffData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopDFSPData>();

            if (this.priceText)
            {
                this.priceText.text = $"{shopBuffData.GetShopDFSPItemData(defensivePointId).price.ToString()}";
            }

            bool unlocked = UserData.Instance.UserHeroDefenseData.CheckDefensePointUnlocked(this.defensivePointId);
            if (unlocked)
            {
                this.currencyGameObject.SetActive(false);
                this.purchasedText.gameObject.SetActive(true);

                if (shopBuffData.GetShopDFSPItemData(defensivePointId).price > 0)
                {
                    this.purchasedText.text = L.popup.purchased_txt;
                }
                else
                {
                    this.purchasedText.text = L.popup.owned_txt;
                }

                GetComponent<UIEffect>().enabled = true;
                this.buyButton.interactable = false;
            }
            else
            {
                this.currencyGameObject.SetActive(true);
                this.purchasedText.gameObject.SetActive(false);
                GetComponent<UIEffect>().enabled = false;
                this.buyButton.interactable = true;
            }
        }

        void OnBuyClick()
        {
            var shopBuffData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopDFSPData>();
            var item = shopBuffData.GetShopDFSPItemData(defensivePointId);
            if (UserData.Instance.GetMoney(item.moneyType) < item.price)
            {
                string content = string.Format(L.popup.insufficient_resource, L.currency_type.currency_0) + " " +
                                 L.popup.get_more_txt;
                PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(L.popup.notice_txt, content,
                    PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_ok, () => {
                        this._onInsufficientGem?.Invoke((int)ShopTabId.Gem);
                    }, L.button.btn_no,
                    null);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
                return;
            }

            UserData.Instance.SubMoney(item.moneyType, (long)item.price, AnalyticsConstants.SourceShop, "");
            UserData.Instance.UserHeroDefenseData.UnlockDefensePoint(this.defensivePointId);
            var fakeReward = new Reward() {
                id = this.defensivePointId, number = 1, type = ResourceType.DFSP, itemType = -1
            };
            PopupUtils.ShowMultipleReward(new[] {fakeReward});
            Repaint();
        }
    }
}