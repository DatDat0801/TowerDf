using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class TerrifyStatus : StatusOverTime
    {
        private Vector3 _posTarget;
        private readonly EnemyBase _enemyTarget;
        private float _speed;

        public TerrifyStatus(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Terrify;

            Stacks = false;

            if (this.config.owner is EnemyBase)
            {
                this._enemyTarget = (EnemyBase)this.config.owner;
            }
        }

        public override void UpdateValue()
        {
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            if (this.config.owner.StatusController.CanDoActionCCStatus(StatusType.Terrify))
            {
                if (this._enemyTarget && this._enemyTarget.UnitState.Current != ActionState.Terrify)
                {
                    this._enemyTarget.UnitState.Set(ActionState.Terrify);
                    this._enemyTarget.MoveToStartPoint();
                    this._enemyTarget.EnableMoveToStartPoint(true);
                }
            }
        }

        public override void Prepare()
        {
        }

        public override void Complete()
        {
            if (this._enemyTarget)
            {
                this._enemyTarget.EnableMoveToStartPoint(false);
                this._enemyTarget.CheckActionAfterStatus();
            }
            else
            {
                config.owner.UnitState.Set(config.owner.UnitState.PreviousState);
            }
        }

        public override string ToString()
        {
            return $"{statusType} => {TimeLife}";
        }
    }
}