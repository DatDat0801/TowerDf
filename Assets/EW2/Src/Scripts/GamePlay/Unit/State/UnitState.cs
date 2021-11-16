using System;

namespace EW2
{
    public enum ActionState
    {
        None,
        Idle,
        Move,
        Appear,
        AttackMelee,
        AttackRange,
        UseSkill,
        Die,
        Reborn,
        Invisible,
        Visible,
        Stun,
        WaitToAction,
        CheckCounter,
        SkillPassive1,
        SkillPassive2,
        SkillPassive3,
        SkillPassive4,
        Rally,
        Skill1,
        Skill2,
        Turn,
        Terrify,
        GetHurt
    }

    public class UnitState
    {
        protected Unit owner;

        protected UnitState(Unit owner)
        {
            this.owner = owner;

            Current = ActionState.None;
        }

        public ActionState Current { get; private set; }
        /// <summary>
        /// this is last <see cref="ActionState"/> "Type" of Unit
        /// </summary>
        public ActionState PreviousState { get; private set; }
        public bool IsLockState { get; set; }


        public Action<ActionState> StartSetState { get; set; }

        public void Set(ActionState state)
        {
            if (IsLockState)
            {
                return;
            }
            if (Current != state)
            {
                PreviousState = Current;
                this.Current = state;
            }
            StartSetState?.Invoke(state);
        }

        public bool CanControl()
        {
            if (Current == ActionState.Stun || Current == ActionState.Die || Current == ActionState.UseSkill ||
                Current == ActionState.Turn || Current == ActionState.Terrify)
            {
                return false;
            }

            return true;
        }

        public bool CanUseSkill()
        {
            if (Current == ActionState.Stun || Current == ActionState.Die || Current == ActionState.Terrify)
            {
                return false;
            }

            return true;
        }

        public bool IsStatusCC()
        {
            if (Current == ActionState.Stun || Current == ActionState.Terrify)
            {
                return true;
            }

            return false;
        }
    }
}
