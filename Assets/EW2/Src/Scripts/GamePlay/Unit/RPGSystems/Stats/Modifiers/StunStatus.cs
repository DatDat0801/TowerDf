using UnityEngine.Events;

namespace EW2
{
    public class StunStatus : StatusOverTime
    {
        public StunStatus(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Stun;

            Stacks = false;
        }

        public override void UpdateValue()
        {
            //Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            //throw new System.NotImplementedException();
            //config.owner.UnitState.Set(ActionState.Stun);
        }

        public override void Prepare()
        {
            if (config.owner.UnitState.Current != ActionState.UseSkill)
            {
                var stun = new StunAction(config.owner);
                config.owner.UnitState.Set(ActionState.Stun);
                config.owner.UnitState.IsLockState = true;
                stun.Execute();
            }
        }

        public override void Complete()
        {
            if (config.owner.UnitState.Current != ActionState.UseSkill)
            {
                UnitState state = config.owner.UnitState;
                state.IsLockState = false;

                state.Set(state.PreviousState);

                if (state.PreviousState == ActionState.Move)
                {
                    config.owner.UnitSpine.Move();
                }
                else if (state.Current == ActionState.Idle && config.owner is HeroBase)
                {
                    var regenHp = ((HeroBase) config.owner).RegenHp;
                    if (regenHp.IsExecuting)
                    {
                        regenHp.Stop();
                    }

                    regenHp.Execute();
                }
            }
        }

        public override string ToString()
        {
            return $"{statusType} => {TimeLife}";
        }
    }
}
