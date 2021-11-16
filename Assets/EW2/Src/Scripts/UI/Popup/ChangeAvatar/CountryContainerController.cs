using TigerForge;

namespace EW2
{
    public class CountryContainerController: TabContainer
    {
        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }
    }
}