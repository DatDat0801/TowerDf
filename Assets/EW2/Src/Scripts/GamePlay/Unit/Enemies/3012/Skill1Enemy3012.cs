namespace EW2
{
    public class Skill1Enemy3012 : SkillEnemy
    {
        private EnemyData3012.EnemyData3012Skill1 skill1Data;
        private Enemy3012 enemy;


        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            skill1Data = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3012>().GetSkill1(enemyBase.Level);
            enemy = (Enemy3012) enemyBase;
        }


        public BleedStatus CalculateBleedStatus(Unit target)
        {
            var bleedStatus = new BleedStatus(new StatusOverTimeConfig()
            {
                creator = enemy,
                owner = target,
                lifeTime = skill1Data.secondApply,
                baseValue = skill1Data.hpLost,
            })
            {
                Stacks = false
            };

            return bleedStatus;
        }
    }
}