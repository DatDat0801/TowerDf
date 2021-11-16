using System;
using Spine;

namespace EW2
{
    public static class SpineUtils
    {
        public static float GetAnimationTime(AnimationState state, string animationName)
        {
            var animation =state.Data.SkeletonData.FindAnimation(animationName);
            if (animation != null)
            {
                return animation.Duration;
            }
            else
            {
                throw new Exception("Can't find animation: " + animationName);
            }
        }
    }
}