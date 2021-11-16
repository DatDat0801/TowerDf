using System;
using EW2.Spell;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class ItemSpellUi : MonoBehaviour
    {
        [SerializeField] private Image iconSpell;
        [SerializeField] private Image border;
        [SerializeField] private Button btn;
        [SerializeField] private Text txtLevel;
        [SerializeField] private Text txtFragment;
        [SerializeField] private Image progressFragment;
        [SerializeField] private Image iconUpgrade;
        [SerializeField] private Image avatarHeroUsed;
        [SerializeField] private GameObject goLock;
        [SerializeField] private GameObject goAvatarHero;
        [SerializeField] private Sprite imgProgressNormal;
        [SerializeField] private Sprite imgProgressFull;
        [SerializeField] private Sprite imgProgressLvlMax;

        private SpellItem spellData;
        private SpellUpgradeData spellUpgradeData;
        private int currHero;

        private void Awake()
        {
            btn.onClick.AddListener(SpellClick);
        }

        public void InitSpell(SpellItem data, int heroId)
        {
            this.spellData = data;
            this.spellUpgradeData = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                .GetSpellDataUpgrade(data.ItemId, data.Level + 1);
            this.currHero = heroId;

            ShowUi();
        }

        private void ShowUi()
        {
            iconSpell.sprite = ResourceUtils.GetSpriteAtlas("spell", $"icon_{spellData.ItemId}_0");
            border.sprite = ResourceUtils.GetSpriteAtlas("border_item", $"border_{spellData.Rarity}_rect");
            var levelMax = GameContainer.Instance.Get<InventoryDataBase>().Get<SpellDataBases>()
                .GetSpellLevelMax(spellData.ItemId);

            txtLevel.text = $"Lv.{spellData.Level}";

            if (spellData.HeroIdEquip > 0)
            {
                avatarHeroUsed.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_info_{spellData.HeroIdEquip}");
            }

            goLock.SetActive(spellData.HeroIdEquip > 0);
            goAvatarHero.SetActive(spellData.HeroIdEquip > 0);


            if (spellData.Level < levelMax)
            {
                var percent = spellData.GetFragments() * 1f / spellUpgradeData.reqFragment * 1f;
                if (percent < 1)
                    progressFragment.sprite = imgProgressNormal;
                else
                    progressFragment.sprite = imgProgressFull;
                progressFragment.fillAmount = percent;
                iconUpgrade.gameObject.SetActive(progressFragment.fillAmount >= 1);
                txtFragment.text = $"{spellData.GetFragments()}/{spellUpgradeData.reqFragment}";
            }

            else
            {
                txtFragment.text = L.popup.level_max_skill_txt;
                progressFragment.fillAmount = 1;
                progressFragment.sprite = imgProgressLvlMax;
                iconUpgrade.gameObject.SetActive(false);
            }
        }

        private void SpellClick()
        {
            if (!UserData.Instance.UserHeroData.CheckHeroUnlocked(currHero))
            {
                Ultilities.ShowToastNoti(L.popup.unlock_hero_to_equip);
                return;
            }

            SpellInfoWindowProperties data = new SpellInfoWindowProperties(spellData, currHero);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_spell_info, data);
        }
    }
}