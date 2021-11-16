using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class VirtualHpBarController : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarFrame;

        [SerializeField] private Image healthBar;

        private HealthPoint healthPoint;
        private Vector3 hpScale;

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

            //Hide(0f);
        }

        public void UpdateHealth()
        {
            var ratio = healthPoint.CalculateCurrentPercent();

            if (ratio > 1f) ratio = 1f;

            this.healthBar.fillAmount = ratio;
            if (ratio <= 0) Hide(0f);
        }

        // public void ResetHealthBar()
        // {
        //     healthBar.localScale = Vector3.one;
        // }
    }
}