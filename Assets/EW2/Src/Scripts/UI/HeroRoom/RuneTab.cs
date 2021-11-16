using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public enum SortType
    {
        ByLevel,
        ByEquippedHero,
        ByUnequippedHero,
        ByRarity
    }

    public class RuneTab : TabContainer, IEnhancedScrollerDelegate, IRuneInventory
    {
        [SerializeField] private Text runeQuantityText;
        [SerializeField] private Button sortButton;
        [SerializeField] private Button dismantleButton;
        [SerializeField] private Text labelSortText;
        [SerializeField] private EnhancedScrollerCellView cellView;
        [SerializeField] private EnhancedScroller container;
        [SerializeField] private RuneDismantleSubWindow dismantleWindow;

        private const int ITEM_SIZE = 168;
        public const int RUNE_TAB_INDEX = 1;

        private List<RuneItem> runeItems = new List<RuneItem>();

        //private List<RuneItem> equippedRuneItems = new List<RuneItem>();
        private List<List<RuneItem>> listSpellsGroup = new List<List<RuneItem>>();
        private int currHeroId;

        private SortType m_SortType;

        public SortType SortType
        {
            get { return m_SortType; }
            private set { m_SortType = value; }
        }


        private List<RuneItem> m_SelectedRuneItems;

        public List<RuneItem> SelectedRuneItems
        {
            get
            {
                if (m_SelectedRuneItems == null)
                {
                    m_SelectedRuneItems = new List<RuneItem>();
                }

                return m_SelectedRuneItems;
            }
            set { m_SelectedRuneItems = value; }
        }


        #region MonoBehaviorMethod

        private void Awake()
        {
            sortButton.onClick.AddListener(Sort);
            dismantleButton.onClick.AddListener(DismantleButtonClick);
            SortType = 0;
        }

        #endregion

        private void DismantleButtonClick()
        {
            dismantleWindow.Open(RUNE_TAB_INDEX);
            Repaint();
        }

        public void Sort()
        {
            if (RuneDismantleSubWindow.IsInventoryOpen)
            {
                //Disable sort by equipped unequipped in rune dismantle window
                if (SortType < SortType.ByRarity)
                {
                    SortType += 3;
                }
                else
                {
                    SortType = 0;
                }
            }
            else
            {
                if (SortType < SortType.ByRarity)
                {
                    SortType += 1;
                }
                else
                {
                    SortType = 0;
                }
            }
            RepaintSortLabel();
            RefreshGroupData(SortData());
            container.ReloadData();
        }

        void RepaintSortLabel()
        {
            switch (SortType)
            {
                case SortType.ByLevel:
                    labelSortText.text = L.button.sort_by_level;
                    break;
                case SortType.ByEquippedHero:
                    labelSortText.text = L.popup.in_use_txt;
                    break;
                case SortType.ByUnequippedHero:
                    labelSortText.text = L.popup.not_in_use_txt;
                    break;
                case SortType.ByRarity:
                    labelSortText.text = L.button.Sort_by_rarity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        void InitViewModelData()
        {
            runeItems.Clear();
            runeItems.AddRange(UserData.Instance.GetListRune());
            RefreshGroupData(SortData());
        }

        private List<RuneItem> SortData()
        {
            var dataSort = new List<RuneItem>();

            switch (SortType)
            {
                case SortType.ByLevel:
                    dataSort = runeItems.OrderByDescending(spell => spell.Level).ThenByDescending(spell => spell.Rarity).ToList();
                    break;
                case SortType.ByEquippedHero:
                    dataSort = runeItems.OrderByDescending(spell => spell.HeroIdEquip > 0)
                        .ThenByDescending(item => item.Rarity).ThenByDescending(item => item.Level).ToList();
                    break;
                case SortType.ByUnequippedHero:
                    dataSort = runeItems.OrderByDescending(spell => spell.HeroIdEquip == 0)
                        .ThenByDescending(item => item.Rarity).ThenByDescending(item => item.Level).ToList();
                    break;
                case SortType.ByRarity:
                    dataSort = runeItems.OrderByDescending(spell => spell.Rarity).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return dataSort;
        }

        public void SetCurrHeroId(int heroId)
        {
            currHeroId = heroId;
        }

        public void Repaint()
        {
            runeQuantityText.text = $"<color=#745f43>{runeItems.Count.ToString()}</color>/<size=28>{ITEM_SIZE}</size>";
            
            dismantleButton.gameObject.SetActive(!RuneDismantleSubWindow.IsInventoryOpen);
        }

        public override void ShowContainer()
        {
            gameObject.SetActive(true);
            InitViewModelData();
            InitScroll();
            Repaint();
            RepaintSortLabel();
        }

        private void InitScroll()
        {
            if (container.Delegate == null)
                container.Delegate = this;
            else
                container.ReloadData();
        }

        public override void HideContainer()
        {
            gameObject.SetActive(false);
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if (runeItems == null)
                return 0;

            return listSpellsGroup.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return ITEM_SIZE;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var groupItemSpells = scroller.GetCellView(cellView) as GroupRuneItemUI;

            groupItemSpells.dataIndex = dataIndex;

            var data = listSpellsGroup[dataIndex];

            groupItemSpells.SetData(data, currHeroId, this);

            return groupItemSpells;
        }

        public void QuickSelect()
        {
            var sortedItemsByRarity = runeItems.OrderBy(item => item.Rarity).Where(item => item.IsEquipped() == false)
                .ToList();
            if (sortedItemsByRarity.Count == dismantleWindow.ItemCount)
            {
                return;
            }

            for (var i = 0; i < sortedItemsByRarity.Count; i++)
            {
                if (dismantleWindow.IsFull() == false && dismantleWindow.Contains(sortedItemsByRarity[i]) == false)
                {
                    dismantleWindow.AddIntoInventory(sortedItemsByRarity[i]);
                    SelectedRuneItems.Add(sortedItemsByRarity[i]);
                }
            }

            dismantleWindow.RefreshInventory();
            RepaintInventoryWithUnequippedItemsOnly();
        }

        public bool IsSelectRune(RuneItem item)
        {
            return SelectedRuneItems.Contains(item);
        }

        public void RemoveFromInventory(RuneItem item)
        {
            UserData.Instance.RemoveRune(item, "", "");
            //ShowContainer();
        }

        public RuneItem AddIntoInventory(RuneItem item)
        {
            UserData.Instance.UpdateInventory(InventoryType.Rune, item, "", false);
            //ShowContainer();
            return item;
        }

        public void ResetInventory()
        {
            SelectedRuneItems.Clear();
            ShowContainer();
            Repaint();
        }

        public bool ToggleModifyItem(RuneItem item)
        {
            if (IsSelectRune(item))
            {
                SelectedRuneItems.Remove(item);
                dismantleWindow.RemoveFromInventory(item);
                dismantleWindow.RefreshInventory();
                return false;
            }
            else
            {
                if (dismantleWindow.IsFull())
                {
                    EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.dismantle_slots_full_txt);
                    return false;
                }

                SelectedRuneItems.Add(item);
                dismantleWindow.AddIntoInventory(item);
                dismantleWindow.RefreshInventory();
                return true;
            }
        }

        /// <summary>
        /// for dismantle function
        /// </summary>
        public void RepaintInventoryWithUnequippedItemsOnly()
        {
            runeItems.Clear();
            runeItems.AddRange(UserData.Instance.GetListRune());

            runeItems.RemoveAll(item => item.IsEquipped());
            //Repaint with unequipped items
            RefreshGroupData(SortData());
            InitScroll();
            Repaint();
            labelSortText.text = L.button.sort_by_level;
        }


        private void RefreshGroupData(List<RuneItem> listDatas)
        {
            listSpellsGroup.Clear();
            if (listDatas.Count <= 0)
                return;

            var count = 0;
            var numberSplit = listDatas.Count / 4;

            numberSplit += 1;

            for (int i = 0; i < numberSplit; i++)
            {
                var listSpellGroups = new List<RuneItem>();
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
    }
}