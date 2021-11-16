using UnityEngine.Events;

namespace EW2
{
    public class EnemyFreezeOverTime : StatusOverTime
    {
        private readonly EnemyBase _enemyTarget;

        private float _cachedTimescale;
        //private float _cachedMoveSpeed;

        private RPGStatModifier _moveSpeedModifier;
        public UnityAction OnComplete { get; set; }

        public EnemyFreezeOverTime(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Freeze;

            Stacks = false;
            if (this.config.owner is EnemyBase @base)
            {
                this._enemyTarget = @base;
            }
        }

        public override void UpdateValue()
        {
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            this._enemyTarget.UnitState.Set(ActionState.Stun);
            this._cachedTimescale = this._enemyTarget.UnitSpine.AnimationState.TimeScale;
            this._enemyTarget.UnitSpine.AnimationState.TimeScale = 0;
            var moveSpeedModifiable = Ultilities.GetStatModifiable(RPGStatType.MoveSpeed);
            this._moveSpeedModifier = new RPGStatModifier(moveSpeedModifiable, ModifierType.TotalPercent, -1f, false);
            this._enemyTarget.Stats.AddStatModifier(RPGStatType.MoveSpeed, this._moveSpeedModifier);
            this._enemyTarget.onDead = OnEnemyDead;
        }

        private void OnEnemyDead(Unit victim, Unit killer)
        {
            this._enemyTarget.UnitSpine.AnimationState.TimeScale = this._cachedTimescale;
        }

        public override void Prepare()
        {
            //Remove cold status if exist
            this._enemyTarget.StatusController.RemoveStatus(StatusType.Cold);
        }

        public override void Complete()
        {
            UnitState state = config.owner.UnitState;
            state.IsLockState = false;
            state.Set(state.PreviousState);

            this._enemyTarget.UnitSpine.AnimationState.TimeScale = this._cachedTimescale;
            //this._enemyTarget.GetSingleMove().Speed = this._cachedMoveSpeed;
            //this._enemyTarget.GetMultiMove().Resume();
            this._enemyTarget.Stats.RemoveStatModifier(RPGStatType.MoveSpeed, this._moveSpeedModifier, true);
            OnComplete?.Invoke();
        }

        public override void Remove()
        {
            base.Remove();
            this._enemyTarget.UnitSpine.AnimationState.TimeScale = this._cachedTimescale;
            //this._enemyTarget.GetSingleMove().Speed = this._cachedMoveSpeed;
            //this._enemyTarget.GetMultiMove().Resume();
            this._enemyTarget.Stats.RemoveStatModifier(RPGStatType.MoveSpeed, this._moveSpeedModifier, true);
            UnitState state = config.owner.UnitState;
            state.IsLockState = false;
            state.Set(state.PreviousState);
        }
    }
}