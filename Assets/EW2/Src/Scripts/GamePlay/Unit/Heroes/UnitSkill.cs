using Invoke;
using Zitga.Observables;

namespace EW2
{
    public abstract class UnitSkill : ObservableObject
    {
        protected Unit owner;

        protected int level;

        public int Level
        {
            get => level;
            set => Set<int>(ref level, value);
        }

        public UnitSkill(Unit owner)
        {
            this.owner = owner;
        }

        public abstract void Init();

        public abstract void Execute();

        public virtual void Remove()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
        }
    }
}