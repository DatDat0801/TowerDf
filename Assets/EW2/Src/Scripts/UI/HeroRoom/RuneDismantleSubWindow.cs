using System;
using System.Collections.Generic;
using Hellmade.Sound;
using Newtonsoft.Json;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class RuneDismantleSubWindow : SubWindow, IRuneInventory
    {
        [SerializeField] private GameObject[] leftUIs;
        [SerializeField] private GameObject dismantle;

        [SerializeField] private Button closeButton;
        [SerializeField] private Button fastSelectButton;
        [SerializeField] private Button dismantleButton;
        [SerializeField] private Text runeExpText;
        [SerializeField] private Text selectedRuneText;


        [SerializeField] private GameObject runeItemHolder;
        [SerializeField] private RuneItemUI runeItemUiPrefab;
        [SerializeField] private Transform itemContainer;

        [SerializeField] private RuneTab mainInventory;

        long TotalExpRune = 0;
        public static bool IsInventoryOpen { get; private set; }

        private List<RuneItem> m_RuneItems;

        public List<RuneItem> SelectedRuneItems
        {
            get
            {
                if (m_RuneItems == null)
                {
                    m_RuneItems = new List<RuneItem>();
                }

                return m_RuneItems;
            }
            set => m_RuneItems = value;
        }

        protected override int TabIndex
        {
            get => RuneTab.RUNE_TAB_INDEX;
        }

        public int ItemCount
        {
            get => SelectedRuneItems.Count;
        }


        public const int DISMANTLE_INVENTORY_SIZE = 20;

        /// <summary>
        /// Open the rune dismantle sub tab
        /// </summary>
        /// <param name="tab">tab=-1, ignore tab index</param>
        public override void Open(int tab)
        {
            if (tab != TabIndex) return;
            foreach (GameObject leftUI in leftUIs)
            {
                leftUI.SetActive(false);
            }

            mainInventory.RepaintInventoryWithUnequippedItemsOnly();
            RefreshInventory();
            dismantle.SetActive(true);
            RepaintText();
            IsInventoryOpen = true;
        }

        /// <summary>
        /// Close the rune dismantle sub tab
        /// </summary>
        /// <param name="tab">tab=-1, ignore tab index</param>
        public override void Close(int tab)
        {
            if (tab == TabIndex) return;
            foreach (GameObject leftUI in leftUIs)
            {
                leftUI.SetActive(true);
            }

            SelectedRuneItems.Clear();

            dismantle.SetActive(false);
            IsInventoryOpen = false;

            mainInventory.ResetInventory();
        }

        void OnCloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            Close(-1);
        }

        bool IsAskToday()
        {
            try
            {
                var json = PlayerPrefs.GetString(PopupNoticeDontAskAgainTodayWindowController.DISMANTLE_DONT_ASK_TODAY);
                var ask = JsonConvert.DeserializeObject<DontAskToday>(json);
                return ask.lastAskTime.Date.Equals(TimeManager.NowUtc.Date);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                PlayerPrefs.DeleteKey(PopupNoticeDontAskAgainTodayWindowController.DISMANTLE_DONT_ASK_TODAY);
                return false;
            }
        }

        public void RepaintText()
        {
            var dismantleDb = GameContainer.Instance.GetRuneDismantleDatabase();
            TotalExpRune = dismantleDb.GetTotalExp(SelectedRuneItems);
            runeExpText.text = $"{TotalExpRune}";
            var selectedRune = SelectedRuneItems.Count;
            selectedRuneText.text = $"{selectedRune}/{DISMANTLE_INVENTORY_SIZE}";
        }

        void FastSelectClick()
        {
            if (IsFull())
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.dismantle_slots_full_txt);
                return;
            }

            mainInventory.QuickSelect();
        }

        void DoDismantle()
        {
            if (IsEmpty())
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.dismantle_warning_txt_4);
                return;
            }

            for (var i = 0; i < SelectedRuneItems.Count; i++)
            {
                mainInventory.RemoveFromInventory(SelectedRuneItems[i]);
            }

            var reward = new Reward()
            {
                id = MoneyType.ExpRune,
                type = ResourceType.Money,
                number = (int) TotalExpRune,
                itemType = InventoryType.None
            };
            PopupUtils.ShowReward(reward);
            //Add to user data
            Reward.AddToUserData(new[] { reward }, AnalyticsConstants.SourceDismantleRune, "", false);
            SelectedRuneItems.Clear();
            RefreshInventory();
            RepaintText();
            mainInventory.RepaintInventoryWithUnequippedItemsOnly();
        }

        bool IsHaveHighRarity()
        {
            return SelectedRuneItems.Exists(item => item.Rarity >= 3);
        }

        void DismantleClick()
        {
            if (IsHaveHighRarity())
            {
                if (IsAskToday())
                {
                    //dismantle here
                    DoDismantle();
                }
                else
                {
                    var property = new PopupNoticeWindowProperties(L.popup.warning_txt,
                        $"{L.popup.dismantle_warning_txt_3}",
                        PopupNoticeWindowProperties.PopupType.OneOption, L.button.btn_continue, DoDismantle,
                        L.button.btn_no, null, false);
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_dont_ask_again_today, property);
                }
            }
            else
            {
                DoDismantle();
            }
        }

        #region MonobehaviorMethod

        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseClick);
            fastSelectButton.onClick.AddListener(FastSelectClick);
            dismantleButton.onClick.AddListener(DismantleClick);
        }

        private void OnDisable()
        {
            if (IsInventoryOpen)
                Close(-1);
        }

        #endregion

        public RuneItem AddIntoInventory(RuneItem item)
        {
            SelectedRuneItems.Add(item);
            return item;
        }

        public void RemoveFromInventory(RuneItem item)
        {
            SelectedRuneItems.Remove(item);
        }

        public bool IsFull()
        {
            return SelectedRuneItems.Count >= DISMANTLE_INVENTORY_SIZE;
        }

        public bool IsEmpty()
        {
            return SelectedRuneItems.Count <= 0;
        }

        public void RefreshInventory()
        {
            foreach (Transform t in itemContainer)
            {
                Destroy(t.gameObject);
            }

            for (int i = 0; i < DISMANTLE_INVENTORY_SIZE; i++)
            {
                if (i < SelectedRuneItems.Count)
                {
                    var runeGameObject = Instantiate(runeItemUiPrefab, itemContainer);
                    runeGameObject.Repaint(SelectedRuneItems[i], 0, this, true);
                }
                else
                {
                    Instantiate(runeItemHolder, itemContainer);
                }
            }

            RepaintText();
        }

        public bool Contains(RuneItem item)
        {
            return SelectedRuneItems.Contains(item);
        }
    }
}
