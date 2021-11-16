using Spine;

namespace EW2
{
    public class AttackMelee : Attack
    {
        public float Range { get; set; }

        public AttackMelee(Unit owner, float range = 0.6f) : base(owner)
        {
            animationName = owner.UnitSpine.attackMeleeName;

            this.Range = range;
            
            Init();
        }

        protected override TrackEntry DoAnimation()
        {
            return owner.UnitSpine.AttackMelee();
        }
    }
}