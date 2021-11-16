namespace EW2
{
    public class Enemy3004 : EnemyBase
    {
        public EnemyNormalDamageBox normalAttackBox;
        
        public override EnemyStatBase EnemyData
        {
            get
            {
                if (enemyData == null)
                {
                    enemyData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3004>().GetStats(Level);
                }

                return enemyData;
            }
        }

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Enemy3004Spine(this));
    }
}