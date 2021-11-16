using Spine;
using UnityEngine;

namespace EW2
{
    public class StunAction : UnitAction
    {
        public StunAction(Unit owner) : base(owner)
        {
            
        }
        
        protected override TrackEntry DoAnimation()
        {
            Debug.Log("Play anim Stun");
            
            //owner.UnitState.Set(ActionState.Stun);
            
            return owner.UnitSpine.Stun();
        }
    }
}