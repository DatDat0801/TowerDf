using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace EW2
{
    public class TowerUpgradeNotifyUI : MonoBehaviour
    {
        public GameObject icon;

        private void OnEnable()
        {
            //init
            StarChanged(MoneyType.SliverStar);
            TowerUpgradeWindowController.OnStarChanged += StarChanged;
        }

        private void OnDisable()
        {
            TowerUpgradeWindowController.OnStarChanged -= StarChanged;
        }

        public void StarChanged(int starId)
        {
            var canUpgradeSilver = TowerUpgradeTool.CanUpgradeSomething(MoneyType.SliverStar);
            var canUpgradeGolden = TowerUpgradeTool.CanUpgradeSomething(MoneyType.GoldStar);
            if (canUpgradeGolden || canUpgradeSilver)
            {
                
                icon.SetActive(true);
            }
            else
            {
                
                icon.SetActive(false);
            }

        }
    }
}