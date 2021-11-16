using Zitga.Observables;

namespace EW2
{
    /// <summary>
    /// RPGStat that inherits from RPGAttribute and implement IStatCurrentValueChange
    /// </summary>
    public class RPGVital : RPGAttribute {
        /// <summary>
        /// Used by the StatCurrentValue Property
        /// </summary>
        private readonly ObservableProperty<float> statCurrentValue;

        /// <summary>
        /// The current value of the stat. Restricted between the values 0 
        /// and StatValue. When set will trigger the OnCurrentValueChange event.
        /// </summary>
        public ObservableProperty<float> StatCurrentValue {
            get {
                if (statCurrentValue > StatValue) {
                    statCurrentValue.Value = StatValue;
                } else if (statCurrentValue < 0) {
                    statCurrentValue.Value = 0;
                }
                return statCurrentValue;
            }
            set => statCurrentValue.Value = value;
        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        public RPGVital() {
            statCurrentValue = new ObservableProperty<float>(0);
        }

        /// <summary>
        /// Sets the StatCurrentValue to StatValue
        /// </summary>
        public void SetCurrentValueToMax() {
            StatCurrentValue = StatValue;
        }
    }

}
