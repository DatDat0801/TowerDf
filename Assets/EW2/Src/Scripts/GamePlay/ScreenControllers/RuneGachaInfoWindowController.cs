using System;
using System.Collections.Generic;
using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class RuneGachaInfoProperty : WindowProperties
    {
        public SummonType summonType;

        public RuneGachaInfoProperty(SummonType type)
        {
            this.summonType = type;
        }
    }

    public class RuneGachaInfoWindowController : AWindowController<RuneGachaInfoProperty>
    {
        private const int NummberRuneType = 12;

        [SerializeField] private Text title;
        [SerializeField] private Text tapToClose;
        [SerializeField] private GameObject itemRunePrefab;
        [SerializeField] private Transform groupRune;
        [SerializeField] private SpellRarityRateInfo[] listRarity;

        private GachaRuneData gachaRuneData;
        private List<int> listRarityIdShow = new List<int>();
        private List<float> listRate = new List<float>();

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            if (Properties.summonType == SummonType.Normal)
                title.text = L.popup.normal_rune_rate;
            else
                title.text = L.popup.premium_rune_rate;

            tapToClose.text = L.popup.tap_to_close;
            GetData();
            ShowInfoRarity();
            ShowItemSpell();
        }

        private void GetData()
        {
            if (gachaRuneData == null)
                gachaRuneData = GameContainer.Instance.Get<GachaDataBase>().Get<GachaRuneData>();

            listRarityIdShow.Clear();
            listRate.Clear();
            foreach (var rarity in gachaRuneData.GetDataGachaRate(Properties.summonType))
            {
                if (rarity.rate > 0)
                {
                    listRarityIdShow.Add(rarity.rarity);
                    listRate.Add(rarity.rate);
                }
            }
        }

        private void ShowInfoRarity()
        {
            for (int i = 0; i < listRarity.Length; i++)
            {
                if (i >= listRarityIdShow.Count)
                {
                    listRarity[i].gameObject.SetActive(false);
                    continue;
                }

                listRarity[i].SetInfo(listRarityIdShow[i], listRate[i]);
                listRarity[i].gameObject.SetActive(true);
            }
        }

        private void ShowItemSpell()
        {
            foreach (Transform child in groupRune)
            {
                Destroy(child.gameObject);
            }

            var runeIdConvert = 0;

            foreach (var rarity in listRarityIdShow)
            {
                for (int i = 0; i < NummberRuneType; i++)
                {
                    var go = Instantiate(itemRunePrefab, groupRune);
                    if (go != null)
                    {
                        runeIdConvert = InventoryDataBase.GetRuneIdConvert(i, rarity);
                        var control = go.GetComponent<RarityRuneItemController>();
                        if (control)
                        {
                            control.SetData(GameContainer.Instance.Get<InventoryDataBase>().GetRuneData(runeIdConvert));
                            go.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}