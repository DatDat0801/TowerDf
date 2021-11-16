using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class MoneyHeroRoomController : MonoBehaviour,IUpdateTabBarChanged
    {
        [SerializeField] private  List<GameObject> moneyInSkillTab;
        [SerializeField] private List<GameObject> moneyInRuneTab;
        [SerializeField] private List<GameObject> moneyInSpellTab;
        private List<GameObject> lastMoneyShow = new List<GameObject>();
        

        private void Start()
        {
            lastMoneyShow = moneyInSkillTab;
        }

        public void OnTabBarChanged(int indexActive)
        {
            foreach (var money in this.lastMoneyShow)
            {
                money.SetActive(false);
            }

            switch (indexActive)
            {
                case 0:
                    foreach (var money in this.moneyInSkillTab)
                    {
                        money.SetActive(true);
                    }
                    lastMoneyShow = this.moneyInSkillTab;
                    break;
                case 1:
                    foreach (var money in this.moneyInSpellTab)
                    {
                        money.SetActive(true);
                    }
                    lastMoneyShow = this.moneyInSpellTab;
                    break;
                case 2:
                    foreach (var money in this.moneyInRuneTab)
                    {
                        money.SetActive(true);
                    }
                    lastMoneyShow = this.moneyInRuneTab;
                    break;
            }
        }
    }
}
