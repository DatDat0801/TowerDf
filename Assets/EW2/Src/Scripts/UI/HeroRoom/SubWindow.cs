using UnityEngine;

namespace EW2
{
    public interface ISubWindowProperties
    {
        
    }

    public class SubWindowProperties : ISubWindowProperties
    {
        
    }

    public abstract class SubWindow : SubWindow<SubWindowProperties>
    {
    }
    public abstract class SubWindow<TProps>  : MonoBehaviour  where TProps : ISubWindowProperties
    {
        protected virtual int TabIndex { get; }
        //public abstract bool IsOpen { get; protected set; }
        
        public abstract void Open(int tabIndex);

        public abstract void Close(int tabIndex);
    }
}