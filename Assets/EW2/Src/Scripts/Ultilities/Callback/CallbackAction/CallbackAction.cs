using System;
using UnityEngine;

namespace Invoke
{
    public abstract class CallbackAction
    {
        public object caller { get; private set; }
        public Action callback { get; private set; }

        public CallbackAction(object caller, Action callback)
        {
            this.caller = caller;
            this.callback = callback;
        }

        /// <summary>
        /// Return: need remove from invoke list or not
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public abstract bool Update(float deltaTime);
    }
}
