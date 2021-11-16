using UnityEngine.Events;

namespace EW2
{
    public class SilentStatus : StatusOverTime
    {
        public SilentStatus(StatusOverTimeConfig config) : base(config)
        {
            statusType = StatusType.Silent;
            
            Stacks = false;
        }

        public override void UpdateValue()
        {
            //Value = BaseValue;
        }

        protected override void DoStatus(UnityAction callback = null)
        {
            //throw new System.NotImplementedException();
        }

        public override void Prepare()
        {
            // if (config.owner.UnitState.Current == ActionState.Move)
            // {
            //
            // }
            
        }

        public override void Complete()
        {
            // do something
            config.owner.StatusController.RemoveStatus(this);
        }

        public override string ToString()
        {
            return $"{statusType} => {TimeLife}";
        }
    }
}