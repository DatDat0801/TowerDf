using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;

namespace EW2
{
    public class TowerSkillInfoUI : MonoBehaviour
    {
        [SerializeField] private Text skillTitle;
        [SerializeField] private Text skillInfo;
        [SerializeField] private Text ingredientQuantity;
        [SerializeField] private Image ingredientIcon;
        [SerializeField] private Image ingredientIcon2;

        [SerializeField] private Text noticeTxt;
        [SerializeField] private GameObject upgradeBtn;
        [SerializeField] private Button closeDetail;
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Text starText;
        [SerializeField] private Image selectedIcon;

        private void Start()
        {
            closeDetail.onClick.AddListener(CloseDetailPanel);
        }

        public void OpenDetailPanel()
        {
            detailPanel.gameObject.SetActive(true);
        }

        public void CloseDetailPanel()
        {
            detailPanel.gameObject.SetActive(false);
        }

        public void NoticeUpgradeSuccess()
        {
            noticeTxt.text = L.popup.upgrade_successful;
            upgradeBtn.SetActive(false);
            noticeTxt.gameObject.SetActive(true);
        }

        public void NoticeUpgradeActivated()
        {
            noticeTxt.text = L.popup.upgrade_activated;
            upgradeBtn.SetActive(false);
            noticeTxt.gameObject.SetActive(true);
        }

        public void NoNoticeUpgrade()
        {
            upgradeBtn.SetActive(true);
            noticeTxt.gameObject.SetActive(false);
        }

        public void Repaint(int towerId, int level, int starId)
        {
            var towerUpgradeData = GameContainer.Instance.GetTowerUpgradeData();
            string localizeInfoKey = $"tower_up_des_{towerId.ToString()}_{(level - 1).ToString()}";
            string localizeTitleKey = $"tower_up_name_{towerId.ToString()}_{(level - 1).ToString()}";

            skillTitle.text = Localization.Current.Get(nameof(L.upgrade_tower), localizeTitleKey);

            string infoStr = Localization.Current.Get(nameof(L.upgrade_tower), localizeInfoKey);
            switch (towerId)
            {
                case 2001:
                    var bonusStat2001 = towerUpgradeData.GetBonusStat2001ByLevel(level);
                    if (level == 3)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2001.targets);
                    }
                    else if (level == 6)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2001.level6Stat.criticalRatio * 100,
                            bonusStat2001.level6Stat.criticalDamage * 100);
                    }
                    else
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2001.GetValueByLevel() * 100);
                    }

                    break;
                case 2002:
                    var bonusStat2002 = towerUpgradeData.GetBonusStat2002ByLevel(level);
                    if (level == 3)
                    {
                        skillInfo.text = string.Format(infoStr);
                    }
                    else if (level == 6)
                    {
                        skillInfo.text = string.Format(infoStr, Mathf.Abs(bonusStat2002.level6Stat.damage));
                    }
                    else if (level == 5)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2002.upgradedBall);
                    }
                    else
                    {
                        skillInfo.text = string.Format(infoStr,Mathf.Abs(bonusStat2002.GetValueByLevel() * 100) );
                    }

                    break;
                case 2003:
                    var bonusStat2003 = towerUpgradeData.GetBonusStat2003ByLevel(level);
                    if (level == 3)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2003.level3Stat.fireBulletRatio * 100,
                            bonusStat2003.level3Stat.fireBulletDamage, bonusStat2003.level3Stat.affectedTime,
                            bonusStat2003.level3Stat.poisonousFire, bonusStat2003.level3Stat.poisonousFireRate);
                    }
                    else if (level == 6)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2003.level6Stat.secondDamage,
                            bonusStat2003.level6Stat.slowDown * 100);
                    }
                    else
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2003.GetValueByLevel() * 100);
                    }

                    break;
                case 2004:
                    var bonusStat2004 = towerUpgradeData.GetBonusStat2004ByLevel(level);
                    if (level == 3)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2004.troopNumber);
                    }
                    else if (level == 6)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2004.ghostExistTime);
                    }
                    else if (level == 5)
                    {
                        skillInfo.text = string.Format(infoStr, bonusStat2004.level5Stat.skill2Armor * 100,
                            bonusStat2004.level5Stat.skill2MagicResistance * 100);
                    }
                    else
                    {
                        skillInfo.text = string.Format(infoStr, Mathf.Abs(bonusStat2004.GetValueByLevel() * 100));
                    }

                    break;
            }

            if (starId == MoneyType.SliverStar)
            {
                ingredientIcon.overrideSprite = ResourceUtils.GetIconMoney(MoneyType.SliverStar);
                ingredientIcon2.overrideSprite = ResourceUtils.GetIconMoney(MoneyType.SliverStar);
            }
            else
            {
                ingredientIcon.overrideSprite = ResourceUtils.GetIconMoney(MoneyType.GoldStar);
                ingredientIcon2.overrideSprite = ResourceUtils.GetIconMoney(MoneyType.GoldStar);
            }

            var quantity = towerUpgradeData.GetStarQuantity(starId, towerId, level);
            ingredientQuantity.text = quantity.ToString();
            selectedIcon.sprite =
                ResourceUtils.GetSpriteAtlas("tower_upgrade_system", $"icon_tower_skill_{towerId}_{level}");
        }

        public void RepaintStarQuantity(int remaining, int totalStarNeeded)
        {
            starText.text = $"{remaining.ToString()}/{totalStarNeeded.ToString()}";
        }
    }
}