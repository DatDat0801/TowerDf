namespace EW2
{
    /// <summary>
    /// RPGStat that inherits from RPGStatModifiable and implements IStatScalable and IStatLinkable.
    /// </summary>
    public class RPGAttribute : RPGStatModifiable, IStatScalable {
        /// <summary>
        /// Used By StatLevelValue Property
        /// </summary>
        private int statLevelValue;

        /// <summary>
        /// The value gained by the ScaledStat method
        /// </summary>
        public int StatLevelValue
        {
            get => statLevelValue;
            set => Set(ref statLevelValue, value);
        }

        /// <summary>
        /// Gets the stat base value with the StatLevelValue added
        /// </summary>
        public override float StatBaseValue => base.StatBaseValue + StatLevelValue;

        /// <summary>
        /// Overridable method that scales the class based off the passed value
        /// Triggers the stat's Value Change event
        /// </summary>
        public virtual void ScaleStat(int level) {
            StatLevelValue = level;
        }    
    }
}

