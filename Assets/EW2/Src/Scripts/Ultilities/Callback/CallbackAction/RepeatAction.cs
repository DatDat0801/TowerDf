using System;
using UnityEngine;

namespace Invoke
{
    public class RepeatAction : CallbackAction
    {
        private float currentTime;
        private float interval;

        public RepeatAction(object caller, Action callback, float delay, float interval) : base(caller, callback)
        {
            this.currentTime = delay;
            this.interval = interval;
        }

        public override bool Update(float delta)
        {
            if (callback == null)
            {
                return true;
            }

            currentTime -= delta;
            if (currentTime <= 0)
            {
                currentTime = interval;
                try
                {
                    //Log.Warning(string.Format("[Repeat][{0}] Hash={1}, Name={2}",
                    //        TimeManager.Instance.NowInSeconds, callback.GetHashCode(), StringUtils.FlattenInvocationList(callback)));
                    callback();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return false;
        }
    }
}
