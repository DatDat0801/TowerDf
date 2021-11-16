using System.Collections.Generic;
using UnityEngine;
using System;

namespace Invoke
{
    public class InvokeCallbackUtils : Singleton<InvokeCallbackUtils>, IInvoke
    {
        #region Attributes

        private Dictionary<object, List<CallbackAction>> invokes;
        private List<ManageInvokeCommand> commands;

        #endregion

        #region Unity Events

        void Update()
        {
            if(invokes == null || commands == null) return;
            ManageInvokeList();
            UpdateInvokes(Time.deltaTime);
        }

        #endregion

        #region Init

        public void Init()
        {
            invokes = new Dictionary<object, List<CallbackAction>>();
            commands = new List<ManageInvokeCommand>();
        }

        #endregion

        #region Manage Callback Actions

        private void ManageInvokeList()
        {
            if(commands == null) return;
            
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].DoCommand(invokes);
            }

            commands.Clear();
        }

        private void UpdateInvokes(float deltaTime)
        {
            foreach (var kvp in invokes)
            {
                if (kvp.Key != null)
                {
                    var callbacks = kvp.Value;
                    for (int i = callbacks.Count - 1; i > -1; i--)
                    {
                        if (callbacks[i].Update(deltaTime))
                        {
                            callbacks.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    CancelInvoke(kvp.Key);
                }
            }
        }

        #endregion

        #region IDelayCallback implementation

        public void CancelInvoke(object caller)
        {
            if (caller != null)
            {
                commands.Add(new CancelAllCommand(caller));
            }
        }

        public void CancelInvoke(object caller, Action callback)
        {
            if (caller != null && callback != null)
            {
                commands.Add(new CancelOneCommand(caller, callback));
            }
        }

        public void Invoke(object caller, Action callback, float delay)
        {
            if (caller != null && callback != null)
            {
                if (delay <= 0)
                {
                    try
                    {
                        callback();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    return;
                }

                CallbackAction action = new DoOnceAction(caller, callback, delay);
                commands.Add(new AddCommand(action));
            }
        }
        
        public void InvokeRepeating(object caller, Action callback, float delay, float interval)
        {
            if (caller != null && callback != null)
            {
                CallbackAction action = new RepeatAction(caller, callback, delay, interval);
                commands.Add(new AddCommand(action));
            }
        }

        #endregion
    }
}