using System;
using System.Collections.Generic;
using System.Text;
using EW2.Tutorial.General;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class HeroSkillContentController : TabContainer
    {
        [SerializeField] private Text txtHeroTag;

        [SerializeField] private Text txtHeroName;

        [SerializeField] private Text txtHeroLevel;

        [SerializeField] private Text txtSkillPoint;

        [SerializeField] private Text txtExp;

        [SerializeField] private Text txtLevelUp;

        [SerializeField] private Text txtCostLevelUp;

        [SerializeField] private Text txtUnlock;

        [SerializeField] private Text txtCostUnlock;
        
        [SerializeField] private Text txtCostUnlockDisable;

        [SerializeField] private Text txtTitleDesc;

        [SerializeField] private Text txtDesc;

        [SerializeField] private Text txtLvlSkill;

        [SerializeField] private Image fillExp;

        //[SerializeField] private Button btnUnlock;
        [SerializeField] private BuyHeroButton btnUnlock;

        [SerializeField] private Button btnReset;

        [SerializeField] private Button btnLevelUp;

        [SerializeField] private Button btnExp;

        [SerializeField] private Button btnSkillPoint;

        [SerializeField] private Button btnCover;

        [SerializeField] private List<ButtonSkillUpgradeController> buttonSkillUpgrade;

        [SerializeField] private Transform groupHeroClasses;

        private int heroId;

        private bool isUnlocked;

        private HeroStatBase heroStatNextLvl;

        private HeroCacheData heroData;

        private int currentSkillSelected;

        public Button BtnLevelUp => btnLevelUp;


        private void Awake()
        {
            //btnUnlock.onClick.AddListener(UnlockClick);

            btnReset.onClick.AddListener(ResetClick);

            BtnLevelUp.onClick.AddListener(LevelUpClick);

            btnExp.onClick.AddListener(ExpClick);

            btnSkillPoint.onClick.AddListener(SkillPointClick);
        }

        public ButtonSkillUpgradeController GetPassive3SkillUpgradeBtn() => buttonSkillUpgrade[3];


        #region Button

        private void SkillPointClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice,
                new PopupNoticeWindowProperties(string.Empty, L.popup.skill_point_guide,
                    PopupNoticeWindowProperties.PopupType.NoOption));
        }

        private void ExpClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice,
                new PopupNoticeWindowProperties(string.Empty, L.popup.hero_exp_guide,
                    PopupNoticeWindowProperties.PopupType.NoOption));
        }

        private void LevelUpClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.USER_UPGRADE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);

            if (heroData.HeroItemData.IsLevelMax()) return;

            var cost = heroStatNextLvl.maxExp - heroData.HeroItemData.exp;

            if (TutorialManager.Instance.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_1))
            {
                if (UserData.Instance.GetMoney(MoneyType.Crystal) < cost)
                {
                    var titleNoti = string.Format(L.popup.insufficient_resource, L.currency_type.currency_1);

                    Ultilities.ShowToastNoti(titleNoti);

                    return;
                }

                UserData.Instance.SubMoney(MoneyType.Crystal, (long)cost, AnalyticsConstants.SourceHeroRoom, "", false,
                    false);
            }
            else
            {
                TutorialManager.Instance.CompleteTutorial(AnyTutorialConstants.FOCUS_UPGRADE_LEVEL_HERO);
            }

            UserData.Instance.UserHeroData.GetHeroById(heroId).SetExp(heroData.HeroItemData.exp + (int)cost);

            UserData.Instance.Save();
        }

        private void ResetClick()
        {
            PopupNoticeWindowProperties properties = new PopupNoticeWindowProperties(string.Empty,
                L.popup.hero_skill_reset_notice, PopupNoticeWindowProperties.PopupType.TwoOption,
                L.button.btn_yes, ResetAllSkill, L.button.btn_no);

            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, properties);
        }

        private void ResetAllSkill()
        {
            heroData.HeroItemData.ResetAllSkill();

            UserData.Instance.Save();

            EventManager.EmitEventData(GamePlayEvent.OnResetAllSkill, heroId);

            Ultilities.ShowToastNoti(L.popup.reset_successful);

            RefreshUiSkill();
        }
        
        #endregion


        public void ShowInfo(HeroCacheData heroCacheData, bool isRefresh = false)
        {
            this.heroData = heroCacheData;

            this.heroId = heroData.HeroId;
            this.btnUnlock.SetHeroId(this.heroId);
            

            this.isUnlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(heroId);

            if (!heroData.HeroItemData.IsLevelMax())
            {
                heroStatNextLvl = GameContainer.Instance.GetHeroData(heroId).stats[heroData.HeroItemData.level];
            }
            else
            {
                heroStatNextLvl = GameContainer.Instance.GetHeroData(heroId).stats[heroData.HeroItemData.level - 1];
            }

            if (!isRefresh)
            {
                currentSkillSelected = -1;

                ShowUi();
            }
            else
            {
                UpdateInfoStat();

                RefreshUiSkill();
            }

            ShowHeroClasses();
        }

        private void ShowUi()
        {
            txtHeroTag.text = Ultilities.GetTagHero(heroId);

            txtHeroName.text = Ultilities.GetNameHero(heroId);

            txtLevelUp.text = L.button.btn_level_up_hero;

            this.btnReset.GetComponentInChildren<Text>().text = L.button.btn_reset;

            btnCover.GetComponentInChildren<Text>().text = L.popup.level_max_skill_txt;

            if (!isUnlocked)
            {
                txtUnlock.text = L.button.btn_unlock_now;

                var bundleUnlock = ProductsManager.HeroShopProducts[heroId];
                if (bundleUnlock != null)
                {
                    var price = ProductsManager.GetLocalPriceStringById(bundleUnlock.productId);

                    if (price.Equals("$0.01") || string.IsNullOrEmpty(price))
                    {
                        price = $"${bundleUnlock.price}";
                    }

                    txtCostUnlock.text = price;
                    txtCostUnlockDisable.text = price;
                }
            }

            txtLvlSkill.gameObject.SetActive(false);

            txtTitleDesc.text = L.popup.backstory_txt;

            txtDesc.text = Ultilities.GetHeroStory(heroId);

            UpdateInfoStat();

            ShowUiSkill();
        }

        private void UpdateInfoStat()
        {
            txtSkillPoint.text = heroData.HeroItemData.skillPoint + "";

            txtHeroLevel.text = $"{L.gameplay.level} {heroData.HeroItemData.level}/{GameConfig.HeroLevelMax}";

            if (heroData.HeroItemData.IsLevelMax())
            {
                txtExp.text = L.popup.max_level_hero_txt;

                fillExp.fillAmount = 1;
            }
            else
            {
                txtExp.text = $"{heroData.HeroItemData.exp}/{heroStatNextLvl.maxExp}";

                var percent = heroData.HeroItemData.exp * 1f / heroStatNextLvl.maxExp * 1f;

                fillExp.fillAmount = percent;

                txtCostLevelUp.text = $"{heroStatNextLvl.maxExp - heroData.HeroItemData.exp}";
            }

            btnUnlock.gameObject.SetActive(!isUnlocked);

            BtnLevelUp.gameObject.SetActive(isUnlocked);

            btnCover.gameObject.SetActive(heroData.HeroItemData.IsLevelMax());

            btnReset.gameObject.SetActive(isUnlocked && heroData.HeroItemData.IsCanResetSkill());
        }

        private void ShowUiSkill()
        {
            for (int i = 0; i < buttonSkillUpgrade.Count; i++)
            {
                buttonSkillUpgrade[i].InitButton(i, heroData);

                buttonSkillUpgrade[i].onSelect = OnSkillSelect;

                buttonSkillUpgrade[i].onUpgrade = OnSkillUpgrage;
            }
        }

        private void RefreshUiSkill()
        {
            foreach (var buttonSkill in buttonSkillUpgrade)
            {
                buttonSkill.UpdateData(heroData);

                if (buttonSkill.GetIdSkill() == currentSkillSelected)
                    buttonSkill.OnShow();
                else
                    buttonSkill.OnHide();
            }

            if (currentSkillSelected >= 0)
                ShowInfoSkill();
        }

        private void OnSkillUpgrage(int skillId)
        {
            ShowInfo(heroData, true);

            ShowInfoSkill();
        }

        private void OnSkillSelect(int skillId)
        {
            currentSkillSelected = skillId;

            ShowInfoSkill();

            foreach (var buttonSkill in buttonSkillUpgrade)
            {
                if (buttonSkill.GetIdSkill() == currentSkillSelected)
                    buttonSkill.OnShow();
                else
                    buttonSkill.OnHide();
            }
        }

        private void ShowInfoSkill()
        {
            txtTitleDesc.text = Ultilities.GetHeroSkillName(heroId, currentSkillSelected);

            txtLvlSkill.text =
                $"{L.gameplay.level} {heroData.HeroItemData.levelSkills[currentSkillSelected]}/{GameConfig.HeroSkillLevelMax}";

            txtLvlSkill.gameObject.SetActive(true);

            txtDesc.text = GetDescSkill();
        }

        private string GetDescSkill()
        {
            var skillLevel = heroData.HeroItemData.levelSkills[currentSkillSelected];

            var dbHero = GameContainer.Instance.GetHeroData(heroId);

            var statDesc = dbHero.GetDescStatSkill(currentSkillSelected);

            var descLocalize = Ultilities.GetHeroDescSkill(heroId, currentSkillSelected);

            var desc = HandleStringDescSkill(descLocalize, skillLevel, statDesc);

            return desc;
        }

        private string HandleStringDescSkill(string desc, int levelSkill, Dictionary<int, List<float>> dictStat)
        {
            var descConvert = desc;


            foreach (var stat in dictStat)
            {
                var stringBuilder = new StringBuilder();

                if (CheckStatSame(stat.Value))
                {
                    stringBuilder.Append($"<color='#9A8C6E'>{stat.Value[0]}</color>");
                }
                else
                {
                    for (int i = 0; i < stat.Value.Count; i++)
                    {
                        if (i != levelSkill - 1)
                        {
                            stringBuilder.Append($"<color='#9A8C6E'>{stat.Value[i]}</color>");
                        }
                        else
                        {
                            stringBuilder.Append($"<color='#ECA324'>{stat.Value[i]}</color>");
                        }

                        if (i < stat.Value.Count - 1)
                            stringBuilder.Append($"<color='#9A8C6E'>/</color>");
                    }
                }

                descConvert = descConvert.Replace("{" + stat.Key + "}", stringBuilder.ToString());
            }

            return descConvert;
        }

        private bool CheckStatSame(List<float> stats)
        {
            var valueCheck = 0f;

            for (int i = 0; i < stats.Count; i++)
            {
                if (i == 0) valueCheck = stats[i];

                if (Math.Abs(stats[i] - valueCheck) > 0)
                    return false;
            }

            return true;
        }

        private void ShowHeroClasses()
        {
            foreach (Transform child in groupHeroClasses)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < heroStatNextLvl.heroClasses.Length; i++)
            {
                var go = ResourceUtils.GetHeroClasses(groupHeroClasses);

                if (go != null)
                {
                    var script = go.GetComponent<HeroClassesControl>();

                    script.InitClasses(heroStatNextLvl.heroClasses[i]);
                }
            }
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }
    }
}
