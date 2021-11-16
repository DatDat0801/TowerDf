using Zitga.Observables;

namespace EW2
{
    public abstract class RPGStatModifierAction : ObservableObject
    {
        protected readonly RPGStatModifier owner;

        protected RPGStatModifierAction(RPGStatModifier owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Do action when be called
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Cancel running action and remove stat from the list 
        /// </summary>
        public abstract void Stop();
    }
}