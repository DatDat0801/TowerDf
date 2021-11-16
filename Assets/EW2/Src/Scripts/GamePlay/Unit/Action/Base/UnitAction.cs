using System;
using Spine;

namespace EW2
{
    public abstract class UnitAction
    {
        protected readonly Unit owner;

        public Action onComplete;

        public UnitAction(Unit owner)
        {
            this.owner = owner;
        }

        public void Execute()
        {
            var trackEntry = DoAnimation();

            void OnComplete(TrackEntry track)
            {
                onComplete?.Invoke();

                track.Complete -= OnComplete;
            }

            if (trackEntry!=null)
            {
                trackEntry.Complete += OnComplete;
            }
           
        }

        protected abstract TrackEntry DoAnimation();
    }
}