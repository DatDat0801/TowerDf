namespace EW2
{
    public class Enemy3001 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox;

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3001Spine(this));
    }
}