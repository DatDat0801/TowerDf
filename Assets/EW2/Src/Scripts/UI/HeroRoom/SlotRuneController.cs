using System;
using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class SlotRuneController : MonoBehaviour
    {
        [SerializeField] protected Image bgr;

        [SerializeField] protected Image border;

        [SerializeField] private Image iconSymbolRune;

        [SerializeField] private Image iconRune;

        [SerializeField] private GameObject runePanel;

        [SerializeField] private GameObject lockPanel;

        [SerializeField] private GameObject iconLock;

        [SerializeField] private GameObject iconUnlock;

        private Button button;
        private HeroItem heroData;
        private int slotId;
        private int runeId;
        private int heroSelected;
        private Action onClick;

        private IRuneInventory Inventory { get; set; }

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            button.onClick.AddListener(RuneClick);
        }

        public void InitData(int slotId, int heroId, IRuneInventory inventory)
        {
            this.slotId = slotId;
            this.runeId = -1;
            this.heroData = UserData.Instance.UserHeroData.GetHeroById(heroId);
            this.heroSelected = heroId;
            Inventory = inventory;
            if (heroData != null)
            {
                this.runeId = heroData.GetRuneBySlot(this.slotId);
            }

            ShowUi();
        }

        public void SetCallbackClick(Action cbClick)
        {
            this.onClick = cbClick;
        }

        private void ShowUi()
        {
            iconLock.SetActive(IconLockEnable());
            iconUnlock.SetActive(!iconLock.activeSelf);
            if (this.runeId >= 0)
            {
                var runeData = (RuneItem) UserData.Instance.GetInventory(InventoryType.Rune, this.runeId);
                if (runeData != null)
                {
                    var dataCompare = InventoryDataBase.GetRuneId(runeData.RuneIdConvert);
                    bgr.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{dataCompare.Item2}");
                    border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{dataCompare.Item2}_rect");
                    iconSymbolRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_{dataCompare.Item1}");
                    iconRune.sprite = ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{dataCompare.Item2}");
                    runePanel.SetActive(true);
                    lockPanel.SetActive(false);
                }
            }
            else
            {
                runePanel.SetActive(false);
                lockPanel.SetActive(true);
            }
        }

        private void RuneClick()
        {
            if (!UnlockFeatureUtilities.IsRuneAvailable())
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    UnlockFeatureUtilities.RUNE_AVAILABLE_AT_STAGE_ID + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            if (!UserData.Instance.UserHeroData.CheckHeroUnlocked(heroSelected))
            {
                Ultilities.ShowToastNoti(L.popup.unlock_hero_to_equip_rune);
                return;
            }

            if (runeId < 0)
            {
                onClick?.Invoke();
            }
            else
            {
                var runeData = (RuneItem) UserData.Instance.GetInventory(InventoryType.Rune, this.runeId);
                var data = new RuneInfoWindowProperties(runeData, (RuneTab) Inventory, heroData.heroId);
                UIFrame.Instance.OpenWindow(ScreenIds.rune_inventory_info, data);
            }
        }

        private bool IconLockEnable()
        {
            var heroUnlocked = UserData.Instance.UserHeroData.CheckHeroUnlocked(heroSelected);
            return !heroUnlocked || !UnlockFeatureUtilities.IsRuneAvailable();
        }
    }
}
