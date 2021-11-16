using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2
{
    public class GroupRuneItemUI : EnhancedScrollerCellView
    {
        [SerializeField] private List<RuneItemUI> listItemSpells;

        public void SetData(List<RuneItem> groupItemSpells, int heroId, IRuneInventory inventory)
        {
            for (int i = 0; i < listItemSpells.Count; i++)
            {
                if (i > groupItemSpells.Count - 1)
                {
                    listItemSpells[i].gameObject.SetActive(false);
                    continue;
                }

                listItemSpells[i].Repaint(groupItemSpells[i], heroId, inventory);
                listItemSpells[i].gameObject.SetActive(true);
            }
        }

        public void RemoveItem()
        {
            
        }
    }
}