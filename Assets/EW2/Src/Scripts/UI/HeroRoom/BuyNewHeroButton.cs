using UnityEngine;


namespace EW2
{
    public class BuyNewHeroButton : IBuyHeroButton
    {
        public int HeroId { get; set; }
        public GameObject BuyBtn { get; set; }
        public GameObject EventBtn { get; set; }
        

        public void ButtonClick()
        {
            var genericBtn = new BuyHeroGenericButton(){HeroId = HeroId};
            genericBtn.ButtonClick();
        }

        public void ChangeBehavior()
        {
            var inEvent = UserData.Instance.UserEventData.NewHeroEventUserData.CheckCanShow();
            
            if (!inEvent)
            {
                BuyBtn.SetActive(true);
                EventBtn.SetActive(false);
            }
            else
            {
                BuyBtn.SetActive(false);
                EventBtn.SetActive(true);
            }
        }

        public void ButtonDisableClick()
        {
            
        }
    }
}