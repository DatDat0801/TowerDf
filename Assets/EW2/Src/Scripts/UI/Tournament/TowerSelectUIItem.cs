using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class TowerSelectUIItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private GameObject plusIcon;
        [SerializeField] private Button addBtn;

        private void Awake()
        {
            this.addBtn.onClick.AddListener(AddNewHero);
        }

        private void AddNewHero()
        {
            //EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
        }

        //public HeroSelectedData Hero { get; private set; }

        public void Repaint(int towerId)
        {
            if (this.icon)
            {
               this.icon.sprite = ResourceUtils.GetSpriteAtlas("avatar_tower", $"tower_{towerId.ToString()}");
            }
            this.plusIcon.SetActive(false);
        }

        // public void ResetUI()
        // {
        //     this.icon.enabled = false;
        //     this.plusIcon.SetActive(true);
        // }
    }
}