using System;
using System.Collections.Generic;
using UnityEngine;

namespace Invoke
{
    public class AddCommand : ManageInvokeCommand
    {
        private CallbackAction action;

        public AddCommand(CallbackAction action)
        {
            this.action = action;
        }

        public override ManageInvokeCommand DoCommand(Dictionary<object, List<CallbackAction>> invokes)
        {
            List<CallbackAction> callbacks;
            if (!invokes.TryGetValue(action.caller, out callbacks))
            {
                callbacks = new List<CallbackAction>();
                invokes.Add(action.caller, callbacks);
            }
            else
            {
                // Remove existing actions
                for (int i = callbacks.Count - 1; i > -1; i--)
                {
                    if (callbacks[i].callback == action.callback)
                    {
                        //Log.Error(string.Format("[ADD][{0}] Hash1={1}, Hash2={2}, Name1={3}, Name2={4}",
                        //    TimeManager.Instance.NowInSeconds, action.callback.GetHashCode(), callbacks[i].callback.GetHashCode(), 
                        //    StringUtils.FlattenInvocationList(action.callback), StringUtils.FlattenInvocationList(callbacks[i].callback)));
                        callbacks.RemoveAt(i);
                    }
                }
            }

            callbacks.Add(action);
            return this;
        }
    }
}
