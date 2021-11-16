using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class HeroRuneInfoController : MonoBehaviour
    {
        [SerializeField] private List<SlotRuneController> listSlotRunes;
        [SerializeField] private RuneTab inventory;
        public void ShowInfoRune(int heroId, Action slotClick)
        {
            for (int i = 0; i < listSlotRunes.Count; i++)
            {
                listSlotRunes[i].InitData(i, heroId, inventory);
                listSlotRunes[i].SetCallbackClick(slotClick);
            }
        }
    }
}