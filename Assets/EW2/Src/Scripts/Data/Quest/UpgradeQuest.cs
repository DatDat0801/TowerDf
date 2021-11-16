using TigerForge;

namespace EW2
{
    public class UpgradeQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.AnyHeroUpgrade || targetType == TargetType.AnyHeroUpgradeMax ||
                this.targetType == TargetType.UpgradeHero)
            {
                EventManager.StartListening(GamePlayEvent.OnHeroLevelUp, OnHeroUpgrade);
            }
            else if (targetType == TargetType.AnySpellUpgrade || targetType == TargetType.SpellLevelMax ||
                     targetType == TargetType.AnySpellLevel)
            {
                EventManager.StartListening(GamePlayEvent.OnSpellUpgrade, OnSpellUpgrade);
            }
            else if (targetType == TargetType.AnyRune)
            {
                EventManager.StartListening(GamePlayEvent.OnEnhanceRune, OnRuneUpgrade);
            }
            else if (targetType == TargetType.AnyTower)
            {
                EventManager.StartListening(GamePlayEvent.OnSpentStarCampaign, OnUpgradeTower);
            }
        }

        private void OnUpgradeTower()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnRuneUpgrade()
        {
            if (IsCanIncrease())
            {
                Count++;
            }
        }

        private void OnSpellUpgrade()
        {
            if (targetType == TargetType.SpellLevelMax)
            {
                var spellId = EventManager.GetInt(GamePlayEvent.OnSpellUpgrade);
                var spellData = (SpellItem)UserData.Instance.GetInventory(InventoryType.Spell, spellId);
                if (spellData != null)
                {
                    if (spellData.IsLevelMax())
                    {
                        if (IsCanIncrease())
                        {
                            Count++;
                        }
                    }
                }
            }
            else if (targetType == TargetType.AnySpellUpgrade)
            {
                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (targetType == TargetType.AnySpellLevel)
            {
                var spellId = EventManager.GetInt(GamePlayEvent.OnSpellUpgrade);
                var spellData = (SpellItem)UserData.Instance.GetInventory(InventoryType.Spell, spellId);
                if (spellData != null)
                {
                    if (spellData.Level >= victim[0])
                    {
                        if (IsCanIncrease())
                        {
                            Count++;
                        }
                    }
                }
            }
        }

        private void OnHeroUpgrade()
        {
            var heroId = EventManager.GetInt(GamePlayEvent.OnHeroLevelUp);
            var heroData = UserData.Instance.UserHeroData.GetHeroById(heroId);
            if (targetType == TargetType.AnyHeroUpgrade)
            {
                if (victim[0] > 0)
                {
                    if (heroData.level < victim[0]) return;
                }

                if (IsCanIncrease())
                {
                    Count++;
                }
            }
            else if (targetType == TargetType.AnyHeroUpgradeMax)
            {
                if (heroData.IsLevelMax())
                {
                    if (IsCanIncrease())
                    {
                        Count++;
                    }
                }
            }
            else if (this.targetType == TargetType.UpgradeHero)
            {
                if (this.victim.Length >= 2)
                {
                    if (victim[0] > 0)
                    {
                        if (heroData.level < victim[0]) return;
                    }

                    if (victim[1] > 0)
                    {
                        if (heroId != victim[1]) return;
                    }
                }

                if (IsCanIncrease())
                {
                    Count++;
                }
            }
        }

        public override void CleanEventQuest()
        {
            base.CleanEventQuest();

            if (targetType == TargetType.AnyHeroUpgrade || targetType == TargetType.AnyHeroUpgradeMax ||
                this.targetType == TargetType.UpgradeHero)
            {
                EventManager.StopListening(GamePlayEvent.OnHeroLevelUp, OnHeroUpgrade);
            }
            else if (targetType == TargetType.AnySpellUpgrade || targetType == TargetType.SpellLevelMax ||
                     targetType == TargetType.AnySpellLevel)
            {
                EventManager.StopListening(GamePlayEvent.OnSpellUpgrade, OnSpellUpgrade);
            }
            else if (targetType == TargetType.AnyRune)
            {
                EventManager.StopListening(GamePlayEvent.OnEnhanceRune, OnRuneUpgrade);
            }
            else if (targetType == TargetType.AnyTower)
            {
                EventManager.StopListening(GamePlayEvent.OnSpentStarCampaign, OnUpgradeTower);
            }
        }

        public override void InsertData(QuestUserData userData)
        {
            base.InsertData(userData);
            CheckHeroUpgradedLevelMax();
            CheckHeroUpgradedReachLevel();
            CheckSpellUpgradedLevelMax();
            CheckSpellUpgradedLevel();
        }

        private void CheckHeroUpgradedLevelMax()
        {
            if (targetType == TargetType.AnyHeroUpgradeMax)
            {
                if (Count == 0)
                {
                    var listHero = UserData.Instance.UserHeroData.GetListHeroes();
                    foreach (var hero in listHero)
                    {
                        if (hero.IsLevelMax()) Count++;
                    }
                }
            }
        }

        private void CheckHeroUpgradedReachLevel()
        {
            if (targetType == TargetType.AnyHeroUpgrade)
            {
                if (Count == 0)
                {
                    var listHero = UserData.Instance.UserHeroData.GetListHeroes();
                    foreach (var hero in listHero)
                    {
                        if (victim[0] > 0)
                        {
                            if (hero.level < victim[0]) continue;
                        }

                        if (IsCanIncrease()) Count++;
                    }
                }
            }
        }

        private void CheckSpellUpgradedLevelMax()
        {
            if (targetType == TargetType.SpellLevelMax)
            {
                if (Count == 0)
                {
                    var listSpell = UserData.Instance.GetListSpell();
                    foreach (var spel in listSpell)
                    {
                        if (spel.IsLevelMax()) Count++;
                    }
                }
            }
        }

        private void CheckSpellUpgradedLevel()
        {
            if (targetType == TargetType.AnySpellLevel)
            {
                if (Count == 0)
                {
                    var listSpell = UserData.Instance.GetListSpell();
                    foreach (var spel in listSpell)
                    {
                        if (spel.Level == victim[0]) Count++;
                    }
                }
            }
        }

        public override void GoToTarget()
        {
            base.GoToTarget();

            if (targetType == TargetType.AnyHeroUpgrade || targetType == TargetType.AnyHeroUpgradeMax ||
                this.targetType == TargetType.UpgradeHero)
            {
                DirectionGoTo.GotoHeroRoom();
            }
            else if (targetType == TargetType.AnySpellUpgrade || targetType == TargetType.SpellLevelMax ||
                     targetType == TargetType.AnySpellLevel)
            {
                DirectionGoTo.GotoUpgradeSpell();
            }
            else if (targetType == TargetType.AnyTower)
            {
                DirectionGoTo.GotoUpgradeTower();
            }
            else if (targetType == TargetType.AnyRune)
            {
                DirectionGoTo.GotoRuneHeroRoom();
            }
        }
    }
}