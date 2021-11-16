namespace EW2
{
    public class BuyNowUserData : BaseBundleData
    {
        public override bool CheckCanShow()
        {
            var buyNowDataBase = GameContainer.Instance.Get<ShopDataBase>().Get<BuyNowData>();

            if (!isOpen)
            {
                if (this.timeEnd <= 0)
                {
                    isOpen = true;
                    SetTimeStart(TimeManager.NowInSeconds);
                    var timeEnd = TimeManager.NowInSeconds + buyNowDataBase.packConditions[0].duration;
                    SetTimeEnd(timeEnd);
                    UserData.Instance.Save();
                    return true;    
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var productId = buyNowDataBase.shopItemDatas[0].productId;
                if (TimeRemain() > 0 && !UserData.Instance.UserShopData.CheckPackNonconsumePurchased(productId))
                {
                    return true;
                }
                else
                {
                    isOpen = false;
                    UserData.Instance.Save();
                    return false;
                }
            }
         }
    }
}