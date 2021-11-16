namespace EW2
{
    public class Hero1001TakeDamageCalculation : TakeDamageCalculation
    {
        private readonly float reduceDamage;
        
        public Hero1001TakeDamageCalculation(Unit owner, float reduceDamage) : base(owner)
        {
            this.reduceDamage = reduceDamage;
        }

        public override float Calculate(DamageInfo info)
        {
            var damage = base.Calculate(info);
            
            return (info.damageType == DamageType.Physical) ? damage * (1 - reduceDamage) : damage;
        }
    }
}