using Coffee.UIEffects;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ShopTournamentUIItem : MonoBehaviour
    {
        [SerializeField] private Transform itemContainer;
        [SerializeField] private Text price;
        [SerializeField] private Text remainingAvailable;
        [SerializeField] private Button buyButton;
        private RewardUI _rewardUi;
        private ShopTournamentItem _item;

        public void Repaint(ShopTournamentItem item)
        {
            this._item = item;
            //reward
            var reward = item.item; //.Generate()[0];
            this._rewardUi = ResourceUtils.GetRewardUi(reward.type);
            this._rewardUi.SetData(reward);
            this._rewardUi.SetParent(this.itemContainer);

            if (this.price)
            {
                this.price.text = $"{item.price.ToString()}";
            }

            if (this.remainingAvailable)
            {
                this.remainingAvailable.text = $"{L.popup.available_txt} {item.purchaseAvailable.ToString()}";
            }

            this.buyButton.onClick.RemoveAllListeners();
            this.buyButton.onClick.AddListener(Buy);

            // var effects = gameObject.GetComponentsInChildren<UIEffect>();
            // foreach (var effect in effects)
            // {
            //     if (item.purchaseAvailable > 0) { effect.enabled = false; }
            //     else { effect.enabled = true; }
            // }

            var graphics = gameObject.GetComponentsInChildren<Graphic>();
            foreach (Graphic graphic in graphics)
            {
                var outline = graphic.gameObject.GetComponent<Outline>();
                if (outline == null)
                {
                    var uieffect = graphic.GetOrAddComponent<UIEffect>();
                    uieffect.effectMode = EffectMode.Grayscale;
                    uieffect.effectFactor = 1f;
                    uieffect.colorMode = ColorMode.Add;
                    uieffect.colorFactor = 0;
                
                    if (item.purchaseAvailable > 0) { uieffect.enabled = false; }
                    else { uieffect.enabled = true; }
                }
            }
        }

        void Buy()
        {
            var grandMaster = UserData.Instance.GlobalResources.GetMoney(MoneyType.GrandMaster);
            if (this._item.item.type == ResourceType.Hero)
            {
                if (UserData.Instance.UserHeroData.CheckHeroUnlocked(this._item.item.id))
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.already_own_this_item_txt);
                    return;
                }
            }

            if (this._item.price <= grandMaster)
            {
                var shopData = UserData.Instance.TournamentData.ShopUserData;
                bool success = shopData.Purchase(this._item.shopItemId, out var remainItem);
                if (success)
                {
                    //buy
                    UserData.Instance.SubMoney(MoneyType.GrandMaster, this._item.price, "tournament_store", "");
                    var rewards = this._item.Generate();
                    PopupUtils.ShowReward(rewards);
                    Reward.AddToUserData(rewards, "tournament_shop");
                    UserData.Instance.Save();
                    Repaint(remainItem);
                }
                else
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.out_of_stock_txt);
                }
            }
            else
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                    string.Format(L.popup.insufficient_resource, L.currency_type.currency_4));
            }
        }
    }
}