using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using EW2.Spell;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class HeroSpellController : TabContainer, IEnhancedScrollerDelegate
    {
        [SerializeField] private Text titleNumbSpell;
        [SerializeField] private Text txtNumbSpell;
        [SerializeField] private Text txtLabelSort;
        [SerializeField] private Button btnSort;
        [SerializeField] private float itemSize;
        [SerializeField] private EnhancedScrollerCellView cellView;
        [SerializeField] private EnhancedScroller scroller;

        private List<SpellItem> listSpells = new List<SpellItem>();
        private List<List<SpellItem>> listSpellsGroup = new List<List<SpellItem>>();
        //private bool isSortByLevel;
        private SortType _sortType;
        private int numberSpell;
        private int currHeroId;

        private void Awake()
        {
            btnSort.onClick.AddListener(SortDataClick);
            EventManager.StartListening(GamePlayEvent.OnRefreshSpell, () => {
                GetData();
                GroupData(SortData());
                scroller.ReloadData();
                ShowInfo();
            });
        }

        public void SetCurrHeroId(int heroId)
        {
            currHeroId = heroId;
        }

        private void SortDataClick()
        {
            //isSortByLevel = !isSortByLevel;
            _sortType = _sortType == SortType.ByLevel ? SortType.ByRarity : SortType.ByLevel;
            GroupData(SortData());
            scroller.ReloadData();
            ShowInfo();
        }

        private void GetData()
        {
            listSpells.Clear();
            listSpells.AddRange(UserData.Instance.GetListSpell());
            GroupData(SortData());
            var spellData = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>();
            numberSpell = spellData.spellDataBases.Length;
        }

        private void GroupData(List<SpellItem> listDatas)
        {
            listSpellsGroup.Clear();
            if (listDatas.Count <= 0)
                return;

            var count = 0;
            var numberSplit = listDatas.Count / 4;

            numberSplit += 1;

            for (int i = 0; i < numberSplit; i++)
            {
                var listSpellGroups = new List<SpellItem>();
                if (count < listDatas.Count)
                    listSpellGroups.Add(listDatas[count]);
                if (count + 1 < listDatas.Count)
                    listSpellGroups.Add(listDatas[count + 1]);

                if (count + 2 < listDatas.Count)
                    listSpellGroups.Add(listDatas[count + 2]);

                if (count + 3 < listDatas.Count)
                    listSpellGroups.Add(listDatas[count + 3]);

                listSpellsGroup.Add(listSpellGroups);

                count += listSpellGroups.Count;
            }
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            GetData();
            InitScroll();
            ShowInfo();
            SortData();
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        private void ShowInfo()
        {
            if (titleNumbSpell)
            {
                this.titleNumbSpell.text = L.stages.powerup_items;
            }
            if(txtNumbSpell)
                txtNumbSpell.text = $"{listSpells.Count}/{numberSpell}";
            if(this._sortType == SortType.ByLevel)
                txtLabelSort.text = L.button.sort_by_level;
            else
                txtLabelSort.text = L.button.Sort_by_rarity;
        }

        private List<SpellItem> SortData()
        {
            var dataSort = new List<SpellItem>();

            if (this._sortType == SortType.ByRarity)
            {
                dataSort = listSpells.OrderByDescending(spell => spell.Rarity).ToList();
            }
            else
            {
                dataSort = listSpells.OrderByDescending(spell => spell.Rarity).ThenByDescending(spell => spell.Level)
                    .ToList();
                //dataSort = dataSort.OrderByDescending(spell => spell.Level).ToList();
            }

            return dataSort;
        }

        #region Scroller event

        private void InitScroll()
        {
            if (scroller.Delegate == null)
                scroller.Delegate = this;
            else
                scroller.ReloadData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if (listSpells == null)
                return 0;

            return listSpellsGroup.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return itemSize;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var groupItemSpells = scroller.GetCellView(cellView) as GroupItemSpells;

            groupItemSpells.dataIndex = dataIndex;

            var data = listSpellsGroup[dataIndex];

            groupItemSpells.SetData(data, currHeroId);

            return groupItemSpells;
        }

        #endregion
    }
}