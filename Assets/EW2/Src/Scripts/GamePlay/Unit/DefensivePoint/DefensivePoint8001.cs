namespace EW2
{
    public class DefensivePoint8001 : DefensivePointBase
    {
        public DefensivePointData DefensivePointData
        {
            get
            {
                if (this.defensivePointData == null)
                {
                    this.defensivePointData = GameContainer.Instance.Get<UnitDataBase>().Get<DefensivePoint8001Data>();
                }

                return this.defensivePointData;
            }
        }
        
        public DefensivePointStatBase StatBase => DefensivePointData.stats[0];
        public override RPGStatCollection Stats => stats ?? (stats = new DefensivePointStats(this, StatBase));

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void InitAction()
        {
            base.InitAction();
        }

    }
}