using System;
using System.Collections.Generic;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;
using Event = Spine.Event;

namespace EW2
{
    public class AnimationEventUnit
    {
        public Action<TrackEntry> CompleteAnimation { get; set; }
        public Action<TrackEntry,Event> HandleEvent { get; set; }
        public Action<TrackEntry> EndAnimation { get; set; }
        public Action<TrackEntry> StartAnimation { get; set; }
        
        private bool isInitAnimationEvent;
        
        public void InitAnimationEvents(SkeletonAnimation skeletonAnimation)
        {
            if (isInitAnimationEvent)
            {
                return;
            }
            var animationState = skeletonAnimation.AnimationState;
            isInitAnimationEvent = true;
            RegisterComplete(animationState);
            RegisterStart(animationState);
            RegisterEnd(animationState);
            RegisterHandlerEvent(animationState);
        }

        private void RegisterComplete(AnimationState animationState)
        {
            if (CompleteAnimation==null)
            {
                return;
            }
            animationState.Complete += CompleteAnimation.Invoke;
        }
        
        private void RegisterStart(AnimationState animationState)
        {
            if (StartAnimation==null)
            {
                return;
            }
            animationState.Start += StartAnimation.Invoke;
        }
        
        private void RegisterEnd(AnimationState animationState)
        {
            if (EndAnimation==null)
            {
                return;
            }
            animationState.End += EndAnimation.Invoke;
        }
        
        private void RegisterHandlerEvent(AnimationState animationState)
        {
            if (HandleEvent==null)
            {
                return;
            }
            animationState.Event += HandleEvent.Invoke;
        }
    }
}