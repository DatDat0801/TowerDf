namespace EW2.Spell
{
    public class WarriorDamageBox : NormalDamageBox<Warrior4004>
    {
        protected override bool CanGetDamage(Unit target)
        {
            return isAoe || owner.SearchTarget.target.Value == target;
        }
    }
}