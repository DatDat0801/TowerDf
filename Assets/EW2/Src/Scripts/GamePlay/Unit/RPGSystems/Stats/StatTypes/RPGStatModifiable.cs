using UnityEngine;
using System.Collections.Generic;

namespace EW2
{
    /// <summary>
    /// A RPGStat Type that implements both IStatModifiable and IStatValueChange
    /// </summary>
    public class RPGStatModifiable : RPGStat, IStatModifiable
    {
        /// <summary>
        /// List of RPGStatModifiers applied to the stat
        /// </summary>
        private readonly List<RPGStatModifier> statMods;

        /// <summary>
        /// Used by the StatModifierValue Property
        /// </summary>
        private float statModValue;

        /// <summary>
        /// The stat's total value including StatModifiers
        /// </summary>
        public override float StatValue => StatModifierValue;

        public override float StatBaseValue
        {
            get => statBaseValue;
            set
            {
                Set(ref statBaseValue, value);

                UpdateModifiers();
            }
        }

        /// <summary>
        /// The total value of the stat modifiers applied to the stat
        /// </summary>
        public float StatModifierValue
        {
            get => statModValue;
            set => Set(ref statModValue, value);
        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        public RPGStatModifiable()
        {
            StatModifierValue = 0;

            statMods = new List<RPGStatModifier>();
        }

        /// <summary>
        /// Adds Modifier to stat and listens to the mod's value change event
        /// </summary>
        public void AddModifier(RPGStatModifier mod)
        {
            if (mod.Stacks == false)
            {
                var modList = statMods.FindAll(x => x.Order == mod.Order);
                foreach (var rpgStatModifier in modList)
                {
                    rpgStatModifier.Remove();
                }
            }

            statMods.Add(mod);

            //Debug.Log($"Add({statMods.Count}) => " + mod);

            mod.PropertyChanged += OnModValueChange;

            OnModValueChange(null, null);
        }

        /// <summary>
        /// Removes modifier from stat and stops listening to value change event
        /// </summary>
        public void RemoveModifier(RPGStatModifier mod)
        {
            statMods.Remove(mod);

            Debug.Log($"Remove({statMods.Count}) => " + mod);

            mod.PropertyChanged -= OnModValueChange;

            OnModValueChange(null, null);
        }

        /// <summary>
        /// Removes all modifiers from the stat and stops listening to the value change event
        /// </summary>
        public virtual void ClearModifiers()
        {
            foreach (var mod in statMods)
            {
                mod.PropertyChanged -= OnModValueChange;
            }

            statMods.Clear();
            
            OnModValueChange(null, null);
        }

        /// <summary>
        /// Updates the StatModifierValue based of the currently applied modifier values
        /// </summary>
        public void UpdateModifiers()
        {
            float baseAdd = 0, basePercent = 0, totalAdd = 0, totalPercent = 0;
            foreach (var rpgStatModifier in statMods)
            {
                switch (rpgStatModifier.Order)
                {
                    case ModifierType.BaseAdd:
                        baseAdd += rpgStatModifier.Value;
                        break;
                    case ModifierType.BasePercent:
                        basePercent += rpgStatModifier.Value;
                        break;
                    case ModifierType.TotalAdd:
                        totalAdd += rpgStatModifier.Value;
                        break;
                    case ModifierType.TotalPercent:
                        totalPercent += rpgStatModifier.Value;
                        break;
                }
            }

            StatModifierValue = (StatBaseValue * (1 + basePercent) + baseAdd) * (1 + totalPercent) + totalAdd;
        }

        /// <summary>
        /// Used to listen to the applied stat modifier OnValueChange events
        /// </summary>
        private void OnModValueChange(object modifier, System.EventArgs args)
        {
            UpdateModifiers();
        }
    }
}