using UnityEngine;
using System.Collections.Generic;
using Zitga.Observables;

namespace EW2
{
    public enum ModifierType
    {
        BaseAdd = 0,
        BasePercent = 1,
        TotalAdd = 2,
        TotalPercent = 3,
    }


    /// <summary>
    /// The base class for all RPGStatModifiers
    /// </summary>
    public class RPGStatModifier : ObservableObject
    {
        /// <summary>
        /// Variable used for the Value property
        /// </summary>
        private float value = 0f;

        /// <summary>
        /// Variable used for the Stacks property
        /// </summary>
        private bool stacks = true;

        /// <summary>
        /// object who affect to
        /// </summary>
        protected Unit owner;

        /// <summary>
        /// object who create to stat
        /// </summary>
        protected Unit creator;

        /// <summary>
        /// 
        /// </summary>
        protected RPGStatModifiable stat;

        public List<RPGStatModifierAction> Actions { get; }

        /// <summary>
        /// The order in which the modifier is applied to the stat
        /// </summary>
        public ModifierType Order { get; }

        /// <summary>
        /// The value of the modifier that is combined with other
        /// modifiers of the same stat then is passed to ApplyModifier
        /// method to determine the final modifier value to apply to the stat
        /// 
        /// Triggers the OnValueChange event
        /// </summary>
        public float Value
        {
            get => value;
            private set
            {
                if (this.value != value)
                {
                    Set(ref this.value, value);
                }
            }
        }

        /// <summary>
        /// Does the modifier's value stat with other modifiers of the 
        /// same type. If value is false, the value of the single modifier will be used
        /// if the sum of stacking modifiers is not greater then the not statcking mod.
        /// </summary>
        public bool Stacks
        {
            get => stacks;
            private set => stacks = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public RPGStatModifiable Stat
        {
            get => stat;
            set => stat = value;
        }


        /// <summary>
        /// Constructs a Stat Modifier with the given value and stack value
        /// </summary>
        public RPGStatModifier(RPGStatModifiable stat, ModifierType order, float value, bool stacks = true,
            Unit creator = null, Unit owner = null)
        {
            Order = order;
            Value = value;
            Stacks = stacks;

            this.Stat = stat;
            this.creator = creator;
            this.owner = owner;

            Actions = new List<RPGStatModifierAction>();
        }

        public void AddAction(RPGStatModifierAction action)
        {
            if (Actions.Contains(action) == false)
            {
                this.Actions.Add(action);
            }
            else
            {
                Debug.Log("Active is exist");
            }
        }

        public void RemoveAction(RPGStatModifierAction action)
        {
            var result = Actions.Remove(action);
            if (result == false)
            {
                Debug.LogError("Action has been remove");
            }
        }

        public void CancelAllActions()
        {
            foreach (var rpgStatModifierAction in Actions)
            {
                rpgStatModifierAction.Stop();
            }

            Actions.Clear();
        }

        public virtual void Remove()
        {
            Stat.RemoveModifier(this);

            CancelAllActions();
        }

        public override string ToString()
        {
            if (creator && owner)
                return $"Modifier Stat: {creator.name} to {owner.name} value<{Value}, stack{stacks}>";

            return $"Modifier Stat: value<{Value}, stack{stacks}>";
        }
    }
}