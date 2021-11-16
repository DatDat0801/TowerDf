using System;
using Spine;
using UnityEngine;

namespace EW2
{
    public class CavalryReadySpecialAction
    {
        protected Cavalry6050Controller owner;

        protected float totalTime;

        protected float timeTriggerAttack;

        protected string animationName;

        public Action<string> onComplete;

        public CavalryReadySpecialAction(Cavalry6050Controller owner)
        {
            this.owner = owner;
        }

        public void ResetTime(string animName)
        {
            this.animationName = animName;

            timeTriggerAttack = SpineUtils.GetAnimationTime(owner.UnitSpine.AnimationState, this.animationName);

            totalTime = timeTriggerAttack;
        }

        public void Execute(float deltaTime)
        {
            totalTime += deltaTime;

            if (totalTime < timeTriggerAttack) return;

            var trackEntry = ((Cavalry6050Spine) owner.UnitSpine).ReadySpecial(this.animationName);

            totalTime = 0;

            trackEntry.Complete += OnComplete;
        }


        public void OnComplete(TrackEntry track)
        {
            onComplete?.Invoke(this.animationName);

            track.Complete -= OnComplete;
        }
    }
}