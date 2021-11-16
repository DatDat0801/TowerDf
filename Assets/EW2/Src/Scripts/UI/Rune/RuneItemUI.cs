using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class RuneItemUI : MonoBehaviour
    {
        [SerializeField] private Image runeIcon;
        [SerializeField] private Image runeTypeIcon;
        [SerializeField] private Image frame;
        [SerializeField] private Image background;
        [SerializeField] private Button runeButton;
        [SerializeField] private Text txtLevel;
        [SerializeField] private Image equippedHero;
        [SerializeField] private GameObject selectedSign;

        public IRuneInventory Inventory { get; private set; }
        public RuneItem RuneItem { get; private set; }
        private int heroId;

        private void Awake()
        {
            runeButton.onClick.AddListener(RuneItemClick);
        }

        public void Repaint(RuneItem runeItem, int currHero, IRuneInventory inventory, bool showLevel = true)
        {
            RuneItem = runeItem;
            heroId = currHero;
            Inventory = inventory;

            var dataCompare = InventoryDataBase.GetRuneId(runeItem.RuneIdConvert);
            if (runeIcon != null)
                runeIcon.overrideSprite =
                    ResourceUtils.GetSpriteAtlas("rune", $"icon_rune_{dataCompare.Item2.ToString()}");
            if (frame != null)
                frame.sprite =
                    ResourceUtils.GetSpriteAtlas("border_item", $"border_{dataCompare.Item2.ToString()}_rect");

            if (txtLevel != null && showLevel)
                txtLevel.text = $"Lv.{runeItem.Level.ToString()}";
            if (runeTypeIcon != null)
                runeTypeIcon.overrideSprite =
                    ResourceUtils.GetSpriteAtlas("rune", $"icon_{dataCompare.Item1.ToString()}");
            if (background != null)
                background.sprite =
                    ResourceUtils.GetSpriteAtlas("border_item", $"bgr_rarity_{dataCompare.Item2.ToString()}");

            if (runeItem.HeroIdEquip > 0)
            {
                equippedHero.overrideSprite =
                    ResourceUtils.GetSpriteHeroIcon($"hero_icon_info_{runeItem.HeroIdEquip.ToString()}");
                equippedHero.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                equippedHero.transform.parent.gameObject.SetActive(false);
            }

            //paint selected item on RuneTab
            if (inventory.SelectedRuneItems.Contains(RuneItem) && inventory is RuneTab)
            {
                //paint selected
                selectedSign.SetActive(true);
            }
            else
            {
                //paint not selected
                selectedSign.SetActive(false);
            }

            if (runeItem.Rarity == 0)
            {
                runeTypeIcon.rectTransform.anchoredPosition = new Vector2(1, 6);
            }
            else
            {
                runeTypeIcon.rectTransform.anchoredPosition = new Vector2(1, -3);
            }
        }

        private void RuneItemClick()
        {
            if (RuneDismantleSubWindow.IsInventoryOpen && Inventory is RuneTab)
            {
                var mainInventory = (RuneTab) Inventory;
                var selected = mainInventory.ToggleModifyItem(RuneItem);
                selectedSign.SetActive(selected);
                return;
            }
            else if (Inventory is RuneDismantleSubWindow)
            {
                return;
            }

            if (!UserData.Instance.UserHeroData.CheckHeroUnlocked(heroId))
            {
                Ultilities.ShowToastNoti(L.popup.unlock_hero_to_equip_rune);
                return;
            }

            var data = new RuneInfoWindowProperties(RuneItem, (RuneTab) Inventory, heroId);
            UIFrame.Instance.OpenWindow(ScreenIds.rune_inventory_info, data);
        }
    }
}