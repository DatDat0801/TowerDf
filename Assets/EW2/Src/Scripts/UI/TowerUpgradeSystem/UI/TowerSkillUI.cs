using Coffee.UIEffects;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EW2
{
    public class TowerSkillItem
    {
        public int towerId;
        public int towerLevel;
    }

    public class TowerSkillUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image activeIcon;
        [SerializeField] private Image activeFrame;
        [SerializeField] private Image selectedFrame;
        //[SerializeField] private Image inactiveIcon;
        [SerializeField] private Image inactiveFrame;
        
        [SerializeField] private Image connectedLine;

        //[SerializeField] private Text ingredientQuantity;
        [SerializeField] private GameObject activeIngredient;
        [SerializeField] private GameObject inactiveIngredient;

        public static TowerSkillUI CurrentSkillSelected { get; private set; }

        public TowerSkillItem currentSkillItem { get; private set; }

        
        public UnityAction<TowerSkillItem> OnSelect { get; set; }


        //private const string ATLAS_NAME = "";
        public void SelectItem()
        {
            selectedFrame.gameObject.SetActive(true);
            CurrentSkillSelected = this;
        }

        public void DeselectItem()
        {
            selectedFrame.gameObject.SetActive(false);
        }

        public void EnableSkill(bool enable)
        {
            if (enable)
            {
                //activeIcon.gameObject.SetActive(true);
                activeIcon.GetComponent<UIEffect>().enabled = false;
                activeFrame.gameObject.SetActive(true);
                if (connectedLine != null)
                {
                    connectedLine.gameObject.SetActive(true);
                }

                //inactiveIcon.gameObject.SetActive(false);
                inactiveFrame.gameObject.SetActive(false);
            }
            else
            {
                //activeIcon.gameObject.SetActive(false);
                activeIcon.GetComponent<UIEffect>().enabled = true;
                activeFrame.gameObject.SetActive(false);
                if (connectedLine != null)
                {
                    connectedLine.gameObject.SetActive(false);
                }

                //inactiveIcon.gameObject.SetActive(true);
                inactiveFrame.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// enable when can upgrade
        /// disable when upgraded, not enough ingredients, can not upgrade
        /// </summary>
        public void EnableIngredient(bool enable)
        {
            if (enable)
            {
                activeIngredient.gameObject.SetActive(true);
                inactiveIngredient.gameObject.SetActive(false);
            }
            else
            {
                activeIngredient.gameObject.SetActive(false);
                inactiveIngredient.gameObject.SetActive(true);
            }
        }

        public void DisableAllIngredient()
        {
            activeIngredient.gameObject.SetActive(false);
            inactiveIngredient.gameObject.SetActive(false);
        }

        public void Repaint(int towerId, int towerLevel, int starId)
        {
            currentSkillItem = new TowerSkillItem() {towerId = towerId, towerLevel = towerLevel};

            var towerUpgradeData = GameContainer.Instance.GetTowerUpgradeData();
            var quantity = towerUpgradeData.GetStarQuantity(starId, towerId, towerLevel);
            var activeQuantity = activeIngredient.GetComponentInChildren<Text>();
            var inactiveQuantity = inactiveIngredient.GetComponentInChildren<Text>();
            //Debug.LogAssertion(quantity + " towerid: " + towerId + " towerLevel:" + towerLevel);
            activeQuantity.text = quantity.ToString();
            inactiveQuantity.text = quantity.ToString();

            activeIcon.sprite = ResourceUtils.GetSpriteAtlas("tower_upgrade_system", $"icon_tower_skill_{towerId}_{towerLevel}");
            //inactiveIcon.sprite = ResourceUtils.GetSpriteAtlas("tower_upgrade_system", $"icon_tower_skill_{towerId}_{towerLevel}_gray");
            
        }

        public bool IsUpgraded()
        {
            return TowerUpgradeTool.IsActivated(currentSkillItem.towerId, currentSkillItem.towerLevel);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem();
            OnSelect?.Invoke(currentSkillItem);
        }
    }
}