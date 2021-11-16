using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace EW2
{
    public class GroupItemSpells : EnhancedScrollerCellView
    {
        [SerializeField] private List<ItemSpellUi> listItemSpells;

        public void SetData(List<SpellItem> groupItemSpells, int heroId)
        {
            for (int i = 0; i < listItemSpells.Count; i++)
            {
                if (i > groupItemSpells.Count - 1)
                {
                    listItemSpells[i].gameObject.SetActive(false);
                    continue;
                }

                listItemSpells[i].InitSpell(groupItemSpells[i],heroId);
                listItemSpells[i].gameObject.SetActive(true);
            }
        }
    }
}