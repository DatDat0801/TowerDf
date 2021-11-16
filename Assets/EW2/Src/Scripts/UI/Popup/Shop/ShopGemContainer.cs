using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ShopGemContainer : TabContainer
    {
        [SerializeField] private Text txtTitleShop;

        [SerializeField] private Transform panelItem;

        private ShopGemData shopData;

        public override void ShowContainer()
        {
            if (shopData == null)
                GetData();

            ShowUi();

            gameObject.SetActive(true);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void ShowUi()
        {
            txtTitleShop.text = L.shop.diamond_pack_txt.ToUpper();

            for (int i = 0; i < panelItem.childCount; i++)
            {
                var itemShop = panelItem.GetChild(i).GetComponent<GemItemUi>();
                if (itemShop != null)
                {
                    if (i >= shopData.shopItemDatas.Length)
                        itemShop.gameObject.SetActive(false);
                    else
                        itemShop.gameObject.SetActive(true);

                    itemShop.SetData(shopData.shopItemDatas[i]);
                }
            }
        }

        private void GetData()
        {
            shopData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopGemData>();
        }
    }
}
