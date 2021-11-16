using EW2.Spell;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class BuyHeroGenericButton : IBuyHeroButton
    {
        public int HeroId { get; set; }
        public GameObject EventBtn { get; set; }
        public GameObject BuyBtn { get; set; }
        public GameObject BuyDisableBtn { get; set; }

        public void ButtonClick()
        {
            ShopService.Instance.BuyHero(ProductsManager.HeroShopProducts[HeroId], UnlockHeroSuccess);
        }

        private void UnlockHeroSuccess(bool sucess, Reward[] gifts)
        {
            if (sucess)
            {
                Ultilities.ShowToastNoti(string.Format(L.popup.hero_unlocked, Ultilities.GetNameHero(HeroId)));
                EventManager.EmitEvent(GamePlayEvent.OnUpdateSaleBundle);
            }
        }

        public void ChangeBehavior()
        {
            this.EventBtn.SetActive(false);
            //make sure the buy button reset active state from switch other hero tab
            if (UnlockFeatureUtilities.IsCanClaimHero1003Free())
            {
                BuyDisableBtn.SetActive(true);
                BuyBtn.SetActive(false);
            }
            else
            {
                BuyDisableBtn.SetActive(false);
                BuyBtn.SetActive(true);
            }
        }

        public void ButtonDisableClick()
        {
            Ultilities.ShowToastNoti(L.game_event.claim_hero_notice);
        }
    }
}