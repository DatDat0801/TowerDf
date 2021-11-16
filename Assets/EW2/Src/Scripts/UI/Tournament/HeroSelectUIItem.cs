using System;
using EffectOnAnimFrameTool;
using EW2.CampaignInfo.HeroSelect;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroSelectUIItem : MonoBehaviour
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
            UIFrame.Instance.OpenWindow(ScreenIds.tournament_select_heroes_popup);
        }

        public HeroSelectedData Hero { get; private set; }

        public void Repaint(HeroSelectedData data)
        {
            this.Hero = data;

            icon.enabled = this.Hero.heroId > 0;
            this.plusIcon.SetActive(this.Hero.heroId <= 0);

            if (this.Hero.heroId >= 0)
            {
                var avatarId = (Hero.heroId % 1000) - 1;
                this.icon.sprite = ResourceUtils.GetSpriteAvatar(avatarId);
            }
        }

        public void ResetUI()
        {
            this.icon.enabled = false;
            this.plusIcon.SetActive(true);
        }
    }
}