using System.Collections.Generic;

namespace EW2
{
    public interface IRuneInventory
    {
        List<RuneItem> SelectedRuneItems { get; set; }

        /// <summary>
        /// Remove an item from the inventory
        /// </summary>
        /// <returns></returns>
        void RemoveFromInventory(RuneItem item);

        /// <summary>
        /// Add item from the inventory
        /// </summary>
        /// <param name="item"></param>
        RuneItem AddIntoInventory(RuneItem item);
    }
}