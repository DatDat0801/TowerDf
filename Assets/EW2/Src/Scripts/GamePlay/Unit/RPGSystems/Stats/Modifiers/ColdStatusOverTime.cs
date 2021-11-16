using UnityEngine.Events;

namespace EW2
{
    public class ColdStatusOverTime : StatusOverTime
    {
        protected Dummy target;
        protected float slowDownPercent;

        private RPGStatModifier _attackSpeedModifier;
        private RPGStatModifier _moveSpeedModifier;

        public ColdStatusOverTime(StatusOverTimeConfig config, float slowDownPercent) : base(config)
        {
            statusType = StatusType.Cold;

            Stacks = false;
            if (this.config.owner is Dummy @base)
            {
                this.target = @base;
            }

            this.slowDownPercent = slowDownPercent;
        }

        public override void UpdateValue()
        {
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            if(this.target.StatusController.ExistStatus(StatusType.Freeze)) return;
            
            var attackSpeedModifiable = Ultilities.GetStatModifiable(RPGStatType.AttackSpeed);
            this._attackSpeedModifier = new RPGStatModifier(attackSpeedModifiable, ModifierType.TotalPercent,
                -this.slowDownPercent, false);
            this.target.Stats.AddStatModifier(RPGStatType.AttackSpeed, this._attackSpeedModifier);

            var moveSpeedModifiable = Ultilities.GetStatModifiable(RPGStatType.MoveSpeed);
            this._moveSpeedModifier = new RPGStatModifier(moveSpeedModifiable, ModifierType.TotalPercent,
                -this.slowDownPercent, false);
            this.target.Stats.AddStatModifier(RPGStatType.MoveSpeed, this._moveSpeedModifier);

            
            this.target.SetColdOverrideMaterial();
            this.target.onDead = OnEnemyDead;
        }

        private void OnEnemyDead(Unit victim, Unit killer)
        {
            Complete();
        }

        public override void Prepare()
        {
        }

        public override void Complete()
        {
            if (this._attackSpeedModifier!=null)
            {
                this.target.Stats.RemoveStatModifier(RPGStatType.AttackSpeed, this._attackSpeedModifier, true);
            }

            if (this._moveSpeedModifier != null)
            {
                this.target.Stats.RemoveStatModifier(RPGStatType.MoveSpeed, this._moveSpeedModifier, true);
            }

            this.target.RemoveColdOverrideMaterial();
        }

        public override void Remove()
        {
            base.Remove();
            Complete();
        }
    }
}