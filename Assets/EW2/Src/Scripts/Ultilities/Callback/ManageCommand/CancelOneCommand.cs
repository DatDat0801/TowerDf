using System;
using System.Collections.Generic;
using UnityEngine;

namespace Invoke
{
    public class CancelOneCommand : ManageInvokeCommand
    {
        private object caller;
        private Action callback;

        public CancelOneCommand(object caller, Action callback)
        {
            this.caller = caller;
            this.callback = callback;
        }

        public override ManageInvokeCommand DoCommand(Dictionary<object, List<CallbackAction>> invokes)
        {
            List<CallbackAction> callbacks;
            if (invokes.TryGetValue(caller, out callbacks))
            {
                for (int i = callbacks.Count - 1; i > -1; i--)
                {
                    if (callbacks[i].callback == callback)
                    {
                        //Log.Error(string.Format("[CANCEL ONE][{0}] Hash1={1}, Hash2={2}, Name1={3}, Name2={4}",
                        //    TimeManager.Instance.NowInSeconds, callback.GetHashCode(), callbacks[i].callback.GetHashCode(),
                        //    StringUtils.FlattenInvocationList(callback), StringUtils.FlattenInvocationList(callbacks[i].callback)));
                        callbacks.RemoveAt(i);
                    }
                }
            }

            return this;
        }
    }
}
