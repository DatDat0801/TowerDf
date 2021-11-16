using Zitga.Observables;

namespace EW2
{
    /// <summary>
    /// The base class for all other Stats.
    /// </summary>
    public class RPGStat : ObservableObject {
        /// <summary>
        /// Used by the StatBase Value Property
        /// </summary>
        protected float statBaseValue;

        /// <summary>
        /// The Total Value of the stat
        /// </summary>
        public virtual float StatValue => StatBaseValue;

        /// <summary>
        /// The Base Value of the stat
        /// </summary>
        public virtual float StatBaseValue
        {
            get => statBaseValue;
            set => Set(ref statBaseValue, value);
        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        public RPGStat() {
            statBaseValue = 0;
        }
    }
}

