namespace EW2
{
    public class Skill1Enemy3012DamageBox : EnemyNormalDamageBox
    {
        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                return null;
            }

            var enemy3012 = (Enemy3012)owner;
            target.StatusController.AddStatus(enemy3012.Skill1.CalculateBleedStatus(target));
            return base.GetDamage(target);
        }
    }
}