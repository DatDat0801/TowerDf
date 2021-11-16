using UnityEngine;

namespace EW2
{
    public class BuffHandler : IBuffHandler
    {
        protected IBuffHandler _nextHandler;

        public IBuffHandler SetNext(IBuffHandler handler)
        {
            this._nextHandler = handler;
            return handler;
        }

        public virtual void Buff(IBuffItem buffItem, int heroid = 0, int stackNumber = 1)
        {
            // if (this._nextHandler != null)
            //     this._nextHandler.Buff(buffItem);
            Debug.Log("base handler");
        }

        public virtual void Buff(IBuffItem buffItem)
        {

        }
    }
}
