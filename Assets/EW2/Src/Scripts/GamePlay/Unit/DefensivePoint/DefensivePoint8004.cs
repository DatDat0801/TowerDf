using System;

namespace EW2
{
    public class DefensivePoint8004 : DefensivePointBase
    {
        
        public DefensivePoint8004Data DefensivePointData
        {
            get
            {
                if (this.defensivePointData == null)
                {
                    this.defensivePointData = GameContainer.Instance.Get<UnitDataBase>().Get<DefensivePoint8004Data>();
                }

                return this.defensivePointData as DefensivePoint8004Data;
            }
        }
        
        public DefensivePointStatBase StatBase => DefensivePointData.stats[0];
        public override RPGStatCollection Stats => stats ?? (stats = new DefensivePointStats(this, StatBase));

        public DefensivePoint8004Passive PassiveData => DefensivePointData.passiveStats[0];

        private void Start()
        {
            Invoke(nameof(HealHp), 3f);
        }

        public void HealHp()
        {
            RegenHp.Execute();
        }
        private RegenHpOverTime _regenHp;

        public RegenHpOverTime RegenHp
        {
            get
            {
                if (this._regenHp == null)
                    InitRegenHp();
                return this._regenHp;
            }
        }
        protected void InitRegenHp()
        {
            //var healthPoint = Stats.GetStat<HealthPoint>(RPGStatType.Health);

            //healthPoint.CurrentValue = healthPoint.StatValue;
            //hpMax = healthPoint.StatValue;
            this._regenHp = new RegenHpOverTime(new StatusOverTimeConfig() {
                creator = this,
                owner = this,
                lifeTime = float.MaxValue,
                intervalTime = PassiveData.internalTime,
                delayTime = 0,
                baseValue = PassiveData.increaseHp
            }) {Stacks = true};
        }
    }
}