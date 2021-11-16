namespace EW2
{
    public interface IBuffHandler
    {
        IBuffHandler SetNext(IBuffHandler handler);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffItem"></param>
        /// <param name="heroid"></param>
        /// <param name="stackNumber"></param>
        void Buff(IBuffItem buffItem, int heroid, int stackNumber);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffItem"></param>
        void Buff(IBuffItem buffItem);
    }
}
