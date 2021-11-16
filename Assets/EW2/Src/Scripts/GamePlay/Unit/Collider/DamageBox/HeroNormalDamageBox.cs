namespace EW2
{
    public class HeroNormalDamageBox : NormalDamageBox<HeroBase>
    {
        protected override bool CanGetDamage(Unit target)
        {
            return isAoe || owner.SearchTarget.target.Value == target;
        }
    }

}
