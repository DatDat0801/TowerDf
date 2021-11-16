using Spine;

namespace EW2
{
    public class AttackRange1004 : AttackRange
    {
        private Unit _human1004;

        public AttackRange1004(Unit owner, Unit human) : base(owner)
        {
            animationName = owner.UnitSpine.attackRangeName;
            this._human1004 = human;
            Init();
        }

        protected override TrackEntry DoAnimation()
        {
            if (this._human1004 && this._human1004.IsAlive &&
                this._human1004.UnitState.Current != ActionState.SkillPassive2)
            {
                if (this._human1004.UnitState.Current != ActionState.AttackRange)
                    this._human1004.UnitState.Set(ActionState.AttackRange);

                this._human1004.UnitSpine.AttackRange();
            }

            return owner.UnitSpine.AttackRange();
        }
    }
}