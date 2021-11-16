using UnityEngine.Events;
using Zitga.UIFramework;

namespace EW2
{
    public static class PopupUtils
    {
        public static void ShowReward(Reward reward, UnityAction callBack = null)
        {
            if (UIFrame.Instance.IsPanelOpen(ScreenIds.single_reward))
            {
                UIFrame.Instance.CloseCurrentPanel(false);
                UIFrame.Instance.ShowPanel(ScreenIds.single_reward,
                    new ToastSingleRewardPanelProperties(reward, callBack));
            }
            else
            {
                UIFrame.Instance.ShowPanel(ScreenIds.single_reward,
                    new ToastSingleRewardPanelProperties(reward, callBack));
            }
        }

        public static void ShowReward(Reward[] rewards, UnityAction callBack = null)
        {
            if (rewards.Length == 0)
            {
                return;
            }

            if (rewards.Length == 1 && rewards[0].id == MoneyType.SkillPoint)
            {
                return;
            }

            if (rewards.Length == 1)
            {
                ShowReward(rewards[0]);
            }
            else
            {
                ShowMultipleReward(rewards, callBack);
            }
        }

        public static void ShowMultipleReward(Reward[] rewards, UnityAction callBack = null)
        {
            if (rewards.Length == 0)
            {
                return;
            }

            if (UIFrame.Instance.IsPanelOpen(ScreenIds.multi_reward))
            {
                UIFrame.Instance.CloseCurrentPanel(false);
                UIFrame.Instance.OpenWindow(ScreenIds.multi_reward, new MultiRewardWindowProperties(rewards, callBack));
            }
            else
            {
                UIFrame.Instance.OpenWindow(ScreenIds.multi_reward, new MultiRewardWindowProperties(rewards, callBack));
            }
        }

        public static void ShowPopupSuggestBuyResource(int moneyType)
        {
            var nameResource = Ultilities.GetNameCurrency(moneyType);
            var content = string.Format(L.popup.go_to_shop_question_txt, nameResource);
            var data = new PopupNoticeWindowProperties(L.popup.notice_txt.ToUpper(), content,
                PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_ok,
                () => {
                    var shopTabId = ShopTabId.Crystal;

                    if (moneyType == MoneyType.Crystal)
                    {
                        shopTabId = ShopTabId.Crystal;
                    }
                    else if (moneyType == MoneyType.Diamond)
                    {
                        shopTabId = ShopTabId.Gem;
                    }

                    var dataShopWindowProperties = new ShopWindowProperties(shopTabId);
                    UIFrame.Instance.OpenWindow(ScreenIds.shop_scene, dataShopWindowProperties);
                }, L.button.btn_no);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, data);
        }
    }
}