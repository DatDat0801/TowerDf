using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class GloryRoadTierItemUI : MonoBehaviour
    {
        [SerializeField] private Text tierNumberText;

        [SerializeField] private GameObject numberBgDisable;
        [SerializeField] private GameObject numberBgEnable;

        [SerializeField] private GameObject disableProgressBar;
        [SerializeField] private GameObject enableProgressBar;
        [SerializeField] private GameObject disableTierBg;
        [SerializeField] private GameObject enableTierBg;
        [SerializeField] private GameObject glowMark;

        [SerializeField] private GloryRoadRewardItem freeItem;
        [SerializeField] private GloryRoadRewardItem premiumItem;


        public void Repaint(GloryRoadTierItem gloryRoadTierItem, GloryRoadUserData userData)
        {
            var unlockedTier = userData.UnlockedTier();
            if (userData.IsTierUnlocked(gloryRoadTierItem.tierId))
            {
                disableTierBg.SetActive(false);
                enableTierBg.SetActive(true);
                numberBgDisable.SetActive(false);
                numberBgEnable.SetActive(true);
            }
            else
            {
                disableTierBg.SetActive(true);
                enableTierBg.SetActive(false);
                numberBgDisable.SetActive(true);
                numberBgEnable.SetActive(false);
            }

            if (unlockedTier == gloryRoadTierItem.tierId)
            {
                glowMark.SetActive(true);
            }
            else
            {
                glowMark.SetActive(false);
            }

            tierNumberText.text = gloryRoadTierItem.tierId.ToString();
            if (unlockedTier < gloryRoadTierItem.tierId)
            {
                disableProgressBar.SetActive(true);
                enableProgressBar.SetActive(false);
            }
            else
            {
                disableProgressBar.SetActive(false);
                enableProgressBar.SetActive(true);
            }

            freeItem.RepaintFreeItem(gloryRoadTierItem, userData);
            premiumItem.RepaintPremiumItem(gloryRoadTierItem, userData);
        }

        public GloryRoadRewardItem GetFreeItem()
        {
            return freeItem;
        }
    }
}
