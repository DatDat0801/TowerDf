using System;
using System.Collections.Generic;
using UnityEngine;

namespace Invoke
{
    public class CancelAllCommand : ManageInvokeCommand
    {
        private object caller;

        public CancelAllCommand(object caller)
        {
            this.caller = caller;
        }

        public override ManageInvokeCommand DoCommand(Dictionary<object, List<CallbackAction>> invokes)
        {
            List<CallbackAction> callbacks;
            if (invokes.TryGetValue(caller, out callbacks))
            {
                //Log.Error(string.Format("[CANCEL ALL][{0}] Hash={1}, Name={2}", TimeManager.Instance.NowInSeconds, caller.GetHashCode(), caller.name));
                callbacks.Clear();
            }

            return this;
        }
    }
}
