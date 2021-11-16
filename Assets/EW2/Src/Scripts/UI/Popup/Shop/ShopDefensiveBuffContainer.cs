using UnityEngine;

namespace EW2
{
    public class ShopDefensiveBuffContainer : TabContainer
    {
        [SerializeField] private Transform container;
        [SerializeField] private TabButton tabButton;
        
        public override void ShowContainer()
        {
            Repaint();
            gameObject.SetActive(true);
            
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void Repaint()
        {
            foreach (Transform t in this.container)
            {
                var item = t.GetComponent<ShopDefensiveBuffItemUi>();
                if (item)
                {
                    item.Repaint(this.tabButton.OnClickButton);
                }
            }
        }
    }
}