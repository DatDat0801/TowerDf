using UnityEngine;
using UnityEngine.UI;

namespace EW2.CampaignInfo.PowerUpSelect
{
    public class PowerUpSelectedData
    {
        public int id;

        public int quantity;
    }
    
    public class PowerUpSelectedView : MonoBehaviour
    {
        [SerializeField] private Image icon;

        [SerializeField] private Text txtNumber;

        private PowerUpSelectedData data;
        
        public void SetInfo(PowerUpSelectedData data)
        {
            this.data = data;
            
            icon.gameObject.SetActive(this.data.id >= 0);

            if (this.data.id >= 0)
            {
                icon.sprite = ResourceUtils.GetSpriteAtlas("power_up", this.data.id.ToString());

                txtNumber.text = this.data.quantity.ToString();
            }
        }
    }
}