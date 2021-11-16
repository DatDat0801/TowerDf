namespace EW2
{
    public class Hero1003AttractiveCollider : HeroAttractiveCollider
    {
        public override void ChangeTarget(Unit unit)
        {
            var enemy = (EnemyBase)unit;
            if (enemy.MoveType == MoveType.Fly || enemy.UnitState.Current == ActionState.Stun)
            {
                return;
            }
            
            Hero1003 hero = (Hero1003) owner;
            if (hero.UnitState.Current == ActionState.SkillPassive1)
            {
                var duration = hero.passiveSkill1.CurrentPassiveData.duration;
            
                var tauntStatus = new TauntStatusOverTime(new StatusOverTimeConfig(){creator = hero, owner = enemy, lifeTime = duration});
                enemy.StatusController.AddStatus(tauntStatus);
            }
            //var targetModifier = new TargetLimitedTimeModifier(enemy, owner, duration);
            //targetModifier.SetTarget();
        }
    }
}