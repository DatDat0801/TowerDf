using System;
using UnityEngine;

namespace Invoke
{
    public class DoOnceAction : CallbackAction
    {
        private float currentTime;

        public DoOnceAction(object caller, Action callback, float delay) : base(caller, callback)
        {
            this.currentTime = delay;
        }

        public override bool Update(float delta)
        {
            if (callback == null || caller == null)
            {
                return true;
            }

            currentTime -= delta;
            if (currentTime <= 0)
            {
                try
                {
                    //Log.Warning(string.Format("[Once][{0}] Hash={1}, Name={2}",
                    //           TimeManager.Instance.NowInSeconds, callback.GetHashCode(), StringUtils.FlattenInvocationList(callback)));
                    callback();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
