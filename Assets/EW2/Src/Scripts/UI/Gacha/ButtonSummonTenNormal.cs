using System;
using TigerForge;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class ButtonSummonTenNormal : ButtonSummonUi
    {
        protected GachaSpellDataBase dataBase;

        public void InitButton(string labelButton, bool isFree, GachaSpellDataBase data, SummonType summonType,
            Action<int, SummonType> summonCb)
        {
            this.dataBase = data;
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
            if (moneyType == dataBase.moneyTypeExchange)
            {
                var numberCurrency = UserData.Instance.GetMoney(dataBase.moneyTypeExchange);
                if (numberCurrency < gemReq)
                {
                    var titleNoti = string.Format(L.popup.insufficient_resource,
                        Ultilities.GetNameCurrency(dataBase.moneyTypeExchange));
                    Ultilities.ShowToastNoti(titleNoti);
                    return;
                }

                UserData.Instance.SubMoney(dataBase.moneyTypeExchange, gemReq, AnalyticsConstants.SourceSpellGachaNormal, "10",
                    false);
                if (summonCallback != null)
                    summonCallback.Invoke(10, summonType);
            }
            else
            {
                UserData.Instance.SubMoney(dataBase.moneyTypeKey, dataBase.numberKey,
                    AnalyticsConstants.SourceSpellGachaNormal, "10", false);
                if (summonCallback != null)
                    summonCallback.Invoke(10, summonType);
            }

            EventManager.EmitEvent(GamePlayEvent.OnRefreshSpellGacha);
        }
    }
}