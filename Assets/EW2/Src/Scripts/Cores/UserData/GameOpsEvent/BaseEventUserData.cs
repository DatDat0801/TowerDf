namespace EW2
{
    public class BaseEventUserData
    {
        public long timeEndEvent;
        
        public bool isOpen;
        
        public void SetTimeEnd(long timeEnd)
        {
            this.timeEndEvent = timeEnd;
        }

        public virtual long TimeRemain()
        {
            var timeCurrent = TimeManager.NowInSeconds;
            return timeEndEvent - timeCurrent;
        }

        public virtual bool CheckCanShow()
        {
            return false;
        }
    }
}