using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;
using Event = Spine.Event;

namespace EW2
{
    public abstract class UnitSpine : UnitAnimation
    {
        protected Unit owner;

        private SkeletonAnimation skeletonAnimation;

        private Skeleton skeleton;

        private AnimationState animationState;

        public SkeletonAnimation SkeletonAnimation
        {
            get => skeletonAnimation;
            protected set
            {
                skeletonAnimation = value;

                skeleton = skeletonAnimation.skeleton;

                animationState = skeletonAnimation.AnimationState;
                animationState.Start += StartAnimation;
                animationState.Event += HandleEvent;
                animationState.Complete += CompleteAnimation;
                animationState.End += EndAnimation;
            }
        }


        public Skeleton Skeleton => skeleton;
        public AnimationState AnimationState => animationState;

        protected UnitSpine(Unit owner)
        {
            this.owner = owner;

            this.SkeletonAnimation = owner.Transform.GetComponent<SkeletonAnimation>();

            this.OnValidate();
        }

        public void OnValidate()
        {
            if (SkeletonAnimation == null)
                throw new System.NotImplementedException("Non-SkeletonAnimation");

            if (Skeleton == null)
                throw new System.NotImplementedException("Non-Skeleton");

            if (AnimationState == null)
                throw new System.NotImplementedException("Non-AnimationState");
        }

        protected TrackEntry SetAnimation(int trackIndex, string animationName, bool loop)
        {
            return AnimationState.SetAnimation(trackIndex, animationName, loop);
        }

        protected TrackEntry AddAnimation(int trackIndex, string animationName, bool loop, float delay)
        {
            return AnimationState.AddAnimation(trackIndex, animationName, loop, delay);
        }

        public virtual void HandleEvent(TrackEntry trackEntry, Event e)
        {
            //print("Event fired! " + e.Data.Name);
            // do something
        }

        protected virtual void StartAnimation(TrackEntry trackEntry)
        {
        }

        protected virtual void EndAnimation(TrackEntry trackEntry)
        {
        }

        protected virtual void CompleteAnimation(TrackEntry trackEntry)
        {
        }

        public void Dispose()
        {
            skeletonAnimation = null;

            skeleton = null;

            animationState = null;
        }

        public virtual void SetSkinSpine(string nameSkin)
        {
            try
            {
                skeleton?.SetSkin(nameSkin);
                skeleton?.SetSlotsToSetupPose();
                animationState?.Apply(skeletonAnimation.Skeleton);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool IsPlayingAnimation(string animationName)
        {
            return AnimationState.GetCurrent(0).Animation.Name.Equals(animationName);
        }

        public void ClearTracks() => AnimationState.ClearTracks();
        public void ClearTrack(int trackIndex) => AnimationState.ClearTrack(trackIndex);

        public void SetToSetupPose() => Skeleton.SetToSetupPose();

        public string CalculateCurrentAnimationName(int trackIndex)
        {
            if (AnimationState.GetCurrent(trackIndex) != null)
            {
                return AnimationState.GetCurrent(trackIndex).Animation.Name;
            }

            return "";
        }
        
        public TrackEntry SetTrackEntryTimeScaleBySpeed(ref TrackEntry trackEntry)
        {
            var attackSpeed = owner.Stats.GetStat<AttackSpeed>(RPGStatType.AttackSpeed).StatValue;
            if (attackSpeed > 1)
            {
                trackEntry.TimeScale = attackSpeed;
            }
            else
            {
                trackEntry.TimeScale = 1;
            }

            return trackEntry;
        }
    }
}