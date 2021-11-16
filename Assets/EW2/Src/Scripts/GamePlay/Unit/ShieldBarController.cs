using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ShieldBarController : MonoBehaviour
    {
        [SerializeField] private GameObject shieldBarFrame;

        [SerializeField] private Image shieldBar;

        private Vector3 shieldScale;

        private float maxHp;

        private ShieldPoint shieldPoint;

        public void SetShieldBar(ShieldPoint shield)
        {
            this.shieldPoint = shield;

            this.maxHp = shield.Value;

            UpdateHealth(maxHp);

            shieldPoint.PropertyChanged += (sender, args) => UpdateHealth(shieldPoint.Value);
        }

        public void UpdateHealth(float hp)
        {
            if ( hp <= 0 || maxHp <= 0)
            {
                Hide();
            }
            else
            {
                Show();

                shieldBar.fillAmount = hp / maxHp;
            }
        }

        private void Show()
        {
            shieldBarFrame.SetActive(true);
        }

        public void Hide()
        {
            shieldBarFrame.SetActive(false);
        }
    }
}