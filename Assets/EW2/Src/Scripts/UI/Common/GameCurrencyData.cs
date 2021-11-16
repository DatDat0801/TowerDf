using System.Collections.Generic;
using Sirenix.OdinInspector;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class GameCurrencyData : MonoBehaviour
    {
        [SerializeField] private Image iconCurrency;

        [SerializeField] private Text lbQuantity;

        [SerializeField] private Reward data;

        [SerializeField] private bool hasBtnInfo;

        [SerializeField] private bool hasBtnPlus;

        [SerializeField] private bool compareWithTotal;

        [ShowIf("hasBtnInfo")] [SerializeField]
        private Button infoButton;

        [ShowIf("hasBtnPlus")] [SerializeField]
        private Button buttonPlus;

        private void Awake()
        {
            switch (this.data.type)
            {
                case ResourceType.Money:
                {
                    EventManager.StartListening(GamePlayEvent.OnMoneyChange(this.data.type, this.data.id),
                        ShowCurrencyQuantity);
                    if (this.iconCurrency) this.iconCurrency.sprite = ResourceUtils.GetIconMoney(this.data.id);
                    break;
                }
                case ResourceType.Inventory:
                {
                    EventManager.StartListening(GamePlayEvent.OnConvertFragmentSpell, ShowCurrencyQuantity);
                    if (this.iconCurrency)
                        this.iconCurrency.sprite = ResourceUtils.GetSpriteSpell($"spellfrag_{this.data.id}");
                    break;
                }
            }

            if (buttonPlus)
            {
                buttonPlus.onClick.AddListener(ButtonPlusOnClick);
            }

            if (infoButton)
            {
                infoButton.onClick.AddListener(ShowInfoPopup);
            }
        }

        private void ShowInfoPopup()
        {
            bool isShard = this.data.itemType == InventoryType.SpellFragment;
            //ResourceType resourceType = ResourceType.Money;
            ItemInfoWindowProperties itemInfo =
                new ItemInfoWindowProperties(this.data.type, this.data.id, this.data.itemType, this.data.number,
                    isShard);
            UIFrame.Instance.OpenWindow(ScreenIds.item_info, itemInfo);
        }

        private void OnEnable()
        {
            ShowCurrencyQuantity();
        }

        private void ButtonPlusOnClick()
        {
            if (this.data.type == ResourceType.Money)
            {
                if (this.data.id == MoneyType.Crystal)
                {
                    ShowPopupInfo(ShopTabId.Crystal);
                }
                else if (this.data.id == MoneyType.Diamond)
                {
                    ShowPopupInfo(ShopTabId.Gem);
                }
                else if (this.data.id == MoneyType.Stamina)
                {
                    BuyStaminaWindowProperties properties = new BuyStaminaWindowProperties();
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_stamina, properties);
                }
                else if (this.data.id == MoneyType.TournamentTicket)
                {
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_tournament_ticket);
                }
                else
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
                }
            }
        }

        private void ShowCurrencyQuantity()
        {
            if (this.data.type == ResourceType.Money)
            {
                if (this.data.id == MoneyType.Stamina)
                {
                    lbQuantity.text = $"{UserData.Instance.GetMoney(this.data.id)}";
                    if (compareWithTotal)
                    {
                        lbQuantity.text += $"/{GameConfig.MaxStamina}";
                    }
                }
                else if (this.data.id == MoneyType.GoldStar)
                {
                    lbQuantity.text =
                        $"{GetCollectedStars(1)}";
                    if (compareWithTotal)
                    {
                        lbQuantity.text += $"/{(GameConfig.MAX_STAGE_ID_WORLD_1 + 1) * 3}";
                    }
                }
                else if (this.data.id == MoneyType.SliverStar)
                {
                    lbQuantity.text =
                        $"{GetCollectedStars(0)}";
                    if (compareWithTotal)
                    {
                        lbQuantity.text += $"/{(GameConfig.MAX_STAGE_ID_WORLD_1 + 1) * 3}";
                    }
                }
                else
                {
                    lbQuantity.text = "" + $"{UserData.Instance.GetMoney(this.data.id)}";
                }
            }
            else if (this.data.type == ResourceType.Inventory)
            {
                ItemInventoryBase fragInfo = UserData.Instance.GetInventory(this.data.itemType, this.data.id);
                if (fragInfo != null)
                {
                    lbQuantity.text = $"{fragInfo.Quantity}";
                }
                else
                {
                    lbQuantity.text = "0";
                }
            }
        }

        public int GetCollectedStars(int modeId)
        {
            int result = 0;
            HashSet<int> stages = new HashSet<int>();
            var campData = UserData.Instance.CampaignData;
            for (int i = 0; i < campData.CampaignDict.Count; i++)
            {
                var campaignId = MapCampaignInfo.GetCampaignId(0, i, modeId);
                if (campData.CampaignDict.ContainsKey(campaignId))
                {
                    stages.Add(campaignId);
                }
            }

            foreach (var stage in stages)
            {
                result += campData.GetStar(stage);
            }


            return result;
        }

        private void ShowPopupInfo(ShopTabId money)
        {
            string currentWindow = UIFrame.Instance.GetCurrentScreenId();
            if (!string.IsNullOrEmpty(currentWindow))
            {
                if (UIFrame.Instance.GetCurrentScreenId().Equals(ScreenIds.shop_scene))
                {
                    EventManager.EmitEventData(GamePlayEvent.OnFocusTabShop, (int)money);
                    return;
                }
            }

            UIFrame.Instance.CloseCurrentWindow();
            ShopWindowProperties properties = new ShopWindowProperties(money);
            UIFrame.Instance.OpenWindow(ScreenIds.shop_scene, properties);
        }
    }
}