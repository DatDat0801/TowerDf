using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EW2
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarFrame;

        [SerializeField] private Transform healthBar;

        private Vector3 hpScale;

        private HealthPoint healthPoint;

        public void Show()
        {
            healthBarFrame.SetActive(true);
        }

        public async void Hide(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            if (healthBarFrame != null)
                healthBarFrame.SetActive(false);
        }

        public void SetHealthBar(HealthPoint health)
        {
            this.healthPoint = health;

            UpdateHealth();

            healthPoint.PropertyChanged += (sender, args) => UpdateHealth();
            
            Hide(0f);
        }

        public void UpdateHealth()
        {
            var ratio = healthPoint.CalculateCurrentPercent();

            if (ratio > 1f) ratio = 1f;

            if (ratio >= 1f || ratio <= 0)
            {
                hpScale = healthBar.localScale;

                hpScale.x = ratio;
                
                healthBar.localScale = hpScale;
                
                Hide(1.5f);
            }
            else
            {
                Show();

                hpScale = healthBar.localScale;

                hpScale.x = ratio;

                healthBar.localScale = hpScale;
            }
        }

        public void ResetHealthBar()
        {
            healthBar.localScale = Vector3.one;
        }
    }
}