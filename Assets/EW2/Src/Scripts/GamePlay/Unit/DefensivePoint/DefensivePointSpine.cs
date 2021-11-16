using Spine;

namespace EW2
{
    public class DefensivePointSpine : UnitSpine
    {
        private readonly string _getAttackName;
        public DefensivePointSpine(Unit owner) : base(owner)
        {
            this.dieName = "destruct";
            this._getAttackName = "get_hurt";
            this.idleName = "idle";
        }
        
        public override TrackEntry Die()
        {
            return SetAnimation(0, this.dieName, false);
        }

        public override TrackEntry Idle()
        {
            return SetAnimation(0, this.idleName, false);
        }

        public override TrackEntry GetHurt()
        {
            return SetAnimation(0, this._getAttackName, false);
        }
    }
}