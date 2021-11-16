using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class SkillOptionButton : TowerOptionButton
    {
        [SerializeField]private Image imageIcon;

        [SerializeField] private Image[] imageSkills;

        private BranchType GetBranchType()
        {
            return myAction == TowerOption.Action.RaiseSkill1 ? BranchType.Skill1 : BranchType.Skill2;
        }
        
        private int GetBranchId()
        {
            return myAction == TowerOption.Action.RaiseSkill1 ? 1 : 2;
        }

        protected override void SetUIUpgrade()
        {
            var isEnablePrice = false;

            var branch = GetBranchType();

            if (!Tower.CheckUpgradeMaxLevelSkill(branch))
            {
                goldNeed = Tower.GetPriceRaiseSkill(branch);

                lbPrice.text = goldNeed.ToString();

                isEnablePrice = true;

                var gold = GamePlayData.Instance.GetMoneyInGame(MoneyInGameType.Gold);

                UpdateBg(gold >= goldNeed);
            }

            lbPrice.transform.parent.gameObject.SetActive(isEnablePrice);
            lockButton = !isEnablePrice;
            
            SetSkill();
            
            SetIcon();
        }

        private void SetIcon()
        {
            imageIcon.sprite = ResourceUtils.GetSpriteAtlas("tower", $"skill_{Tower.Id}_{GetBranchId()}");
            imageIcon.SetNativeSize();
        }

        private void SetSkill()
        {
            var skill = Tower.GetSkill(GetBranchType());

            for (int i = 0; i < imageSkills.Length; i++)
            {
                imageSkills[i].enabled = skill.Level > i;
            }
        }

        public override void LockBtn()
        {
            base.LockBtn();
            imageIcon.gameObject.SetActive(false);
            foreach (var imageSkill in imageSkills)
            {
                imageSkill.gameObject.SetActive(false);
            }
        }

        public override void UnlockBtn()
        {
            base.UnlockBtn();
            imageIcon.gameObject.SetActive(true);
            foreach (var imageSkill in imageSkills)
            {
                imageSkill.gameObject.SetActive(true);
            }
        }
    }
}