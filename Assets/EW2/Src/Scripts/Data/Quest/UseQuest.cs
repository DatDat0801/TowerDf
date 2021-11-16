using TigerForge;

namespace EW2
{
    public class UseQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();

            if (targetType == TargetType.UseAnySpell)
            {
                EventManager.StartListening(GamePlayEvent.OnUseSpell, OnUseSpell);
            }
            else if (targetType == TargetType.UseSkillAnyHero || this.targetType == TargetType.UseSkillHero)
            {
                EventManager.StartListening(GamePlayEvent.OnHeroUseActiveSkill, OnHeroUseActiveSkill);
            }
            else if (targetType == TargetType.AnyRune || this.targetType == TargetType.EquipRuneForHero)
            {
                EventManager.StartListening(GamePlayEvent.OnEquipRune, OnEquipRune);
            }
            else if (targetType == TargetType.Hero)
            {
                EventManager.StartListening(GamePlayEvent.OnPlayCampaign, OnPlayCampaign);
            }
        }

        private void OnPlayCampaign()
        {
            var isPass = false;

            foreach (var heroSelected in UserData.Instance.UserHeroData.SelectedHeroes)
            {
                if (heroSelected.heroId == this.victim[0])
                {
                    isPass = true;
                    break;
                }
            }

            if (isPass && IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnEquipRune()
        {
            var heroId = EventManager.GetInt(GamePlayEvent.OnEquipRune);
            var heroData = UserData.Instance.UserHeroData.GetHeroById(heroId);

            if (heroData != null)
            {
                if (this.targetType == TargetType.AnyRune)
                {
                    var countEquip = 0;
                    foreach (var rune in heroData.runeEquips)
                    {
                        if (rune >= 0)
                            countEquip++;
                    }

                    if (countEquip >= victim[0])
                        if (IsCanIncrease())
                        {
                            Count++;
                        }
                }
                else if (this.targetType == TargetType.EquipRuneForHero)
                {
                    if (heroId == this.victim[0] && IsCanIncrease())
                    {
                        Count++;
                    }
                }
            }
        }

        private void OnHeroUseActiveSkill()
        {
            var heroId = EventManager.GetInt(GamePlayEvent.OnHeroUseActiveSkill);

            if (this.victim[0] > 0)
            {
                if (heroId == this.victim[0] && IsCanIncrease())
                {
                    Count++;
                }

                return;
            }

            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnUseSpell()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        public override void CleanEventQuest()
        {
            if (targetType == TargetType.UseAnySpell)
            {
                EventManager.StopListening(GamePlayEvent.OnUseSpell, OnUseSpell);
            }
            else if (targetType == TargetType.UseSkillAnyHero || this.targetType == TargetType.UseSkillHero)
            {
                EventManager.StopListening(GamePlayEvent.OnHeroUseActiveSkill, OnHeroUseActiveSkill);
            }
            else if (targetType == TargetType.AnyRune)
            {
                EventManager.StopListening(GamePlayEvent.OnEquipRune, OnEquipRune);
            }
            else if (targetType == TargetType.Hero)
            {
                EventManager.StopListening(GamePlayEvent.OnPlayCampaign, OnPlayCampaign);
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();

            if (targetType == TargetType.UseAnySpell || targetType == TargetType.UseSkillAnyHero ||
                targetType == TargetType.Hero || this.targetType == TargetType.UseSkillHero)
            {
                DirectionGoTo.GotoNormalCampaign();
            }
            else if (targetType == TargetType.AnyRune || this.targetType == TargetType.EquipRuneForHero)
            {
                DirectionGoTo.GotoRuneHeroRoom();
            }
        }
    }
}