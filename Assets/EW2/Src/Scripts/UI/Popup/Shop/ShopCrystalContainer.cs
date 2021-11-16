using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ShopCrystalContainer : TabContainer
    {
        [SerializeField] private Text txtTitleShop;

        [SerializeField] private Transform panelItem;

        [SerializeField] private TabButton _tabButton;
        private ShopCrystalData shopData;

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
            txtTitleShop.text = L.shop.crystal_pack_txt.ToUpper();

            for (int i = 0; i < panelItem.childCount; i++)
            {
                var itemShop = panelItem.GetChild(i).GetComponent<CrystalItemUi>();
                if (itemShop != null)
                {
                    if (i >= shopData.shopCrystalItemDatas.Length)
                        itemShop.gameObject.SetActive(false);
                    else
                        itemShop.gameObject.SetActive(true);

                    itemShop.SetData(shopData.shopCrystalItemDatas[i], _tabButton.OnClickButton);
                }
            }
        }

        private void GetData()
        {
            shopData = GameContainer.Instance.Get<ShopDataBase>().Get<ShopCrystalData>();
        }
    }
}
