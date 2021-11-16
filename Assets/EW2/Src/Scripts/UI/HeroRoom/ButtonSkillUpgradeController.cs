using System;
using EW2.Tutorial.General;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class ButtonSkillUpgradeController : MonoBehaviour
    {
        [SerializeField] private Image iconSkill;

        [SerializeField] private GameObject iconCover;

        [SerializeField] private GameObject focus;

        [SerializeField] private GameObject iconNoti;

        [SerializeField] private GameObject iconCanUpgrade;

        [SerializeField] private GameObject iconDisableUpgrade;

        [SerializeField] private Text txtCost;

        [SerializeField] private GameObject panelCost;

        private bool isSelected;

        private int itemId;

        private int levelSkill;

        private int costUpgrade;

        public Action<int> onSelect;

        public Action<int> onUpgrade;

        private HeroCacheData heroData;

        private HeroSkillUpgradeData skillUpgrade;

        public GameObject Focus
        {
            get => focus;
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public int GetIdSkill()
        {
            return itemId;
        }

        public void InitButton(int id, HeroCacheData heroCacheData)
        {
            itemId = id;

            isSelected = false;

            skillUpgrade = GameContainer.Instance.GetHeroSkillUpgrade();

            heroData = heroCacheData;

            levelSkill = heroData.HeroItemData.levelSkills[itemId];

            if (levelSkill < skillUpgrade.skillUpgradeDatas.Length)
                costUpgrade = skillUpgrade.skillUpgradeDatas[levelSkill].cost;

            OnHide();

            ShowUi();
        }

        public void UpdateData(HeroCacheData heroCacheData)
        {
            heroData = heroCacheData;

            levelSkill = heroData.HeroItemData.levelSkills[itemId];

            if (this.skillUpgrade == null)
                skillUpgrade = GameContainer.Instance.GetHeroSkillUpgrade();

            if (levelSkill < skillUpgrade.skillUpgradeDatas.Length)
                costUpgrade = skillUpgrade.skillUpgradeDatas[levelSkill].cost;
        }

        private void ShowUi()
        {
            iconSkill.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_{heroData.HeroId}_skill_{itemId}");

            CheckShowUpgradeNoti();
        }

        private void OnClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.USER_UPGRADE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);

            if (!isSelected)
            {
                isSelected = true;

                onSelect?.Invoke(itemId);
            }
            else
            {
                if (!CheckCanUpgrade())
                {
                    if (levelSkill >= GameConfig.HeroSkillLevelMax)
                    {
                        Ultilities.ShowToastNoti(L.popup.level_max_skill_txt);
                    }
                    else if (heroData.HeroItemData.skillPoint < costUpgrade)
                    {
                        Ultilities.ShowToastNoti(L.popup.insufficient_skill_point);
                    }

                    return;
                }

                heroData.HeroItemData.SetLevelSkill(itemId, levelSkill + 1);

                heroData.HeroItemData.SetSkillPoint(heroData.HeroItemData.skillPoint - costUpgrade);

                onUpgrade?.Invoke(itemId);

                ShowEffectUpgradeDone();

                var statModifier = GameContainer.Instance.GetHeroData(heroData.HeroId)
                    .GetStatModifierBySkill(itemId, levelSkill);

                if (statModifier.Item1 != RPGStatType.None)
                {
                    heroData.StatData.ClearStatModifier(statModifier.Item1, true);

                    heroData.StatData.AddStatModifier(statModifier.Item1, statModifier.Item2);

                    heroData.StatData.UpdateStatModifer(statModifier.Item1);
                }

                EventManager.EmitEvent(GamePlayEvent.OnUpgradeSkillHero);


                TutorialManager.Instance.CompleteTutorial(AnyTutorialConstants.FOCUS_UPGRADE_SKILL_HERO);


                UserData.Instance.Save();
            }
        }

        public void OnShow()
        {
            ShowHideNoti(false);

            iconCover.SetActive(true);


            Focus.SetActive(true);


            if (levelSkill < GameConfig.HeroSkillLevelMax)
            {
                ShowCost(true);

                CheckShowIconCanUpgrade();
            }
            else
            {
                iconCover.SetActive(false);

                ShowCost(false);

                iconCanUpgrade.SetActive(false);

                iconDisableUpgrade.SetActive(false);
            }
        }

        public void OnHide()
        {
            isSelected = false;

            iconCover.SetActive(false);

            Focus.SetActive(false);

            ShowCost(false);

            CheckShowUpgradeNoti();
        }

        private void CheckShowUpgradeNoti()
        {
            if (!isSelected && CheckCanUpgrade())
            {
                ShowHideNoti(true);
            }
            else
            {
                ShowHideNoti(false);
            }
        }

        private void ShowHideNoti(bool isShow)
        {
            iconCover.SetActive(isShow);

            iconNoti.SetActive(isShow);
        }

        private void ShowCost(bool isShow)
        {
            panelCost.SetActive(isShow);

            txtCost.text = "" + costUpgrade;

            if (CheckCanUpgrade())
            {
                txtCost.color = Color.white;
            }
            else
            {
                txtCost.color = Color.red;
            }
        }

        private void CheckShowIconCanUpgrade()
        {
            if (!isSelected) return;

            if (CheckCanUpgrade())
            {
                iconCanUpgrade.SetActive(true);

                iconDisableUpgrade.SetActive(false);
            }
            else
            {
                iconCanUpgrade.SetActive(false);

                iconDisableUpgrade.SetActive(true);
            }
        }

        private bool CheckCanUpgrade()
        {
            return levelSkill < GameConfig.HeroSkillLevelMax && heroData.HeroItemData.skillPoint >= costUpgrade;
        }

        private void ShowEffectUpgradeDone()
        {
            ResourceUtils.GetVfx("UI", "fx_ui_upgrade_skill_done", Vector3.zero, Quaternion.identity, Focus.transform);
        }
    }
}