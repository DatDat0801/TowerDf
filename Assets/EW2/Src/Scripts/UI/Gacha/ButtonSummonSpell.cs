using System;
using TigerForge;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class ButtonSummonSpell : ButtonSummonUi
    {
        protected GachaSpellDataBase dataBase;

        public void InitButton(bool isFree, GachaSpellDataBase data, SummonType summonType,
            Action<int, SummonType> summonCb)
        {
            this.dataBase = data;
            var labelButton = $"{L.button.summon_btn} x{data.numberKey}";
            InitButton(labelButton, isFree, summonType, summonCb);
        }

        public override void ShowUiPrice()
        {
            var numberKey = UserData.Instance.GetMoney(dataBase.moneyTypeKey);

            if (numberKey >= dataBase.numberKey)
            {
                if (imgMoneyType)
                {
                    imgMoneyType.sprite = ResourceUtils.GetIconMoney(dataBase.moneyTypeKey);
                }

                if (txtLabelPrice)
                {
                    txtLabelPrice.text = $"{dataBase.numberKey}";
                }

                moneyType = dataBase.moneyTypeKey;
            }
            else
            {
                if (imgMoneyType)
                {
                    imgMoneyType.sprite = ResourceUtils.GetIconMoney(dataBase.moneyTypeExchange);
                }

                gemReq = dataBase.numberKey * dataBase.costExchange;

                if (txtLabelPrice)
                {
                    txtLabelPrice.text = $"{gemReq}";
                }

                moneyType = dataBase.moneyTypeExchange;
            }
        }

        protected override void SummonOnClick()
        {
            if (txtLabelFree && txtLabelFree.gameObject.activeSelf)
            {
                if (summonType == SummonType.Normal)
                    UserData.Instance.OtherUserData.timeCountdownNormalSpell =
                        TimeManager.NowInSeconds + (long)(3600 * dataBase.timeCountdownFree);
                else
                    UserData.Instance.OtherUserData.timeCountdownPremiumSpell =
                        TimeManager.NowInSeconds + (long)(3600 * dataBase.timeCountdownFree);

                EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
                if (summonCallback != null)
                    summonCallback.Invoke(dataBase.numberKey, summonType);
            }
            else
            {
                var resourceId = summonType == SummonType.Normal
                    ? AnalyticsConstants.SourceSpellGachaNormal
                    : AnalyticsConstants.SourceSpellGachaPremium;

                if (moneyType == dataBase.moneyTypeExchange)
                {
                    var numberCurrency = UserData.Instance.GetMoney(dataBase.moneyTypeExchange);
                    if (numberCurrency < gemReq)
                    {
                        PopupUtils.ShowPopupSuggestBuyResource(MoneyType.Diamond);
                        return;
                    }

                    UserData.Instance.SubMoney(dataBase.moneyTypeExchange, gemReq, resourceId,
                        dataBase.moneyTypeExchange.ToString());
                    if (summonCallback != null)
                        summonCallback.Invoke(dataBase.numberKey, summonType);
                }
                else
                {
                    UserData.Instance.SubMoney(dataBase.moneyTypeKey, dataBase.numberKey, resourceId,
                        dataBase.moneyTypeKey.ToString());
                    if (summonCallback != null)
                        summonCallback.Invoke(dataBase.numberKey, summonType);
                }
            }

            EventManager.EmitEvent(GamePlayEvent.OnRefreshSpellGacha);
        }
    }
}