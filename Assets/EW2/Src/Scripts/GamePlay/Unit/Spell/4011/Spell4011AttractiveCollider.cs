namespace EW2
{
    public class Spell4011AttractiveCollider : ColliderTrigger<Dummy>, IAttractive
    {
        public void ChangeTarget(Unit unit)
        {
            var enemy = (EnemyBase)unit;
            if (enemy.MoveType == MoveType.Fly || enemy.UnitState.Current == ActionState.Stun)
            {
                return;
            }
            
            Totem totem = (Totem) owner;
            if (totem.UnitState.Current == ActionState.SkillPassive1)
            {
                var duration = totem.SpellStatBase.duration;
            
                var tauntStatus = new AttractiveOverTime(new StatusOverTimeConfig(){creator = totem, owner = enemy, lifeTime = duration});
                enemy.StatusController.AddStatus(tauntStatus);
            }
        }
    }
}