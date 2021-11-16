using System;
using System.Collections.Generic;
using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class SpellGachaInfoProperty : WindowProperties
    {
        public SummonType summonType;

        public SpellGachaInfoProperty(SummonType type)
        {
            this.summonType = type;
        }
    }

    public class SpellGachaInfoWindowController : AWindowController<SpellGachaInfoProperty>
    {
        [SerializeField] private Text title;
        [SerializeField] private Text tapToClose;
        [SerializeField] private GameObject itemSpellPrefab;
        [SerializeField] private Transform groupSpell;
        [SerializeField] private SpellRarityRateInfo[] listRarity;
        [SerializeField] private ScrollRect scrollRect;

        private GachaSpellNormal _spellNormalData;
        private GachaSpellPremium _spellPremiumData;
        private List<int> listRarityIdShow = new List<int>();

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            if (Properties.summonType == SummonType.Normal)
                title.text = L.popup.normal_spell_rate;
            else
                title.text = L.popup.premium_spell_rate;
            tapToClose.text = L.popup.tap_to_close;
            GetData();
            ShowInfoRarity();
            ShowItemSpell();
            scrollRect.content.anchoredPosition = Vector2.zero;
        }

        private void GetData()
        {
            if (Properties.summonType == SummonType.Normal)
            {
                listRarityIdShow.Clear();
                this._spellNormalData = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellNormal>();
                foreach (var rarity in this._spellNormalData.gachaSpellRateDatas)
                {
                    if (rarity.rate > 0)
                        listRarityIdShow.Add(rarity.rarity);
                }
            }
            else
            {
                listRarityIdShow.Clear();
                this._spellPremiumData = GameContainer.Instance.Get<GachaDataBase>().Get<GachaSpellPremium>();
                foreach (var rarity in this._spellPremiumData.gachaSpellRateDatas)
                {
                    if (rarity.rate > 0)
                        listRarityIdShow.Add(rarity.rarity);
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

                if (Properties.summonType == SummonType.Normal)
                    listRarity[i].SetInfo(listRarityIdShow[i],
                        this._spellNormalData.gachaSpellRateDatas[listRarityIdShow[i]].rate);
                else
                    listRarity[i].SetInfo(listRarityIdShow[i],
                        this._spellPremiumData.gachaSpellRateDatas[listRarityIdShow[i]].rate);

                listRarity[i].gameObject.SetActive(true);
            }
        }

        private void ShowItemSpell()
        {
            foreach (Transform child in groupSpell)
            {
                Destroy(child.gameObject);
            }

            var spellDataBases = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();

            foreach (var spell in spellDataBases.spellDataBases)
            {
                if (listRarityIdShow.Contains(spell.spellStatDatas.rarity))
                {
                    var go = Instantiate(itemSpellPrefab, groupSpell);
                    if (go != null)
                    {
                        var control = go.GetComponent<RaritySpellItemController>();
                        if (control)
                        {
                            control.SetData(spell.spellStatDatas);
                            go.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}