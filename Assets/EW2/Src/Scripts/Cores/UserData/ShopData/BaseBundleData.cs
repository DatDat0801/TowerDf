using System;
using UnityEditor;

namespace EW2
{
    [Serializable]
    public class BaseBundleData
    {
        public long timeStart;

        public long timeEnd;

        public bool isOpen;

        private bool isFirstOpen;

        public void SetTimeEnd(long timeEnd)
        {
            this.timeEnd = timeEnd;
        }

        public void SetTimeStart(long timeStart)
        {
            this.timeStart = timeStart;
        }

        public virtual long TimeRemain()
        {
            var timeCurrent = TimeManager.NowInSeconds;
            return timeEnd - timeCurrent;
        }

        public void SetFirstOpen()
        {
            isFirstOpen = true;
        }

        public bool GetFirstOpen()
        {
            return isFirstOpen;
        }

        public virtual bool CheckCanShow()
        {
            return false;
        }
    }
}