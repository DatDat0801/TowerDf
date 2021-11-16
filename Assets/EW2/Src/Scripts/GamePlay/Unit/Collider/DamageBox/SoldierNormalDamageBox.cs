namespace EW2
{
    public class SoldierNormalDamageBox: NormalDamageBox<SoldierBase>
    {
        protected override bool CanGetDamage(Unit target)
        {
            return isAoe || owner.SearchTarget.target.Value == target;
        }
    }
}