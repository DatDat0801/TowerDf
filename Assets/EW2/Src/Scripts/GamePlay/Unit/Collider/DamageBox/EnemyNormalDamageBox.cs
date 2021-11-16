namespace EW2
{
    public class EnemyNormalDamageBox : NormalDamageBox<EnemyBase>
    {
        
        protected override bool CanGetDamage(Unit target)
        {
            return isAoe || owner.target == target;
        }
        
    }

}
