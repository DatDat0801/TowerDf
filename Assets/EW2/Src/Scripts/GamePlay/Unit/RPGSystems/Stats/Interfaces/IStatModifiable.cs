
namespace EW2
{
    /// <summary>
    /// Allows the stat to use modifiers
    /// </summary>
    public interface IStatModifiable {
        float StatModifierValue { get; }

        void AddModifier(RPGStatModifier mod);
        void RemoveModifier(RPGStatModifier mod);
        void ClearModifiers();
        void UpdateModifiers();
    }
}
